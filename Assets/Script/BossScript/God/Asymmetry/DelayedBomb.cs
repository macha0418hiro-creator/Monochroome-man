using System.Collections;
using UnityEngine;

public class DelayedBomb : MonoBehaviour
{
    [Header("爆発設定")]
    [SerializeField] private float delayTime = 2.0f; // 爆発までの時間
    [SerializeField] private float explosionDuration = 0.5f; // 爆発判定の持続時間
    [SerializeField] private GameObject explosionEffectPrefab; // 爆発エフェクト（あれば）

    private Collider2D bombCollider;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        bombCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        StartCoroutine(ExplodeRoutine());
    }

    private IEnumerator ExplodeRoutine()
    {
        // 1. 設置直後はダメージ判定をOFFにしておく（点滅などの予兆演出）
        if (bombCollider != null) bombCollider.enabled = false;

        float elapsed = 0f;
        while (elapsed < delayTime)
        {
            elapsed += Time.deltaTime;
            // 簡易的な点滅演出
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = (Mathf.FloorToInt(elapsed * 10) % 2 == 0);
            }
            yield return null;
        }

        // --- 爆発発生 ---

        // 2. 爆発エフェクトの生成
        if (explosionEffectPrefab != null)
        {
            GameObject effect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 2.0f); // エフェクトの自動消滅
        }

        // 3. 爆弾本体の「見た目」を消す（エフェクトと重ならないようにする）
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        // 4. 爆発判定をON
        if (bombCollider != null) bombCollider.enabled = true;

        // 5. 攻撃判定の持続時間を待つ
        yield return new WaitForSeconds(explosionDuration);

        // 6. 爆弾オブジェクト自体の削除
        Destroy(gameObject);
    }
}