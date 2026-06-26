using System;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPuller : MonoBehaviour
{
    [Header("ブロックの引きずり設定")]
    [SerializeField] private float checkDistance = 0.8f; //ブロック検知距離(つかめる距離)
    [SerializeField] private LayerMask blockLayer;       //レイヤーを受け取る

    private GameObject grabbedBlock = null;              //つかんでるブロック
    private Rigidbody2D grabbedBlockRb = null;           //つかんでるブロックのRigidBody
    private Animator animator;
    private Rigidbody2D playerRb;

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
        //Eキーでブロックをつかんだり話したりする処理
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (IsPulling)
            {
                TryGrabBlock();
            }
            else
            {
                ReleaseBlock();
            }
        }

        if (animator != null)
        {
            animator.SetBool("IsPulling", IsPulling);
        }
    }

    private void FixedUpdate()
    {
        if(IsPulling && grabbedBlock != null)
        {
            grabbedBlockRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, grabbedBlockRb.linearVelocity.y);
        }
    }
}
