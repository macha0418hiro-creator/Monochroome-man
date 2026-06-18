using System;
using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("HP設定")]
    [SerializeField] private int maxHp = 3;
    private int currentHp;

    [Header("無敵時間設定")]
    [SerializeField] private float invincibilityDuration = 1.5f; //ダメージ後の無敵時間
    private bool isInvicible;                                    //現在無敵かどうか

    [Header("点滅エフェクト設定")]
    [SerializeField] private SpriteRenderer spriteRenderer;     //プレイヤーのSpriteRenderer
    [SerializeField] private float blinkInterval = 0.1f;               //点滅の速さ

    [Header("ノックバック設定")]
    [SerializeField] private float knockbackForce = 0.8f;       //ノックバックの強さ

    [Header("連動するUI")]
    [SerializeField] private PlayerHpUI hpUI;

    private Rigidbody2D rb;

    public bool IsKnockbacking { get; private set; }

    void Start()
    {
        currentHp = maxHp;
        rb = GetComponent<Rigidbody2D>();

        //ゲーム開始時にUIを現在の満タンHPに合わせる
        if (hpUI != null)
        {
            hpUI.UpdateHpUI(currentHp);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //外部(エネミーなど)空ダメージを受ける処理
    public void TakeDamage(int damageAmount, Vector2 attackerPosition)
    {
        //無敵時間中は処理しない
        if (isInvicible) return;

        currentHp -= damageAmount;
        Debug.Log($"プレイヤーがダメージ! 残りHP{currentHp}");

        //HPが減ったことをUIに伝えてハートをグレーにしてもらう ---
        if (hpUI != null)
        {
            hpUI.UpdateHpUI(currentHp);
        }

        // 1. 攻撃者の位置から自分の位置への方向（ベクトル）を計算する
        // 自分（の座標） - 敵（の座標） = 敵から自分へ向かう向き
        float xDirection = transform.position.x - attackerPosition.x;
        float signX = Mathf.Sign(xDirection);

        // 2. 斜め上に気持ちよく飛ぶように、上方向（Y軸）のベクトルを少しプラスして調整する
        Vector2 knockbackDirection = new Vector2(signX, 0.7f);
        knockbackDirection = knockbackDirection.normalized;

        // 3. 瞬間的な力を加える（ジャンプと同じ Impulse）
        // 一度速度をリセットしてから力を加えると、移動入力に負けずに綺麗に吹っ飛ぶらしい
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        if(currentHp <= 0)
        {
            Die();
        }
        else
        {
            // 無敵時間と点滅のカウントダウンを開始
            StartCoroutine(DamageRoutine());
        }
    }

    // 無敵時間と点滅を同時に管理
    private IEnumerator DamageRoutine()
    {
        isInvicible = true;

        IsKnockbacking = true; //ノックバックフラグ

        float knockbackDuration = 0.4f;

        float elapsed = 0f;

        //無敵時間が終わるまでループ
        while(elapsed < invincibilityDuration)
        {
            //移動操作を制限
            if(elapsed >= knockbackDuration && IsKnockbacking)
            {
                IsKnockbacking = false;
            }

            // SpriteRendererの表示・非表示を切り替えて点滅
            spriteRenderer.enabled = !spriteRenderer.enabled;

            // blinkIntervalだけ処理を一時停止してUnityに時間を進めさせる(点滅の間隔)
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        spriteRenderer.enabled = true;
        isInvicible = false;
        Debug.Log("無敵時間が終了しました");
    }


    private void Die()
    {
        Debug.Log("プレイヤーは倒れた！");

        //***ここにゲームオーバー画面の表示や、シーンの再読み込みなどを後々追加***
        gameObject.SetActive(false); // とりあえず消す
    }
}
