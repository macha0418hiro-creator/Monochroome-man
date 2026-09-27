using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerContoroller : MonoBehaviour
{
    [System.Serializable]
    public struct PlayerStatus
    {
        [Header("移動関連")]
        public float moveSpeed;
        public float jumpForce;

        [Header("接触判定関連")]
        public Transform groundCheck;   //足元に置くGameObject
        public float groundCheckRadius; //接触判定用の円
    }

    [SerializeField] private PlayerStatus status;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isGrounded; //地面に触れているかのフラグ
    private PlayerHealth playerHealth;
    private Animator animator;

    //直前に接地していた位置を記憶する変数
    private Vector3 lastGroundedPosition;
    public Vector3 LastGroundedPosition => lastGroundedPosition;
    private float lastGroundedDirection = 1f;   //最後に入力された方向（1:右,2:左）

    void Start() 
    {
        //Unityから情報を受け取る
        rb = GetComponent<Rigidbody2D>();
        playerHealth = GetComponent<PlayerHealth>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() 
    {
        if (PauseManager.IsPaused)
        {
            return;
        }

        //ダメージを受けて吹っ飛び中（IsKnockbackingがtrue）なら、以下の移動速度上書きを行わない
        if (playerHealth != null && playerHealth.IsKnockbacking)
        {
            return;
        }

        //毎フレーム地面との接触チェック
        CheckGroundedWithTag();

        //地面に足がついている間だけ、その座標を記憶し続ける
        if (isGrounded)
        {
            lastGroundedPosition = transform.position;
        }

        //移動速度処理
        rb.linearVelocity = new Vector2(moveInput.x * status.moveSpeed, rb.linearVelocity.y);

        //アニメーターへのパラメーター送信
        if (animator != null)
        {
            // Mathf.Abs で絶対値にすることで、左右どちらに動いてもプラスの速度として扱う
            animator.SetFloat("xSpeed", Mathf.Abs(rb.linearVelocity.x));
            animator.SetFloat("ySpeed", rb.linearVelocity.y);
            animator.SetBool("isGrounded", isGrounded);
        }

        //プレイヤーの向きを移動方向（左右）に反転させる処理
        ObjectPuller puller = GetComponent<ObjectPuller>();
        bool isCurrentlyPulliing = puller != null && puller.IsPulling;

        if (moveInput.x != 0 && !isCurrentlyPulliing)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveInput.x), 1, 1);
        }

        // 地面に足がついている間だけ、その座標と向きを記憶し続ける
        if (isGrounded)
        {
            lastGroundedPosition = transform.position;

            // 移動入力がある場合はその方向、入力がなければ現在の向きを記憶
            if (moveInput.x != 0)
            {
                lastGroundedDirection = Mathf.Sign(moveInput.x);
            }
            else
            {
                lastGroundedDirection = Mathf.Sign(transform.localScale.x);
            }
        }
    }

    private void CheckGroundedWithTag()
    {
        if (status.groundCheck == null) return;

        //足元の円の範囲にあるコライダーをすべて取得(配列)
        Collider2D[] colliders = Physics2D.OverlapCircleAll(status.groundCheck.position, status.groundCheckRadius);

        //最初は触れてない状態として扱う
        isGrounded = false;

        foreach (Collider2D collider in colliders) 
        {
            //自分自身のコライダー(プレイヤー)は無視する
            if (collider.gameObject == this.gameObject) continue;

            //Groundタグのあるものに触れたらジャンプ可能
            if (collider.CompareTag("Ground") || collider.CompareTag("PushableBlock"))
            {
                if(Physics2D.GetIgnoreLayerCollision(gameObject.layer, collider.gameObject.layer))
                {
                    continue;
                }

                isGrounded = true;
                break;
            }
        }
    }

    //転落時に呼ばれるリスポー(復帰)処理
    public void RespawnToLastGround()
    {
        if (rb != null)
        {
            //すり抜け対策で落下中の移動速度(慣性)をリセット
            rb.linearVelocity = Vector2.zero;
        }

        // 復帰位置の計算
        float backOffset = 1.0f; // 崖の内側に戻す距離
        Vector3 safePosition = lastGroundedPosition;

        // 「地面に接地していた時点の進行方向」の逆へ押し戻す
        safePosition.x -= lastGroundedDirection * backOffset;

        //記憶しておいた最後に接地していた位置へ移動し少し浮かせる(スタック防止)
        transform.position = safePosition + Vector3.up * 0.2f;
    }

    public void OnMove(InputAction.CallbackContext context) //移動処理
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context) //ジャンプ処理
    {
        if(context.started)
        {
            ObjectPuller puller = GetComponent<ObjectPuller>();
            bool isPulling = puller != null && puller.IsPulling;

            if (PauseManager.IsPaused)
            {
                return;
            }

            //地面にいるときのみジャンプ
            if (isGrounded && !isPulling)
            {
                rb.AddForce(Vector2.up * status.jumpForce, ForceMode2D.Impulse);

                SoundManager.Instance?.PlaySE(SoundManager.SEType.Jump);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        //判定できてるか見た目で確認
        if(status.groundCheck != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(status.groundCheck.position, status.groundCheckRadius);
        }
    }
}
