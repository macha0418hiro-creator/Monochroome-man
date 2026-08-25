using UnityEngine;

public class BossClearManager : MonoBehaviour
{
    [Header("監視対象のボス")]
    [SerializeField] private EnemyHealth targetBoss;

    [Header("撃破時に生成するオブジェクト")]
    [SerializeField] private GameObject spawnObject1; // クリアポータルなど
    [SerializeField] private GameObject spawnObject2; // 宝箱など
    [SerializeField] private GameObject spawnObject3; // ジェムなど
    [SerializeField] private GameObject spawnObject4;

    [Header("撃破時に消去する攻撃のタグ")]
    [SerializeField] private string attackTag = "BossAttack";

    private void OnEnable()
    {
        if (targetBoss != null)
        {
            // EnemyHealth の死亡イベントを購読
            targetBoss.OnDied += HandleBossDied;
        }
    }

    private void OnDisable()
    {
        if (targetBoss != null)
        {
            // 登録解除（メモリリーク防止）
            targetBoss.OnDied -= HandleBossDied;
        }
    }

    private void HandleBossDied()
    {
        ClearBossAttacks();

        if (spawnObject1 == null) return;

        // オブジェクトの生成
        spawnObject1.SetActive(true);

        if (spawnObject2 == null) return;

        // オブジェクトの生成
        spawnObject2.SetActive(true);

        if (spawnObject3 == null) return;

        // オブジェクトの生成
        spawnObject3.SetActive(true);

        if (spawnObject4 == null) return;

        // オブジェクトの生成
        spawnObject4.SetActive(true);
    }

    private void ClearBossAttacks()
    {
        if (string.IsNullOrEmpty(attackTag)) return;

        // 指定タグの付いた全ゲームオブジェクトを取得して削除
        GameObject[] attackObjects = GameObject.FindGameObjectsWithTag(attackTag);
        foreach (GameObject obj in attackObjects)
        {
            Destroy(obj);
        }
    }
}