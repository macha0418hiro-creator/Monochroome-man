using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttributeController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public enum PlayerColor //属性(色)の定義
    {
        White,
        Black
    }

    //プレイヤーの色の初期値設定
    [Header("現在の属性")]
    [SerializeField] private PlayerColor currentColor = PlayerColor.White;

    [Header("立ち絵(Sprite)の設定")]
    [SerializeField] private SpriteRenderer spriteRenderer; //表示してる立ち絵
    [SerializeField] private Sprite whiteSprite;            //白用の立ち絵
    [SerializeField] private Sprite blackSprite;            //黒用の立ち絵

    [Header("連動するUI")]
    [SerializeField] private PlayerHpUI hpUI; // UIへの通知用

    private ObjectPuller objectPuller;
    private PlayerContoroller playerContoroller;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        objectPuller = GetComponent<ObjectPuller>();
        playerContoroller = GetComponent<PlayerContoroller>();
        SetColor(currentColor);
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseManager.IsPaused)
        {
            return;
        }

        //Fキーが押されたときに色を変更
        if(Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            //ブロックをつかんでる間は色変更禁止
            if(objectPuller != null && objectPuller.IsPulling)
            {
                Debug.Log("ブロックをつかんでる間は色を変えれません");
                return;
            }

            if(animator != null && !animator.GetBool("isGrounded"))
            {
                Debug.Log("空中では色を帰れません");
                return;
            }

            if (currentColor == PlayerColor.White)
            {
                SetColor(PlayerColor.Black);
            }
            else
            {
                SetColor(PlayerColor.White);
            }
        }
    }

    public void SetColor(PlayerColor newColor) //色を変更するシステム
    {
        currentColor = newColor;

        //属性(色)に合わせて立ち絵を変更
        if(currentColor == PlayerColor.White)
        {
            spriteRenderer.sprite = whiteSprite; //白の立ち絵を表示
        }
        else if(currentColor == PlayerColor.Black)
        {
            spriteRenderer.sprite = blackSprite; //黒の立ち絵を表示
        }

        //アニメーターに現在の色（白 = 0, 黒 = 1）を伝える
        if (animator != null)
        {
            int index = (currentColor == PlayerColor.White) ? 0 : 1;
            animator.SetFloat("colorIndex", index);
        }

        // UIに色が変わったことを伝える
        if (hpUI != null) hpUI.ChangeUiColor(currentColor);

        //当たり判定(レイヤー)を切り替える
        string layerName = "Player" + currentColor.ToString();
        gameObject.layer = LayerMask.NameToLayer(layerName);

        Debug.Log($"プレイヤーの立ち絵を切り替え、判定を【{layerName}】にしました。");
    }
}
