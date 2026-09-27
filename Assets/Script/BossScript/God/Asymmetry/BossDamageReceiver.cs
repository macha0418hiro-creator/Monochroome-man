using UnityEngine;

public class BossDamageReceiver : MonoBehaviour
{
    private EnemyHealth parentHealth;

    private void Awake()
    {
        // 親（Asymmetry）についている EnemyHealth を取得
        parentHealth = GetComponentInParent<EnemyHealth>();
    }

    public void TakeDamage(int damage)
    {
        if (parentHealth != null)
        {
            parentHealth.TakeDamage(damage);
        }
    }
}