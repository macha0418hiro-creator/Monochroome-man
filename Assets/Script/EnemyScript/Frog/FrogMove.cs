using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FrogMove : MonoBehaviour
{
    [Header("ジャンプ力設定")]
    [SerializeField] private float jumpForwardForce = 5.0f;     //前方への力
    [SerializeField] private float jumpUpwardForce = 7.0f;      //上方への力

    [Header("行動間隔")]
    [SerializeField] private float jumpInterval = 3.0f;         //ジャンプの間隔
    [SerializeField] private float savePowerDuration = 0.25f;   //ジャンプのため時間

    [Header("接触判定関連")]
    [SerializeField] private Transform groundCheck;             //足元のGameObject
    [SerializeField] private float checkRadius = 0.2f;          //判定用の円の半径
    [SerializeField] private string targetTag = "Ground";       //地面のタグ

    private Rigidbody2D rb;
    private Animator animator;
    private Transform playerTransform;          //プレイヤーの座標を記録
    private FrogAttack frogAttack;              //攻撃処理を呼ぶため
    private bool isGrounded = true;             //地面に着いてるか
    private bool isMovingRight = false;         //向いてる方向
    private bool isSaveingPower = false;        //踏ん張り中かの判定
    private bool wasGroundedLastFrame = true;   //着地検知
    private float jumpTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        frogAttack = GetComponent<FrogAttack>();
        jumpTimer = jumpInterval;

        //Playerタグのついたオブジェクトから座標を取得
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player != null )
        {
            playerTransform = player.transform;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckGround();

        //地面に着地した瞬間は踏らせる
        if(!wasGroundedLastFrame && isGrounded && !isSaveingPower)
        {
            StartCoroutine(LandRoutine());
        }
        wasGroundedLastFrame = isGrounded;

        if(animator != null)
        {
            animator.SetBool("isGrounded", isGrounded);
            animator.SetBool("isJumping", !isGrounded && rb.linearVelocity.y > 0.1f);
        }

        //攻撃処理中は移動処理はしない
        if(frogAttack != null && frogAttack.IsAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            if (CheckPlayerInRange())
            {
                frogAttack.TryAttack();
                return;
            }

            jumpTimer -= Time.deltaTime;
            if(jumpTimer <= 0f)
            {
                TargetPlayerDirection();

                if (!isSaveingPower)
                {
                    StartCoroutine(JumpRoutine());
                }

                jumpTimer = jumpInterval;
            }
        }
    }

    //ジャンプ前の踏ん張りモーション用のデータをUnityに送る
    private IEnumerator JumpRoutine()
    {
        isSaveingPower = true;
        if (animator != null) animator.SetBool("isSavingPower", true);

        yield return new WaitForSeconds(savePowerDuration);

        if (animator != null) animator.SetBool("isSavingPower", false);
        isSaveingPower = false;

        Jump();

        yield return new WaitForSeconds(savePowerDuration);
    }

    private IEnumerator LandRoutine()
    {
        if (animator != null) animator.SetBool("isSavingPower", true);

        yield return new WaitForSeconds(0.15f);

        if (animator != null) animator.SetBool("isSavingPower", false);
    }

    //射程距離内にプレイヤーがいるか確認
    private bool CheckPlayerInRange()
    {
        if(playerTransform == null || frogAttack == null) return false;
        float distance = Vector2.Distance(transform.position, playerTransform.position);
        return distance <= frogAttack.AttackRange;
    }

    public void ResetJumpTimer()
    {
        jumpTimer = jumpInterval;
    }

    private void Jump()
    {
        float directionX = isMovingRight ? jumpForwardForce : -jumpForwardForce;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(directionX, jumpUpwardForce), ForceMode2D.Impulse);
    }

    //プレイヤーの方向に向きを変える
    public void TargetPlayerDirection()
    {
        if(playerTransform == null) return;

        //プレイヤーのx座標-カエルのx座標
        float directionToPlayer = playerTransform.position.x - transform.position.x;

        if(directionToPlayer > 0 && !isMovingRight)
        {
            SetFacing(true);
        }
        else if(directionToPlayer < 0 && isMovingRight)
        {
            SetFacing(false);
        }
    }

    private void SetFacing(bool faceRight)
    {
        isMovingRight = faceRight;

        Vector3 localScale = transform.localScale;

        //Unityの画像サイズ(x)を逆にすることで画像を反転
        localScale.x = faceRight ? -1f : 1f;
        transform.localScale = localScale;
    }

    //地面の判定
    private void CheckGround()
    {
        if(groundCheck == null) return;

        // 足元の円の範囲にあるコライダーをすべて取得
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, checkRadius);

        //最初は着地していない状態として扱う
        isGrounded = false;

        foreach(Collider2D col in colliders)
        {
            //自身のコライダーは無視する
            if(col.gameObject == this.gameObject) continue;

            //地面に触れたとき
            if (col.CompareTag(targetTag))
            {
                //すり抜けないレイヤーの時
                if(col.gameObject.layer != this.gameObject.layer)
                {
                    isGrounded = true;  //ジャンプ許可
                    break;
                }
            }
        }
    }

    //判定を目視で確認可能に
    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }

        if (frogAttack != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, frogAttack.AttackRange);
        }
    }

    //見た目の反転
    public void Flip()
    {
        isMovingRight = !isMovingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}

