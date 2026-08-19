using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class InkBullet : MonoBehaviour
{
    [Header("弾の属性")]
    [SerializeField] private bool isWhiteInk = true; // 白インクならTrue、黒インクならFalse

    [Header("放物線の設定")]
    [SerializeField] private float minUpForce = 5f;    // 上方向への最小の力
    [SerializeField] private float maxUpForce = 8f;    // 上方向への最大の力
    [SerializeField] private float minSideForce = -3f;  // 横方向への最小の力（左）
    [SerializeField] private float maxSideForce = 3f;   // 横方向への最大の力（右）

    [Header("消滅設定")]
    [SerializeField] private float lifeTime = 4f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // ランダムな初速（斜め上方向）を与えて放物線を描かせる
        float randomX = Random.Range(minSideForce, maxSideForce);
        float randomY = Random.Range(minUpForce, maxUpForce);

        rb.linearVelocity = new Vector2(randomX, randomY);

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // 飛んでいる方向にスプライトの向きを合わせる
        if (rb.linearVelocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // プレイヤーの属性コントローラーを取得
            PlayerAttributeController playerAttr = collision.GetComponent<PlayerAttributeController>();

            if (playerAttr != null)
            {
                // 直接メソッドを呼ぶ
                playerAttr.ChangePlayerColor(isWhiteInk);
            }

            Destroy(gameObject); // プレイヤーに当たったら消滅
        }
        else if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject); // 地面に当たったら消滅
        }
    }
}