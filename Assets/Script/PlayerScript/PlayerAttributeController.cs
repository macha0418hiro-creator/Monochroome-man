using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttributeController : MonoBehaviour
{
    //他のスクリプトに属性変更を知らせるイベントを定義
    public static event Action<GameObject> OnAttributeChanged;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public enum PlayerColor //属性(色)の定義
    {
        White,
        Black
    }

    //プレイヤーの色の初期値設定
    [Header("現在の属性")]
    [SerializeField] private PlayerColor currentColor = PlayerColor.White;

    //モノクロの筆を持っているか(Fキー切り替えができるか)
    [Header("能力解放フラグ")]
    [SerializeField] private bool canSwitchColor = false;
    public bool CanSwitchColor => canSwitchColor;

    [Header("立ち絵(Sprite)の設定")]
    [SerializeField] private SpriteRenderer spriteRenderer; //表示してる立ち絵
    [SerializeField] private Sprite whiteSprite;            //白用の立ち絵
    [SerializeField] private Sprite blackSprite;            //黒用の立ち絵

    [Header("連動するUI")]
    [SerializeField] private PlayerHpUI hpUI; //UIへの通知用

    [Header("色変更エフェクト")]
    [SerializeField] private GameObject explosionPrefab;

    [Header("色変更のクールダウン時間(秒)")]
    [SerializeField] private float colorChangeCooldown = 0.7f;

    private bool isColorChanging = false;   //連打防止用フラグ

    private ObjectPuller objectPuller;
    private PlayerContoroller playerContoroller;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        objectPuller = GetComponent<ObjectPuller>();
        playerContoroller = GetComponent<PlayerContoroller>();
        SetColorFloat(currentColor, spawnEffect: false);
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
            if (isColorChanging)
            {
                Debug.Log("色変更アニメーション中のため、変更できません！");
                return;
            }

            //モノクロの筆を持っていない場合はFキー切替不可
            if (!canSwitchColor)
            {
                Debug.Log("モノクロの筆を持っていないため、色を変更できません！");
                return;
            }

            //ブロックをつかんでる間は色変更禁止
            if (objectPuller != null && objectPuller.IsPulling)
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

    //色変更を可能にする処理
    public void UnlockColorSwitch()
    {
        canSwitchColor = true;
        Debug.Log("モノクロの筆を獲得！ Fキーで属性を自由に切り替えられるようになった！");
    }

    //外部から呼ばれる通常の色変更(エフェクト発生あり)
    public void SetColor(PlayerColor newColor)
    {
        SetColorFloat(newColor, spawnEffect: true);
    }

    //実処理用のメソッド
    private void SetColorFloat(PlayerColor newColor, bool spawnEffect)
    {
        currentColor = newColor;

        //エフェクトを出す場合のみ処理
        if (spawnEffect)
        {
            float colorIndex = (currentColor == PlayerColor.White) ? 0 : 1;
            SpawnExplosion(colorIndex);

            StartCoroutine(ColorChangeCooldownRoutine());
        }

        //属性(色)に合わせて立ち絵を変更
        if (currentColor == PlayerColor.White)
        {
            spriteRenderer.sprite = whiteSprite; //白の立ち絵を表示
        }
        else if (currentColor == PlayerColor.Black)
        {
            spriteRenderer.sprite = blackSprite; //黒の立ち絵を表示
        }

        //アニメーターに現在の色(白 = 0, 黒 = 1)を伝える
        if (animator != null)
        {
            float index = (currentColor == PlayerColor.White) ? 0 : 1;
            animator.SetFloat("colorIndex", index);
        }

        //UIに色が変わったことを伝える
        if (hpUI != null) hpUI.ChangeUiColor(currentColor);

        //当たり判定(レイヤー)を切り替える
        string layerName = "Player" + currentColor.ToString();
        gameObject.layer = LayerMask.NameToLayer(layerName);

        //レイヤー変更後、属性が変わったことを足場に通知
        OnAttributeChanged?.Invoke(gameObject);

        Debug.Log($"プレイヤーの立ち絵を切り替え、判定を【{layerName}】にしました。");
    }

    //色変更時に呼ぶ
    void SpawnExplosion(float colorIndex)
    {
        if (explosionPrefab == null) return;

        //transform(プレイヤー自身)を渡すことで、生成時に追従(子要素化)させる
        GameObject effect = Instantiate(explosionPrefab, transform.position, Quaternion.identity, transform);

        Animator effectAnimator = effect.GetComponent<Animator>();
        if (effectAnimator != null)
        {
            //colorIndexをAnimatorに伝える
            effectAnimator.SetFloat("colorIndex", colorIndex);
        }
    }

    //アニメーション再生中の連打をブロックするコルーチン
    private IEnumerator ColorChangeCooldownRoutine()
    {
        isColorChanging = true;
        yield return new WaitForSeconds(colorChangeCooldown);
        isColorChanging = false;
    }
}
