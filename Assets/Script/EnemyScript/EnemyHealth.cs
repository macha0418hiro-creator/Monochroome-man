using Unity.VisualScripting;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("敵の最大体力")]
    [SerializeField] private int maxHp = 1;
    private int currentHp;

    [Header("ボスかどうか")]
    [SerializeField] private bool isBoss = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHp = maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        Debug.Log($"{gameObject.name}は{damage}ダメージ受けた(残りHP:{currentHp})");

        if(currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isBoss)
        {
            Debug.Log($"Boss {gameObject.name}を倒した");
        }
        else
        {
            Debug.Log($"Enemy {gameObject.name}を倒した");
        }
    }
}
