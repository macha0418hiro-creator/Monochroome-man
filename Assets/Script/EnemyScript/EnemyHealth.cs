using System;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("敵の最大体力")]
    [SerializeField] private int maxHp = 1;
    private int currentHp;

    [Header("ボスかどうか")]
    [SerializeField] private bool isBoss = false;

    //外部(UI)からHP情報を取得するためのプロパティ
    public int MaxHp => maxHp;
    public int CurrentHp => currentHp;
    public bool IsBoss => isBoss;

    //HP変更時と死亡時にUIへ通知するイベント
    public event Action OnSpawned;
    public event Action<int> OnHpChanged;
    public event Action OnDied;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        currentHp = maxHp;
        OnSpawned?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        Debug.Log($"{gameObject.name}は{damage}ダメージ受けた(残りHP:{currentHp})");

        OnHpChanged?.Invoke(currentHp); //HPが変化したことをUIに通知

        if (currentHp <= 0)
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

        OnDied?.Invoke();   //死亡したことをUIに通知

        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
