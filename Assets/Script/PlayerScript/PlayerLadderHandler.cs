using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLadderHandler : MonoBehaviour
{
    [Header("ハシゴ昇降スピード")]
    [SerializeField] private float climbSpeed = 5f;

    private Rigidbody2D rb;
    private Ladder currentLadder;
    private PlayerContoroller playerController;
    private bool isNearLadder = false;
    private bool isClimbing = false;
    private float defaultGravityScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerContoroller>();

        if (rb != null)
        {
            defaultGravityScale = rb.gravityScale;
        }
    }

    void Update()
    {
        //Eキーが押された時の処理
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (isClimbing)
            {
                //すでに昇降中ならハシゴから離れる
                StopClimbing();
            }
            else if (isNearLadder && currentLadder != null)
            {
                if (currentLadder.CanClimb(gameObject))
                {
                    //ハシゴの近くにいれば掴まる
                    StartClimbing();
                }
                else
                {
                    Debug.Log("属性が合わないため掴めません");
                }
            }
        }

        //ハシゴ昇降中の移動処理
        if (isClimbing)
        {
            float verticalInput = 0f;

            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            {
                verticalInput = 1f;
            }
            else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            {
                verticalInput = -1f;
            }

            //上下移動(Y軸速度の上書き)
            rb.linearVelocity = new Vector2(0f, verticalInput * climbSpeed);
        }
    }

    //ハシゴに掴まる処理
    private void StartClimbing()
    {
        isClimbing = true;

        if (playerController != null) playerController.enabled = false; //移動スクリプトを止める

        //重力を切って勝手に落下しないようにする
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        //プレイヤーのX座標をハシゴの真ん中にピッタリ合わせる(見た目のフィット感を向上)
        if (currentLadder != null)
        {
            transform.position = new Vector3(currentLadder.GetCenterX(transform.position), transform.position.y, transform.position.z);
        }

        Debug.Log("ハシゴに掴まった");
    }

    //ハシゴから離れる処理
    public void StopClimbing()
    {
        if (!isClimbing) return;

        isClimbing = false;

        //重力を元に戻す
        rb.gravityScale = defaultGravityScale;

        if (playerController != null) playerController.enabled = true;  //移動スクリプトを再開

        Debug.Log("ハシゴから離れた");
    }

    //Ladder.csから範囲内に入ったか通知を受けるメソッド
    public void SetNearLadder(Ladder ladder, bool isNear)
    {
        if (isNear)
        {
            currentLadder = ladder;
            isNearLadder = true;
        }
        else
        {
            isNearLadder = false;
            currentLadder = null;

            //ハシゴの範囲から出たら強制的に離脱
            if (isClimbing)
            {
                StopClimbing();
            }
        }
    }

    public bool IsClimbing => isClimbing;
}