using UnityEngine;

public class HomingBullet : MonoBehaviour
{
    [System.Serializable]
    public struct VisualSetup
    {
        public Sprite bulletSprite; // 弾の画像
    }

    [Header("ボスの色に合わせた見た目の設定")]
    [SerializeField] private VisualSetup setupForWhiteBoss; //白用のSprite
    [SerializeField] private VisualSetup setupForBlackBoss; //黒用のSprite

    [Header("移動速度")]
    [SerializeField] private float speed = 5f;

    [Header("追尾の強さ(秒間の回転角度)")]
    [SerializeField] private float rotateSpeed = 90f;

    [Header("追尾時間")]
    [SerializeField] private float homingDuration = 1.5f;

    private Transform playerTransform;
    private Rigidbody2D rb;
    private float timer = 0f;
    private SpriteRenderer spriteRenderer;

    //弾の見た目をボスに合わせて適応
    public void SetUpBullet(bool isWhiteBoss)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        VisualSetup currentSetup = isWhiteBoss ? setupForWhiteBoss : setupForBlackBoss;

        if(spriteRenderer != null && currentSetup.bulletSprite != null)
        {
            spriteRenderer.sprite = currentSetup.bulletSprite;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if(player != null)
        {
            playerTransform = player.transform;
        }
    }

    //追尾処理
    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        if(playerTransform != null && timer < homingDuration)
        {
            //方向計算
            Vector2 direction = (Vector2)playerTransform.position - rb.position;
            direction.Normalize();

            //追尾用の回転
            float rotateAmount = Vector3.Cross(direction, transform.right).z;
            rb.angularVelocity = -rotateAmount * rotateSpeed;
        }
        else
        {
            rb.angularVelocity = 0f;
        }

        rb.linearVelocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }
}
