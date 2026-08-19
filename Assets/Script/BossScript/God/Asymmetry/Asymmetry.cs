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

    protected override IEnumerator ExecuteAttackPattern()
    {
        int attackType = Random.Range(0, 3);
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
        bool isLeftWhite = Random.value > 0.5f;
        Vector3 centerPos = stageCenter != null ? stageCenter.position : Vector3.zero;

        yield return new WaitForSeconds(1.2f); // 予兆時間

        float beamWidth = 10f;
        float halfWidth = beamWidth / 2f;

        Vector3 leftPos = centerPos + new Vector3(-halfWidth, 0, 0);
        Vector3 rightPos = centerPos + new Vector3(halfWidth, 0, 0);

        GameObject leftBeam = Instantiate(isLeftWhite ? halfBeamWhitePrefab : halfBeamBlackPrefab, leftPos, Quaternion.identity);
        GameObject rightBeam = Instantiate(isLeftWhite ? halfBeamBlackPrefab : halfBeamWhitePrefab, rightPos, Quaternion.identity);

        yield return new WaitForSeconds(1.5f);

        if (leftBeam != null) Destroy(leftBeam);
        if (rightBeam != null) Destroy(rightBeam);
    }

    // 技2: 白黒インク弾のランダム噴射（放物線）
    private IEnumerator ShootInkBulletsRoutine()
    {
        int bulletCount = 8;
        for (int i = 0; i < bulletCount; i++)
        {
            if (isDead) yield break;

            // 白か黒かを完全ランダムで選ぶ
            bool isWhiteInk = Random.value > 0.5f;
            GameObject selectedPrefab = isWhiteInk ? inkBulletWhitePrefab : inkBulletBlackPrefab;

            if (selectedPrefab != null)
            {
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

                Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
            }

            // 少しだけ時間差をつけてバババッとバラ撒く演出
            yield return new WaitForSeconds(0.08f);
        }

        yield return new WaitForSeconds(2.0f);
    }
}