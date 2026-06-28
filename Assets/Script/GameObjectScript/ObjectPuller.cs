using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectPuller : MonoBehaviour
{
    [Header("ブロックの引きずり設定")]
    [SerializeField] private float checkDistance = 0.8f; //ブロック検知距離(つかめる距離)
    [SerializeField] private LayerMask blockLayers;       //レイヤーを受け取る

    private GameObject grabbedBlock = null;              //つかんでるブロック
    private Rigidbody2D grabbedBlockRb = null;           //つかんでるブロックのRigidBody
    private Animator animator;
    private Rigidbody2D playerRb;
    private FixedJoint2D attachJoint = null;             //関節コンポーネント保存(blockの物理演算を残したまま運ぶため)

    //他のScriptやAnimaterから状態を受け取る
    public bool IsPulling {  get; private set; } = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        playerRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (animator != null)
        {
            animator.SetBool("IsPulling", IsPulling);
        }
    }

    public void OnGrab(InputAction.CallbackContext context)
    {

        if (context.started)
        {
            if (!IsPulling)
            {
                TryGrabBlock();
            }
            else
            {
                if(playerRb != null && Mathf.Abs(playerRb.linearVelocity.x) > 0.01f)
                {
                    Debug.Log("移動中は手を離せません");
                    return;
                }

                ReleaseBlock();
            }
        }
    }

    //ブロックをつかむ処理
    private void TryGrabBlock()
    {
        //プレイヤーの位置に合わせてブロックのレイヤーを動かす
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, checkDistance, blockLayers);

        foreach(RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject == this.gameObject) continue;

            if(hit.collider != null && hit.collider.CompareTag("PushableBlock"))
            {
                string playerLayerName = LayerMask.LayerToName(gameObject.layer);
                string blockLayerName = LayerMask.LayerToName(hit.collider.gameObject.layer);

                //プレイヤーのレイヤー名をWhiteやBlackに変換する処理
                string cleanPlayerColor = playerLayerName.Replace("Player", "");

                if (cleanPlayerColor != blockLayerName)
                {
                    grabbedBlock = hit.collider.gameObject;
                    grabbedBlockRb = grabbedBlock.GetComponent<Rigidbody2D>();
                    IsPulling = true;

                    FixPlayerPosition(grabbedBlock, direction);

                    //つかんでる間、摩擦を0にする
                    if(grabbedBlockRb != null)
                    {
                        //つかんでる間はx軸を動くようにする
                        grabbedBlockRb.constraints = RigidbodyConstraints2D.FreezeRotation;

                        //FixedJoint2Dにより、物理演算をたもったままブロックをつかむ
                        attachJoint = gameObject.AddComponent<FixedJoint2D>();
                        attachJoint.connectedBody = grabbedBlockRb;
                    }

                    Debug.Log($"{grabbedBlock.name}をつかんだ");
                    return;
                }
                else
                {
                    Debug.Log("同じ色なのでつかめません");
                    return;
                }
            } 
        }  
    }

    //ブロックをつかんだ際に、ブロックとプレイヤーを引っ付かせる処理
    private void FixPlayerPosition(GameObject block, Vector2 direction)
    {
        float playerHalfWidth = GetComponent<Collider2D>().bounds.extents.x;
        float blockHalfWidth = block.GetComponent<Collider2D>().bounds.extents.x;
        float contactDistance = playerHalfWidth + blockHalfWidth + 0.13f;

        float targetX;
        if(direction == Vector2.right)
        {
            targetX = block.transform.position.x - contactDistance;
        }
        else
        {
            targetX= block.transform.position.x + contactDistance;
        }

        transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, direction * checkDistance);
    }

    //ブロックを話す処理
    public void ReleaseBlock()
    {
        if(attachJoint != null)
        {
            Destroy(attachJoint);
            attachJoint = null;
        }

        if(grabbedBlockRb != null)
        {
            grabbedBlockRb.bodyType = RigidbodyType2D.Dynamic;
            //ブロックを離したとき滑らないようx軸の力を0に
            grabbedBlockRb.linearVelocity = Vector2.zero;
            grabbedBlockRb.angularVelocity = 0f;

            grabbedBlockRb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }

        //少し離れた位置にプレイヤーを移す(ブロックにめり込まないよう)
        if(grabbedBlock != null)
        {
            Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            FixPlayerPosition(grabbedBlock, direction);
        }

        grabbedBlock = null;
        grabbedBlockRb = null;
        IsPulling = false;
        Debug.Log("ブロックを離した");
    }
}
