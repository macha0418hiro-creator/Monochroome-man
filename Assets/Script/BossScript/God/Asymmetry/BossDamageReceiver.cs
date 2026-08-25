using UnityEngine;

public class BossDamageReceiver : MonoBehaviour
{
    private EnemyHealth parentHealth;

    private void Awake()
    {
        // 親（Asymmetry）についている EnemyHealth を取得
        parentHealth = GetComponentInParent<EnemyHealth>();
    }

    // プレイヤーの攻撃（Is Trigger）が当たった時
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // プレイヤーの攻撃スクリプト（例: PlayerAttack や Bullet など）からダメージを受ける処理
        // ※ 既存のプレイヤー攻撃処理に合わせて適宜調整してください
        /* 
        PlayerAttack attack = collision.GetComponent<PlayerAttack>();
        if (attack != null && parentHealth != null)
        {
            parentHealth.TakeDamage(attack.Damage);
        }
        */
    }

    // もし直接 TakeDamage を呼ばれる仕組みの場合の受け皿
    public void TakeDamage(int damage)
    {
        if (parentHealth != null)
        {
            parentHealth.TakeDamage(damage);
        }
    }
}