using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class StageSelectManager : MonoBehaviour
{
    [System.Serializable]
    public struct StageData
    {
        public string stageDisplayName;     // 画面に表示する名前
        public string sceneName;            // 実際のScene名

        [Header("このステージの白宝石のSprite")]
        public Sprite whiteGemColorSprite;
        public Sprite whiteGemGraySprite;

        [Header("このステージの黒宝石のSprite")]
        public Sprite blackGemColorSprite;
        public Sprite blackGemGraySprite;
    }

    [Header("UIの参照")]
    [SerializeField] private RectTransform stageContainer;
    [SerializeField] private TMP_Text stageNameText;
    [SerializeField] private Button leftArrowButton;
    [SerializeField] private Button rightArrowButton;
    [SerializeField] private List<Button> indicatorDots;

    [Header("宝石UIオブジェクト")]
    [SerializeField] private Image centerWhiteGemImage;
    [SerializeField] private Image centerBlackGemImage;

    //通常時とStage4時で座標を切り替える設定
    [Header("Stage4用：レイアウト調整")]
    [SerializeField] private Vector2 normalWhiteGemPos = new Vector2(-40f, -250f); // Stage1~3の白宝石座標
    [SerializeField] private Vector2 normalBlackGemPos = new Vector2(40f, -250f);  // Stage1~3の黒宝石座標
    [SerializeField] private Vector2 stage4WhiteGemPos = new Vector2(-90f, -250f); // Stage4の白宝石座標（左へ広げる）
    [SerializeField] private Vector2 stage4BlackGemPos = new Vector2(90f, -250f);  // Stage4の黒宝石座標（右へ広げる）

    //Stage4用のカウント表示UI（「x3」テキストや「/3」などの補足画像）
    [Header("Stage4用：宝石カウントUI")]
    [SerializeField] private TMP_Text whiteGemCountText; // 例: "1/3" や "x1"
    [SerializeField] private TMP_Text blackGemCountText;

    [Header("ステージデータの設定")]
    [SerializeField] private List<StageData> stages;
    [SerializeField] private float stageSpacing = 600f;
    [SerializeField] private float moveSpeed = 10f;

    private int currentStageIndex = 0;
    private Vector2 targetPosition;
    private bool canPlay = false;

    private void OnEnable()
    {
        UpdateStageUI();
    }

    void Start()
    {
        if (stageContainer != null)
        {
            targetPosition = stageContainer.anchoredPosition;
        }

        UpdateStageUI();

        if (leftArrowButton != null) leftArrowButton.onClick.AddListener(OnClickLeftArrow);
        if (rightArrowButton != null) rightArrowButton.onClick.AddListener(OnClickRightArrow);

        for (int i = 0; i < indicatorDots.Count; i++)
        {
            int index = i;
            if (indicatorDots[i] != null)
            {
                indicatorDots[i].onClick.AddListener(() => OnClickDot(index));
            }
        }
    }

    void Update()
    {
        if (!canPlay)
        {
            if (UnityEngine.InputSystem.Pointer.current != null && !UnityEngine.InputSystem.Pointer.current.press.isPressed)
            {
                canPlay = true;
            }
        }

        if (stageContainer != null)
        {
            stageContainer.anchoredPosition = Vector2.Lerp(
                stageContainer.anchoredPosition,
                targetPosition,
                Time.deltaTime * moveSpeed
            );
        }
    }

    public bool IsStageUnlocked(int index)
    {
        if (index == 0) return true;

        // ★ Stage 4 (index == 3) は Stage 1〜3 の宝石全6個が必要
        if (index == 3)
        {
            return HasAllGemsStage1To3();
        }

        int previousStageNumber = index;
        int isCleared = PlayerPrefs.GetInt($"Stage_{previousStageNumber}_Cleared", 0);

        return isCleared == 1;
    }

    private bool HasAllGemsStage1To3()
    {
        for (int stageNum = 1; stageNum <= 3; stageNum++)
        {
            int hasWhite = PlayerPrefs.GetInt($"Stage_{stageNum}_Gem_White_Collected", 0);
            int hasBlack = PlayerPrefs.GetInt($"Stage_{stageNum}_Gem_Black_Collected", 0);

            if (hasWhite == 0 || hasBlack == 0) return false;
        }
        return true;
    }

    private void OnClickLeftArrow()
    {
        if (currentStageIndex > 0) SelectStage(currentStageIndex - 1);
    }

    private void OnClickRightArrow()
    {
        if (currentStageIndex < stages.Count - 1) SelectStage(currentStageIndex + 1);
    }

    private void OnClickDot(int index)
    {
        if (index >= 0 && index < stages.Count) SelectStage(index);
    }

    private void SelectStage(int index)
    {
        currentStageIndex = index;

        float targetX = (-index * stageSpacing) - 300f;
        if (stageContainer != null)
        {
            targetPosition = new Vector2(targetX, stageContainer.anchoredPosition.y);
        }

        UpdateStageUI();
    }

    private void UpdateStageUI()
    {
        // 1. ステージ名テキストの更新
        if (stageNameText != null && currentStageIndex < stages.Count)
        {
            string displayName = stages[currentStageIndex].stageDisplayName;
            if (!IsStageUnlocked(currentStageIndex))
            {
                displayName += " <size=70%>(未解放)</size>";
            }
            stageNameText.text = displayName;
        }

        // 2. 矢印ボタンの切り替え
        if (leftArrowButton != null) leftArrowButton.interactable = (currentStageIndex > 0);
        if (rightArrowButton != null) rightArrowButton.interactable = (currentStageIndex < stages.Count - 1);

        // 3. ドットの切り替え
        for (int i = 0; i < indicatorDots.Count; i++)
        {
            if (indicatorDots[i] != null)
            {
                ColorBlock colors = indicatorDots[i].colors;
                colors.normalColor = (i == currentStageIndex) ? Color.white : new Color(0f, 0f, 0f, 1f);
                colors.selectedColor = colors.normalColor;
                colors.pressedColor = colors.normalColor;
                indicatorDots[i].colors = colors;
            }
        }

        // 4.宝石UIの切り替え処理
        UpdateGemDisplayUI();
    }

    //宝石表示の表示分岐メソッド
    private void UpdateGemDisplayUI()
    {
        RectTransform whiteRect = centerWhiteGemImage != null ? centerWhiteGemImage.rectTransform : null;
        RectTransform blackRect = centerBlackGemImage != null ? centerBlackGemImage.rectTransform : null;

        // --- Stage 4 (裏ボス) の場合：間隔を広げてカウント表示 ---
        if (currentStageIndex == 3)
        {
            // ★ 宝石の位置を広げる
            if (whiteRect != null) whiteRect.anchoredPosition = stage4WhiteGemPos;
            if (blackRect != null) blackRect.anchoredPosition = stage4BlackGemPos;

            int whiteCount = 0;
            int blackCount = 0;

            for (int i = 1; i <= 3; i++)
            {
                if (PlayerPrefs.GetInt($"Stage_{i}_Gem_White_Collected", 0) == 1) whiteCount++;
                if (PlayerPrefs.GetInt($"Stage_{i}_Gem_Black_Collected", 0) == 1) blackCount++;
            }

            if (stages.Count > 0)
            {
                if (centerWhiteGemImage != null)
                    centerWhiteGemImage.sprite = (whiteCount == 3) ? stages[0].whiteGemColorSprite : stages[0].whiteGemGraySprite;

                if (centerBlackGemImage != null)
                    centerBlackGemImage.sprite = (blackCount == 3) ? stages[0].blackGemColorSprite : stages[0].blackGemGraySprite;
            }

            if (whiteGemCountText != null)
            {
                whiteGemCountText.gameObject.SetActive(true);
                whiteGemCountText.text = $"{whiteCount}/3";
            }
            if (blackGemCountText != null)
            {
                blackGemCountText.gameObject.SetActive(true);
                blackGemCountText.text = $"{blackCount}/3";
            }
        }
        // --- Stage 1〜3 の通常表示の場合：元の間隔に戻して通常表示 ---
        else if (currentStageIndex < stages.Count)
        {
            // ★ 宝石の位置を通常に戻す
            if (whiteRect != null) whiteRect.anchoredPosition = normalWhiteGemPos;
            if (blackRect != null) blackRect.anchoredPosition = normalBlackGemPos;

            if (whiteGemCountText != null) whiteGemCountText.gameObject.SetActive(false);
            if (blackGemCountText != null) blackGemCountText.gameObject.SetActive(false);

            int stageNum = currentStageIndex + 1;
            StageData currentStage = stages[currentStageIndex];

            if (centerWhiteGemImage != null)
            {
                int hasWhiteGem = PlayerPrefs.GetInt($"Stage_{stageNum}_Gem_White_Collected", 0);
                centerWhiteGemImage.sprite = (hasWhiteGem == 1) ? currentStage.whiteGemColorSprite : currentStage.whiteGemGraySprite;
            }

            if (centerBlackGemImage != null)
            {
                int hasBlackGem = PlayerPrefs.GetInt($"Stage_{stageNum}_Gem_Black_Collected", 0);
                centerBlackGemImage.sprite = (hasBlackGem == 1) ? currentStage.blackGemColorSprite : currentStage.blackGemGraySprite;
            }
        }
    }

    public void OnClickCurrentStagePlay()
    {
        if (!canPlay) return;

        if (!IsStageUnlocked(currentStageIndex))
        {
            //Debug.LogWarning($"ステージ {currentStageIndex + 1} はまだ解放されていません！条件を満たしてください。");
            return;
        }

        if (currentStageIndex < stages.Count)
        {
            string targetScene = stages[currentStageIndex].sceneName;

            if (!string.IsNullOrEmpty(targetScene))
            {
                //Debug.Log($"[StageSelect] {targetScene} へ遷移します");
                SceneNavigator.LoadTargetScene(targetScene);
            }
        }
    }

    // デバッグ系
    [ContextMenu("Debug: すべてのステージを解放")]
    public void DebugUnlockAllStages()
    {
        for (int i = 1; i <= stages.Count; i++) PlayerPrefs.SetInt($"Stage_{i}_Cleared", 1);
        PlayerPrefs.Save();
        RefreshAllPanels();
    }

    [ContextMenu("Debug: Stage 1〜3 の全宝石を獲得済みにする")]
    public void DebugCollectAllGemsStage1To3()
    {
        for (int i = 1; i <= 3; i++)
        {
            PlayerPrefs.SetInt($"Stage_{i}_Gem_White_Collected", 1);
            PlayerPrefs.SetInt($"Stage_{i}_Gem_Black_Collected", 1);
        }
        PlayerPrefs.Save();
        RefreshAllPanels();
    }

    [ContextMenu("Debug: セーブデータを全削除")]
    public void DeleteSaveData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        RefreshAllPanels();
    }

    private void RefreshAllPanels()
    {
        StageSelectPanel[] allPanels = Object.FindObjectsByType<StageSelectPanel>(FindObjectsInactive.Include);
        foreach (var panel in allPanels) panel.UpdateItemUI();
        UpdateStageUI();
    }
}