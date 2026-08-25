using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HomingBulletBoss : MonoBehaviour
{
    [Header("移動・追尾設定")]
    [SerializeField] private float speed = 6.0f;           // 移動速度
    [SerializeField] private float homingDuration = 3.5f;  // 追尾する時間（秒）
    [SerializeField] private float totalLifeTime = 6.0f;   // 弾の寿命（秒）

    [Header("爆発設定")]
    [SerializeField] private GameObject explosionPrefab;   // 爆発エフェクトのプレハブ
    [SerializeField] private float explosionLifeTime = 1.0f;

    private Transform playerTransform;
    private Rigidbody2D rb;
    private float timer = 0f;
    private Vector2 moveDirection;
    private bool isExploded = false;                       // 二重爆発防止用フラグ

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // プレイヤーの検索
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        // 注意: Destroy(gameObject, totalLifeTime) は爆発処理を挟むため削除し、Updateで時間管理します
    }

    private void Update()
    {
        if (isExploded) return;

        timer += Time.deltaTime;

        // 一定時間（寿命）経過したら爆発して消滅
        if (timer >= totalLifeTime)
        {
            Explode();
            return;
        }

        // 追尾時間内かつプレイヤーが存在する場合、進行方向を更新
        if (timer <= homingDuration && playerTransform != null)
        {
            moveDirection = (playerTransform.position - transform.position).normalized;

            // 弾の見た目を進行方向に向ける
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        // 追尾時間を過ぎたら最後に計算された moveDirection のまま直進
    }

    private void FixedUpdate()
    {
        if (!isExploded && rb != null)
        {
            rb.linearVelocity = moveDirection * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isExploded) return;

        // プレイヤーまたは地面に当たったら爆発
        if (collision.CompareTag("Player") || collision.CompareTag("Ground"))
        {
            // ※もしプレイヤーに直接ダメージを与える処理があればここで行います
            Explode();
        }
    }

    // 爆発処理
    private void Explode()
    {
        if (isExploded) return;
        isExploded = true;

        // 爆発エフェクトが設定されていれば生成
        if (explosionPrefab != null)
        {
            SoundManager.Instance?.PlaySE(SoundManager.SEType.Explosion);

            GameObject explosionInstance = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            Destroy(explosionInstance, explosionLifeTime);
        }

        // 弾自体を削除
        Destroy(gameObject);
    }
}