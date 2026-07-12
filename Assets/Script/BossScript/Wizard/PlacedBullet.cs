using UnityEngine;
using System.Collections;

public class PlacedBullet : MonoBehaviour
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
    [SerializeField] private float speed = 8f;

    private bool isLaunched = false;    //設置済みか判定
    private Vector2 targetDirection;
    private SpriteRenderer spriteRenderer;

    //弾の見た目をボスに合わせて適応
    public void SetUpBullet(bool isWhiteBoss)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        VisualSetup currentSetup = isWhiteBoss ? setupForWhiteBoss : setupForBlackBoss;

        if (spriteRenderer != null && currentSetup.bulletSprite != null)
        {
            spriteRenderer.sprite = currentSetup.bulletSprite;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    //弾を配置する処理
    public void Launch(Vector3 playerPosition)
    {
        //プレイヤーの方向を記録
        targetDirection = (playerPosition - transform.position).normalized;

        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        isLaunched = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isLaunched)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
    }

    //画面外の弾を削除する処理
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }
}
