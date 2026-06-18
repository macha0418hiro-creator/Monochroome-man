using UnityEditor.Tilemaps;
using UnityEngine;

public class MashroomMove : MonoBehaviour
{
    [Header("移動関連")]
    [SerializeField] private float moveSpeed = 2.0f;

    [Header("センサー(位置判定)")]
    [SerializeField] private Transform wallCheck;   //前に壁があるか判定するGmaeObject

    [Header("検知設定")]
    [SerializeField] private float checkDistance = 0.2f;        //壁のセンサーの距離
    [SerializeField] private float groundCheckDistance = 0.2f;  //崖のセンサーの距離
    [SerializeField] private string targetTag = "Ground";       //ブロック(地面・壁)のタグ
    
    private Rigidbody2D rb;
    private bool movingRight = false;    //左右の判定用

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //移動処理
        float currentSpeed = movingRight ? moveSpeed : -moveSpeed;
        rb.linearVelocity = new Vector2 (currentSpeed, rb.linearVelocity.y);

        //進行方向の判断
        Vector2 forwardDirection = movingRight ? Vector2.right : Vector2.left;

        //壁検知
        RaycastHit2D[] hitsWall = Physics2D.RaycastAll(wallCheck.position, forwardDirection, checkDistance);
        bool isWallDetected = false;

        // 触れたすべてのもの(自分やプレイヤー含む)の中からtargetTag(Ground)だけを探す
        foreach (var hit in hitsWall)
        {
            if (hit.collider != null && hit.collider.CompareTag(targetTag))
            {
                isWallDetected = true;
                break; //壁が見つかったら
            }
        }

        //崖検知
        Vector2 downDirection = forwardDirection + Vector2.down;
        RaycastHit2D[] hitsGround = Physics2D.RaycastAll(wallCheck.position, downDirection.normalized, groundCheckDistance);
        bool isGroundDetected = false;

        //壁検知と同じく
        foreach (var hit in hitsGround)
        {
            if (hit.collider != null && hit.collider.CompareTag(targetTag))
            {
                isGroundDetected = true;
                break; // 地面が見つからなかったら
            }
        }

        //壁or崖があったときに進行方向反転
        if (isWallDetected || !isGroundDetected)
        {
            Flip();
        }
    }

    //進行方向の反転
    private void Flip()
    {
        movingRight = !movingRight;

        //見た目の反転
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    // Unityの画面上にセンサーの線を視覚的に表示する（デバッグ用）
    private void OnDrawGizmos()
    {
        if (wallCheck != null)
        {
            Vector3 forwardDirection = movingRight ? Vector3.right : Vector3.left;

            // 壁センサー（赤色）
            Gizmos.color = Color.red;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + forwardDirection * checkDistance);

            // 崖センサー（青色）
            Gizmos.color = Color.blue;
            Vector3 downDirection = forwardDirection + Vector3.down;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + downDirection.normalized * groundCheckDistance);
        }
    }
}



