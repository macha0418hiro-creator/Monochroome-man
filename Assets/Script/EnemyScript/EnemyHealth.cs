using System;
using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("敵の最大体力")]
    [SerializeField] private int maxHp = 1;
    private int currentHp;

    [Header("ボスかどうか")]
    [SerializeField] private bool isBoss = false;

    [Header("無敵時間・点滅設定")]
    [SerializeField] private float invincibilityDuration = 1.5f; // 無敵時間の長さ（秒）
    [SerializeField] private float flashInterval = 0.15f;         // 点滅の切り替え間隔（秒）

    private bool isInvincible = false;      // 現在無敵状態かどうか
    private SpriteRenderer spriteRenderer;  // 点滅用のRenderer

    //外部(UI)からHP情報を取得するためのプロパティ
    public int MaxHp => maxHp;
    public int CurrentHp => currentHp;
    public bool IsBoss => isBoss;

    //HP変更時と死亡時にUIへ通知するイベント
    public event Action OnSpawned;
    public event Action<int> OnHpChanged;
    public event Action OnDied;

    private void Awake()
    {
        // 1. まずは自分自身(EnemyHealth)が本体(Sprite持ち)であるケースを探す
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 2. もし自分になければ、親・子・自分を含めた階層全体から探す
        if (spriteRenderer == null)
        {
            // GetComponentInParent は自分から親、そのさらに親へ遡って探す
            spriteRenderer = GetComponentInParent<SpriteRenderer>();
        }

        // 3. 親にもなければ(稀ですが)、自分より下の階層(子)を探す
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void OnEnable()
    {
        currentHp = maxHp;
        isInvincible = false;

        // 再生成時に姿が見えるようにアルファ値を1に戻しておく
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }

        OnSpawned?.Invoke();
    }

    public void TakeDamage(int damage)
    {
        // 無敵時間中ならダメージを受け付けない
        if (isInvincible) return;

        currentHp -= damage;
        OnHpChanged?.Invoke(currentHp);

        if (currentHp > 0)
        {
            SoundManager.Instance?.PlaySE(SoundManager.SEType.Damage);

            StartCoroutine(InvincibilityRoutine());
        }
        else
        {
            Die();
        }
    }

    // 無敵時間＆点滅を制御するコルーチン
    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;

        float timer = 0f;
        while (timer < invincibilityDuration)
        {
            if (spriteRenderer != null)
            {
                // 点滅（アルファ値を 0.2 と 1.0 で交互に切り替え）
                Color color = spriteRenderer.color;
                color.a = (color.a == 1f) ? 0.2f : 1f;
                spriteRenderer.color = color;
            }

            yield return new WaitForSeconds(flashInterval);
            timer += flashInterval;
        }

        // 無敵時間終了時に不透明度を100%に戻す
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }

        isInvincible = false;
    }

    private void Die()
    {
        if (isBoss)
        {
            //Debug.Log($"Boss {gameObject.name}を倒した");
        }
        else
        {
            //Debug.Log($"Enemy {gameObject.name}を倒した");
        }

        SoundManager.Instance?.PlaySE(SoundManager.SEType.Disappearance);

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
