using System.Collections;
using UnityEngine;

public class Symmetry : BaseBossMetry
{
    [Header("属性設定")]
    [SerializeField] private float attributeChangeInterval = 8.0f;
    [SerializeField] private Sprite whiteBossSprite;
    [SerializeField] private Sprite blackBossSprite;
    private float attributeTimer = 0f;

    [Header("攻撃Prefab（白属性用）")]
    [SerializeField] private GameObject starRainVisibleWhitePrefab;
    [SerializeField] private GameObject starRainInvisibleWhitePrefab;
    [SerializeField] private GameObject centerRotatingBeamWhitePrefab;
    [SerializeField] private GameObject homingBulletBlackPrefab;
    [SerializeField] private GameObject fullScreenAuraWhitePrefab;

    [Header("攻撃Prefab（黒属性用）")]
    [SerializeField] private GameObject starRainVisibleBlackPrefab;
    [SerializeField] private GameObject starRainInvisibleBlackPrefab;
    [SerializeField] private GameObject centerRotatingBeamBlackPrefab;
    [SerializeField] private GameObject homingBulletWhitePrefab;
    [SerializeField] private GameObject fullScreenAuraBlackPrefab;

    [Header("CenterRotatingBeam 予備動作（予告Prefab）")]
    [SerializeField] private GameObject centerBeamWarningWhitePrefab; // 白ビーム用の予告Prefab
    [SerializeField] private GameObject centerBeamWarningBlackPrefab; // 黒ビーム用の予告Prefab
    [SerializeField] private float beamWarningDuration = 1.0f;        // 予告表示時間（秒）

    [Header("共通Prefab・演出")]
    [SerializeField] private GameObject centerWallBarrier;

    private SpriteRenderer mySpriteRenderer;
    private bool justChangedAttribute = false; // 色変更直後かどうか
    private bool canUseAura = false;           // オーラ発動可能フラグ

    //連続攻撃防止用の変数
    private int lastAttackType = -1;
    private int consecutiveCount = 0;

    protected override void Awake()
    {
        base.Awake();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Start()
    {
        base.Start();
        UpdateGodVisuals();

        if (centerWallBarrier != null && centerWallBarrier.scene.rootCount > 0)
        {
            centerWallBarrier.SetActive(false);
        }
    }

    private void Update()
    {
        if (isDead) return;

        // 攻撃中(isActionRunning == true)は属性切り替えを行わない
        if (!isActionRunning)
        {
            attributeTimer += Time.deltaTime;
            if (attributeTimer >= attributeChangeInterval)
            {
                attributeTimer = 0f;
                ToggleAttribute();
            }
        }
    }

    private void ToggleAttribute()
    {
        if (attributeController == null) return;

        bool isCurrentWhite = (attributeController.CurrentAttribute == EnemyAttributeController.EnemyColor.White);
        EnemyAttributeController.EnemyColor nextColor = isCurrentWhite ? EnemyAttributeController.EnemyColor.Black : EnemyAttributeController.EnemyColor.White;

        attributeController.ApplyAttribute(nextColor);
        UpdateGodVisuals();

        // 色変更が行われたため、オーラ発動権を付与
        canUseAura = true;
    }

    private void UpdateGodVisuals()
    {
        if (attributeController == null || mySpriteRenderer == null) return;

        bool isWhite = (attributeController.CurrentAttribute == EnemyAttributeController.EnemyColor.White);
        mySpriteRenderer.sprite = isWhite ? whiteBossSprite : blackBossSprite;
    }

    protected override IEnumerator ExecuteAttackPattern()
    {
        int attackType;

        // 同じ技が3回連続（2回を超える連続）にならないよう選定
        while (true)
        {
            // オーラ(2) が使用可能な場合はオーラを含めて抽選
            if (canUseAura)
            {
                attackType = Random.value < 0.7f ? 2 : Random.Range(0, 4);
            }
            else
            {
                // オーラ以外の技（0: StarRain, 1: Beam, 3: Homing）から抽選
                int[] availableAttacks = { 0, 1, 3 };
                attackType = availableAttacks[Random.Range(0, availableAttacks.Length)];
            }

            // 連続判定のチェック
            if (attackType == lastAttackType)
            {
                if (consecutiveCount < 2)
                {
                    consecutiveCount++;
                    break;
                }
                // consecutiveCount が 2 の場合は loop により再抽選される
            }
            else
            {
                lastAttackType = attackType;
                consecutiveCount = 1;
                break;
            }
        }

        // 選ばれた技を実行
        switch (attackType)
        {
            case 0:
                yield return StartCoroutine(SymmetricalStarRainRoutine());
                break;
            case 1:
                yield return StartCoroutine(RotatingBeamRoutine());
                break;
            case 2:
                yield return StartCoroutine(AuraColorLockRoutine());
                break;
            case 3:
                yield return StartCoroutine(HomingBulletsRoutine());
                break;
        }

        isActionRunning = false;
    }

    // 技1: 星降らし
    private IEnumerator SymmetricalStarRainRoutine()
    {
        bool isBossWhite = (attributeController.CurrentAttribute == EnemyAttributeController.EnemyColor.White);
        GameObject visibleStarPrefab = isBossWhite ? starRainVisibleWhitePrefab : starRainVisibleBlackPrefab;
        GameObject invisibleStarPrefab = isBossWhite ? starRainInvisibleWhitePrefab : starRainInvisibleBlackPrefab;

        Vector3 centerPos = stageCenter != null ? stageCenter.position : Vector3.zero;

        // 壁の出現
        GameObject barrierInstance = null;
        if (centerWallBarrier != null)
        {
            SoundManager.Instance?.PlaySE(SoundManager.SEType.Teleport);

            if (centerWallBarrier.scene.rootCount > 0) centerWallBarrier.SetActive(true);
            else barrierInstance = Instantiate(centerWallBarrier, centerPos, Quaternion.identity);
        }

        // 1. 飛ばす方向（True: 左側に集める / False: 右側に集める）を確実に 50% で判定
        bool sendToLeft = Random.Range(0, 2) == 0;

        // 2. プレイヤーの移動位置を計算
        if (playerTransform != null)
        {
            float currentX = playerTransform.position.x;
            float targetX = currentX;

            if (sendToLeft)
            {
                // 左側に飛ばしたい時
                if (currentX >= centerPos.x)
                {
                    // 右側にいるなら Center を軸に線対称の左側へ移動
                    targetX = 2f * centerPos.x - currentX;
                }
                // すでに左側(currentX < centerPos.x)ならそのままの位置を保持
            }
            else
            {
                // 右側に飛ばしたい時
                if (currentX <= centerPos.x)
                {
                    // 左側にいるなら Center を軸に線対称の右側へ移動
                    targetX = 2f * centerPos.x - currentX;
                }
                // すでに右側(currentX > centerPos.x)ならそのままの位置を保持
            }

            playerTransform.position = new Vector3(targetX, playerTransform.position.y, playerTransform.position.z);
        }

        yield return new WaitForSeconds(1.0f);

        // 3. 星降らしの生成処理（飛ばされた側（sendToLeft側）に見えない星を降らせる）
        for (int i = 0; i < 8; i++)
        {
            if (isDead) break;

            float spawnXOffset = Random.Range(1f, 9f);
            float spawnY = centerPos.y + 8f;

            // sendToLeft が true（左側）のときは、左側(-spawnXOffset)に見えない星、右側(+spawnXOffset)に見える星を配置
            Vector3 invisiblePos = centerPos + new Vector3(sendToLeft ? -spawnXOffset : spawnXOffset, spawnY, 0);
            Vector3 visiblePos = centerPos + new Vector3(sendToLeft ? spawnXOffset : -spawnXOffset, spawnY, 0);

            SoundManager.Instance?.PlaySE(SoundManager.SEType.Generate);

            if (visibleStarPrefab != null) Instantiate(visibleStarPrefab, visiblePos, Quaternion.identity);
            if (invisibleStarPrefab != null) Instantiate(invisibleStarPrefab, invisiblePos, Quaternion.identity);

            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(1.5f);

        // 壁の消去
        if (centerWallBarrier != null && centerWallBarrier.scene.rootCount > 0)
        {
            centerWallBarrier.SetActive(false);
        }
        else if (barrierInstance != null)
        {
            Destroy(barrierInstance);
        }
    }

    // 技2: 中央回転ビーム
    private IEnumerator RotatingBeamRoutine()
    {
        Vector3 centerPos = stageCenter != null ? stageCenter.position : Vector3.zero;

        // ボス本体を中央へ移動
        transform.position = new Vector3(centerPos.x, transform.position.y, transform.position.z);

        bool isBossWhite = (attributeController.CurrentAttribute == EnemyAttributeController.EnemyColor.White);
        GameObject beamPrefab = isBossWhite ? centerRotatingBeamWhitePrefab : centerRotatingBeamBlackPrefab;
        GameObject warningPrefab = isBossWhite ? centerBeamWarningWhitePrefab : centerBeamWarningBlackPrefab;

        // 1. 予備動作
        if (warningPrefab != null)
        {
            SoundManager.Instance?.PlaySE(SoundManager.SEType.BuildUp);

            GameObject warningInstance = Instantiate(warningPrefab, centerPos, Quaternion.identity);
            yield return new WaitForSeconds(beamWarningDuration);
            Destroy(warningInstance);
        }
        else
        {
            yield return new WaitForSeconds(0.8f);
        }

        // 2. 実際のビーム攻撃の開始
        if (beamPrefab != null)
        {
            SoundManager.Instance?.PlaySE(SoundManager.SEType.Beam);

            GameObject beam = Instantiate(beamPrefab, centerPos, Quaternion.identity);

            float duration = 4.0f;
            float elapsed = 0f;
            float rotateSpeed = 90f;

            while (elapsed < duration && !isDead)
            {
                elapsed += Time.deltaTime;
                if (beam != null)
                {
                    beam.transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
                }
                yield return null;
            }

            if (beam != null) Destroy(beam);
        }
    }

    // 技3: 全画面オーラ
    private IEnumerator AuraColorLockRoutine()
    {
        canUseAura = false;

        bool isBossWhite = (attributeController.CurrentAttribute == EnemyAttributeController.EnemyColor.White);
        bool auraIsWhite = !isBossWhite;
        GameObject auraPrefab = auraIsWhite ? fullScreenAuraWhitePrefab : fullScreenAuraBlackPrefab;

        Vector3 spawnPos = stageCenter != null ? stageCenter.position : Vector3.zero;
        GameObject auraInstance = null;

        // 1. オーラ演出（エフェクト）の生成
        if (auraPrefab != null)
        {
            SoundManager.Instance?.PlaySE(SoundManager.SEType.Magic);

            auraInstance = Instantiate(auraPrefab, spawnPos, Quaternion.identity);
        }

        // 2. プレイヤーの属性ロック処理（ロック時間: 5秒）
        if (playerTransform != null)
        {
            PlayerAttributeController playerAttr = playerTransform.GetComponent<PlayerAttributeController>();
            if (playerAttr != null)
            {
                playerAttr.ApplyAuraColorLock(auraIsWhite, 15.0f);
            }
        }

        // ★ 3. オーラ演出自体は1.0秒で消去（演出時間はお好みで調整してください）
        yield return new WaitForSeconds(1.0f);
        if (auraInstance != null) Destroy(auraInstance);

        // ★ 4. ボス側の攻撃後の隙時間（必要に応じて待機時間を調整）
        yield return new WaitForSeconds(1.0f);
    }

    // 技4: 追尾弾の複数生成
    private IEnumerator HomingBulletsRoutine()
    {
        int homingBulletCount = 4;

        bool isBossWhite = (attributeController.CurrentAttribute == EnemyAttributeController.EnemyColor.White);
        GameObject bulletPrefab = isBossWhite ? homingBulletWhitePrefab : homingBulletBlackPrefab;

        if (bulletPrefab != null)
        {
            for (int i = 0; i < homingBulletCount; i++)
            {
                if (isDead) yield break;

                SoundManager.Instance?.PlaySE(SoundManager.SEType.HomingBullet);

                Vector3 spawnOffset = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(0.5f, 2.0f), 0);
                Instantiate(bulletPrefab, transform.position + spawnOffset, Quaternion.identity);

                yield return new WaitForSeconds(0.4f);
            }
        }

        yield return new WaitForSeconds(2.0f);
    }
}