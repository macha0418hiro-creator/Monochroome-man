using System;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("攻撃設定")]
    [SerializeField] private int damage = 1;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 2Dの当たり判定（Collider2D）が重なった瞬間に自動で呼ばれる関数
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ぶつかった相手が「PlayerHealth」コンポーネントを持っているか確認
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

        if(playerHealth != null)
        {
            //ダメージを与える
            playerHealth.TakeDamage(damage, transform.position);
        }
    }

    // もしエネミーのコライダーを「Is Trigger（すり抜ける設定）」にしている場合は、下の関数が動く
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage, transform.position);
        }
    }
}
