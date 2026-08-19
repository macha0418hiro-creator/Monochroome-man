using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttributeController : MonoBehaviour
{
    //他のスクリプトに属性変更を知らせるイベントを定義
    public static event Action<GameObject> OnAttributeChanged;

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
    private bool isColorLocked = false;     // ★追加：オーラ等による色変更封印フラグ

    private ObjectPuller objectPuller;
    private PlayerContoroller playerContoroller;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        objectPuller = GetComponent<ObjectPuller>();
        playerContoroller = GetComponent<PlayerContoroller>();

        //シーン読み込み時にデータを引き継ぐ
        if (PlayerDataManager.Instance != null)
        {
            currentColor = PlayerDataManager.Instance.CurrentColor;
            canSwitchColor = PlayerDataManager.Instance.CanSwitchColor;
        }

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
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            // ★追加：オーラによってロックされている場合は変更不可
            if (isColorLocked)
            {
                Debug.Log("オーラによって色変更が封印されています！");
                return;
            }

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

            if (animator != null && !animator.GetBool("isGrounded"))
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

        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.CanSwitchColor = true;
        }

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

        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.CurrentColor = currentColor;
        }

        if (spawnEffect)
        {
            float colorIndex = (currentColor == PlayerColor.White) ? 0 : 1;
            SpawnExplosion(colorIndex);

            StartCoroutine(ColorChangeCooldownRoutine());

            SoundManager.Instance?.PlaySE(SoundManager.SEType.ColorChange);
        }

        if (currentColor == PlayerColor.White)
        {
            spriteRenderer.sprite = whiteSprite;
        }
        else if (currentColor == PlayerColor.Black)
        {
            spriteRenderer.sprite = blackSprite;
        }

        if (animator != null)
        {
            float index = (currentColor == PlayerColor.White) ? 0 : 1;
            animator.SetFloat("colorIndex", index);
        }

        if (hpUI != null) hpUI.ChangeUiColor(currentColor);

        string layerName = "Player" + currentColor.ToString();
        gameObject.layer = LayerMask.NameToLayer(layerName);

        OnAttributeChanged?.Invoke(gameObject);

        Debug.Log($"プレイヤーの立ち絵を切り替え、判定を【{layerName}】にしました。");
    }

    void SpawnExplosion(float colorIndex)
    {
        if (explosionPrefab == null) return;

        GameObject effect = Instantiate(explosionPrefab, transform.position, Quaternion.identity, transform);

        Animator effectAnimator = effect.GetComponent<Animator>();
        if (effectAnimator != null)
        {
            effectAnimator.SetFloat("colorIndex", colorIndex);
        }
    }

    // インク弾やギミックなど外部から強制的に色を変えられる処理
    public void ChangePlayerColor(bool toWhite)
    {
        PlayerColor targetColor = toWhite ? PlayerColor.White : PlayerColor.Black;

        if (currentColor == targetColor) return;

        SetColor(targetColor);

        Debug.Log($"[PlayerAttributeController] インクにより強制的に【{targetColor}】に変更されました！");
    }

    // ★追加：オーラ用（指定色に強制変更し、一定時間Fキー等をロックする）
    public void ApplyAuraColorLock(bool toWhite, float lockDuration)
    {
        // 1. 強制的に色変更
        ChangePlayerColor(toWhite);

        // 2. ロック処理を開始
        StartCoroutine(LockColorRoutine(lockDuration));
    }

    // ★追加：属性ロック管理コルーチン
    private IEnumerator LockColorRoutine(float duration)
    {
        isColorLocked = true;
        Debug.Log($"【属性ロック】{duration}秒間、属性の切り替えが不可になりました！");

        yield return new WaitForSeconds(duration);

        isColorLocked = false;
        Debug.Log("【属性ロック解除】属性の切り替えが可能になりました。");
    }

    private IEnumerator ColorChangeCooldownRoutine()
    {
        isColorChanging = true;
        yield return new WaitForSeconds(colorChangeCooldown);
        isColorChanging = false;
    }
}