using System.Collections;
using UnityEngine;

public class Asymmetry : BaseBossMetry
{
    [Header("左右分割ビーム")]
    [SerializeField] private GameObject halfBeamWhitePrefab;
    [SerializeField] private GameObject halfBeamBlackPrefab;

    [Header("インク弾（放物線）")]
    [SerializeField] private GameObject inkBulletWhitePrefab;
    [SerializeField] private GameObject inkBulletBlackPrefab;

    [Header("時間差爆発弾（ランダムばら撒き）")]
    [SerializeField] private GameObject delayedBombWhitePrefab;
    [SerializeField] private GameObject delayedBombBlackPrefab;
    [SerializeField] private int bombCount = 10;           // 生成する爆弾の数
    [SerializeField] private float bombSpawnRadius = 6.0f; // 爆弾をばら撒く半径

    [Header("HalfBeam 予備動作（予告Prefab）")]
    [SerializeField] private GameObject halfBeamWarningWhitePrefab; // 白ビーム用の予告Prefab
    [SerializeField] private GameObject halfBeamWarningBlackPrefab; // 黒ビーム用の予告Prefab
    [SerializeField] private float warningDuration = 0.8f;          // 予告を表示する時間（秒）

    //連続攻撃防止用の変数
    private int lastAttackType = -1;
    private int consecutiveCount = 0;

    protected override IEnumerator ExecuteAttackPattern()
    {
        int attackType;

        // 同じ技が3回連続（2回を超える連続）にならないよう選定
        while (true)
        {
            attackType = Random.Range(0, 3); // 0: Beam, 1: InkBullets, 2: Bombs

            if (attackType == lastAttackType)
            {
                // 直前と同じ技の場合、すでに2回連続していたら再抽選
                if (consecutiveCount < 2)
                {
                    consecutiveCount++;
                    break;
                }
            }
            else
            {
                // 新しい技が選ばれたらカウントリセット
                lastAttackType = attackType;
                consecutiveCount = 1;
                break;
            }
        }

        // 選ばれた技を実行
        switch (attackType)
        {
            case 0:
                yield return StartCoroutine(HalfAndHalfBeamRoutine());
                break;
            case 1:
                yield return StartCoroutine(ShootInkBulletsRoutine());
                break;
            case 2:
                yield return StartCoroutine(SpawnDelayedBombsRoutine());
                break;
        }

        isActionRunning = false;
    }

    // 技1: ステージ左右分割ビーム
    private IEnumerator HalfAndHalfBeamRoutine()
    {
        // 1. 左右の属性パターンを決定（左が白なら右は黒 / 左が黒なら右は白）
        bool isLeftWhite = Random.value > 0.5f;

        // 左右の配置座標を計算
        Vector3 centerPos = stageCenter != null ? stageCenter.position : transform.position;
        Vector3 leftSpawnPos = new Vector3(centerPos.x - 7.5f, centerPos.y, 0);
        Vector3 rightSpawnPos = new Vector3(centerPos.x + 7.5f, centerPos.y, 0);

        // 左右それぞれの予告PrefabとビームPrefabをセット
        GameObject leftWarningPrefab = isLeftWhite ? halfBeamWarningWhitePrefab : halfBeamWarningBlackPrefab;
        GameObject rightWarningPrefab = isLeftWhite ? halfBeamWarningBlackPrefab : halfBeamWarningWhitePrefab;

        GameObject leftBeamPrefab = isLeftWhite ? halfBeamWhitePrefab : halfBeamBlackPrefab;
        GameObject rightBeamPrefab = isLeftWhite ? halfBeamBlackPrefab : halfBeamWhitePrefab;

        // 2. 予備動作（左右それぞれの属性に対応した予告Objectを同時に生成）
        GameObject leftWarningInstance = null;
        GameObject rightWarningInstance = null;
        Vector3 warningOffset = new Vector3(0, 8f, 0); // 予告表示位置（Y軸オフセット）

        if (leftWarningPrefab != null)
        {
            leftWarningInstance = Instantiate(leftWarningPrefab, leftSpawnPos + warningOffset, Quaternion.identity);
        }
        if (rightWarningPrefab != null)
        {
            rightWarningInstance = Instantiate(rightWarningPrefab, rightSpawnPos + warningOffset, Quaternion.identity);
        }

        SoundManager.Instance?.PlaySE(SoundManager.SEType.BuildUp);

        // 予告を表示して待機
        yield return new WaitForSeconds(warningDuration);

        // 予告Objectを消去
        if (leftWarningInstance != null) Destroy(leftWarningInstance);
        if (rightWarningInstance != null) Destroy(rightWarningInstance);

        // 3. 実際のビーム攻撃を左右同時に生成
        GameObject leftBeamInstance = null;
        GameObject rightBeamInstance = null;

        if (leftBeamPrefab != null)
        {
            leftBeamInstance = Instantiate(leftBeamPrefab, leftSpawnPos, Quaternion.identity);
        }
        if (rightBeamPrefab != null)
        {
            rightBeamInstance = Instantiate(rightBeamPrefab, rightSpawnPos, Quaternion.identity);
        }

        SoundManager.Instance?.PlaySE(SoundManager.SEType.GiantBeam);

        // ビーム持続時間待機
        yield return new WaitForSeconds(3.0f);

        // ビームを消去
        if (leftBeamInstance != null) Destroy(leftBeamInstance);
        if (rightBeamInstance != null) Destroy(rightBeamInstance);
    }

    // 技2: 白黒インク弾のランダム噴射（放物線）
    private IEnumerator ShootInkBulletsRoutine()
    {
        int bulletCount = 16;
        for (int i = 0; i < bulletCount; i++)
        {
            if (isDead) yield break;

            // 白か黒かを完全ランダムで選ぶ
            bool isWhiteInk = Random.value > 0.5f;
            GameObject selectedPrefab = isWhiteInk ? inkBulletWhitePrefab : inkBulletBlackPrefab;

            if (selectedPrefab != null)
            {
                SoundManager.Instance?.PlaySE(SoundManager.SEType.PaintAttack);

                // ボスの上部周辺から打ち出す
                Vector3 spawnPos = transform.position + new Vector3(Random.Range(-1.5f, 1.5f), 1f, 0);
                Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
            }

            yield return new WaitForSeconds(0.15f);
        }
        yield return new WaitForSeconds(1.5f);
    }

    // 技3: 周囲へ不規則・ランダム色での時間差爆発弾配置
    private IEnumerator SpawnDelayedBombsRoutine()
    {
        for (int i = 0; i < bombCount; i++)
        {
            if (isDead) yield break;

            // 1. 色（属性）をランダムで決定
            bool isWhiteBomb = Random.value > 0.5f;
            GameObject selectedPrefab = isWhiteBomb ? delayedBombWhitePrefab : delayedBombBlackPrefab;

            if (selectedPrefab != null)
            {
                // 2. 規則的ではなく、円の内部でランダムな位置オフセットを計算
                Vector2 randomCircle = Random.insideUnitCircle * bombSpawnRadius;
                Vector3 spawnPos = transform.position + new Vector3(randomCircle.x, randomCircle.y, 0);

                SoundManager.Instance?.PlaySE(SoundManager.SEType.Generate);
                Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
            }

            // 少しだけ時間差をつけてバババッとバラ撒く演出
            yield return new WaitForSeconds(0.08f);
        }

        yield return new WaitForSeconds(2.0f);
    }
}