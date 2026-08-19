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
    [SerializeField] private GameObject starRainInvisibleWhitePrefab; // ★追加: 白用Invisible
    [SerializeField] private GameObject centerRotatingBeamWhitePrefab;
    [SerializeField] private GameObject fullScreenAuraWhitePrefab;

    [Header("攻撃Prefab（黒属性用）")]
    [SerializeField] private GameObject starRainVisibleBlackPrefab;
    [SerializeField] private GameObject starRainInvisibleBlackPrefab; // ★追加: 黒用Invisible
    [SerializeField] private GameObject centerRotatingBeamBlackPrefab;
    [SerializeField] private GameObject fullScreenAuraBlackPrefab;

    [Header("共通Prefab・演出")]
    [SerializeField] private GameObject centerWallBarrier; // 中央遮断壁

    private SpriteRenderer mySpriteRenderer;

    protected override void Awake()
    {
        base.Awake();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Start()
    {
        base.Start();
        UpdateGodVisuals();

        // 起動時に中央壁を確実に非表示にしておく
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
    }

    private void UpdateGodVisuals()
    {
        if (attributeController == null || mySpriteRenderer == null) return;

        bool isWhite = (attributeController.CurrentAttribute == EnemyAttributeController.EnemyColor.White);
        mySpriteRenderer.sprite = isWhite ? whiteBossSprite : blackBossSprite;
    }

    protected override IEnumerator ExecuteAttackPattern()
    {
        int attackType = Random.Range(0, 3);
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
        }

        isActionRunning = false;
    }

    // 技1: 星降らし（StarRainの時だけCenterWallBarrierを出現させる）
    private IEnumerator SymmetricalStarRainRoutine()
    {
        bool isBossWhite = (attributeController.CurrentAttribute == EnemyAttributeController.EnemyColor.White);
        GameObject visibleStarPrefab = isBossWhite ? starRainVisibleWhitePrefab : starRainVisibleBlackPrefab;
        GameObject invisibleStarPrefab = isBossWhite ? starRainInvisibleWhitePrefab : starRainInvisibleBlackPrefab; // ★属性別のInvisibleを選択

        bool isLeftVisible = Random.value > 0.5f;
        Vector3 centerPos = stageCenter != null ? stageCenter.position : Vector3.zero;

        // --- 中央遮断壁の出現 ---
        GameObject barrierInstance = null;
        if (centerWallBarrier != null)
        {
            if (centerWallBarrier.scene.rootCount > 0)
            {
                centerWallBarrier.SetActive(true); // ヒエラルキー上のオブジェクトをON
            }
            else
            {
                barrierInstance = Instantiate(centerWallBarrier, centerPos, Quaternion.identity); // Prefabの場合生成
            }
        }

        if (playerTransform != null)
        {
            float targetX = isLeftVisible ? centerPos.x + 5f : centerPos.x - 5f;
            playerTransform.position = new Vector3(targetX, playerTransform.position.y, playerTransform.position.z);
        }

        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < 8; i++)
        {
            if (isDead) break;

            float spawnXOffset = Random.Range(1f, 9f);
            float spawnY = centerPos.y + 8f;

            Vector3 visiblePos = centerPos + new Vector3(isLeftVisible ? -spawnXOffset : spawnXOffset, spawnY, 0);
            Vector3 invisiblePos = centerPos + new Vector3(isLeftVisible ? spawnXOffset : -spawnXOffset, spawnY, 0);

            if (visibleStarPrefab != null) Instantiate(visibleStarPrefab, visiblePos, Quaternion.identity);
            if (invisibleStarPrefab != null) Instantiate(invisibleStarPrefab, invisiblePos, Quaternion.identity);

            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(1.5f);

        // --- 中央遮断壁の消去（StarRain終了時） ---
        if (centerWallBarrier != null && centerWallBarrier.scene.rootCount > 0)
        {
            centerWallBarrier.SetActive(false); // ヒエラルキー上のオブジェクトをOFF
        }
        else if (barrierInstance != null)
        {
            Destroy(barrierInstance); // 生成したPrefabを削除
        }
    }

    // 技2: 中央回転ビーム
    private IEnumerator RotatingBeamRoutine()
    {
        bool isBossWhite = (attributeController.CurrentAttribute == EnemyAttributeController.EnemyColor.White);
        GameObject beamPrefab = isBossWhite ? centerRotatingBeamWhitePrefab : centerRotatingBeamBlackPrefab;

        Vector3 centerPos = stageCenter != null ? stageCenter.position : Vector3.zero;

        yield return new WaitForSeconds(0.8f);

        if (beamPrefab != null)
        {
            GameObject beam = Instantiate(beamPrefab, centerPos, Quaternion.identity);

            float duration = 4.0f;
            float elapsed = 0f;
            float rotateSpeed = 90f;

            while (elapsed < duration && !isDead)
            {
                elapsed += Time.deltaTime;
                if (beam != null)
                {
                    // Transformの回転
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
        bool isBossWhite = (attributeController.CurrentAttribute == EnemyAttributeController.EnemyColor.White);
        bool auraIsWhite = !isBossWhite;
        GameObject auraPrefab = auraIsWhite ? fullScreenAuraWhitePrefab : fullScreenAuraBlackPrefab;

        Vector3 spawnPos = stageCenter != null ? stageCenter.position : Vector3.zero;
        GameObject auraInstance = null;

        if (auraPrefab != null)
        {
            auraInstance = Instantiate(auraPrefab, spawnPos, Quaternion.identity);
        }

        if (playerTransform != null)
        {
            PlayerAttributeController playerAttr = playerTransform.GetComponent<PlayerAttributeController>();
            if (playerAttr != null)
            {
                // 第1引数: 強制変更する属性(bool), 第2引数: 禁止時間(秒)
                // オーラが消えるタイミングとは独立して、発動した瞬間から5秒間禁止されます
                playerAttr.ApplyAuraColorLock(auraIsWhite, 20.0f);
            }
        }

        yield return new WaitForSeconds(5.0f);

        if (auraInstance != null) Destroy(auraInstance);
    }
}