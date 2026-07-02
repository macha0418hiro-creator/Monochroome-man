using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class StageSelectManager : MonoBehaviour
{
    [System.Serializable]
    public struct StageData
    {
        public string stageDisplayName;     //画面に表示する名前
        public string sceneName;            //実際のScene名

        [Header("このステージの白宝石のSprite")]
        public Sprite whiteGemColorSprite;
        public Sprite whiteGemGraySprite;

        [Header("このステージの黒宝石のSprite")]
        public Sprite blackGemColorSprite;
        public Sprite blackGemGraySprite;
    }

    [Header("UIの参照")]
    [SerializeField] private RectTransform stageContainer;    //StageContainerを入れる
    [SerializeField] private TMP_Text stageNameText;          //TextMeshProUGUIに対応
    [SerializeField] private Button leftArrowButton;          //LeftArrowButtonを入れる
    [SerializeField] private Button rightArrowButton;         //RightArrowButtonを入れる
    [SerializeField] private List<Button> indicatorDots;      //下のドットボタンを順番にすべて入れる

    [Header("宝石UIオブジェクト")]
    [SerializeField] private Image centerWhiteGemImage;
    [SerializeField] private Image centerBlackGemImage;

    [Header("ステージデータの設定")]
    [SerializeField] private List<StageData> stages;          
    [SerializeField] private float stageSpacing = 600f;       //ステージ画像1枚分(横幅＋余白)の移動距離
    [SerializeField] private float moveSpeed = 10f;           //ステージ画像が動くスピード

    private int currentStageIndex = 0;                        //現在選択されているステージの番号
    private Vector2 targetPosition;                           //目標座標
    private bool canPlay = false;                             //画面遷移暴発対策

    //シーンが読み込まれた際UIを最新状態にする
    private void OnEnable()
    {
        UpdateStageUI();
    }

    void Start()
    {
        //初期位置を記憶
        if(stageContainer != null)
        {
            targetPosition = stageContainer.anchoredPosition;
        }

        UpdateStageUI();

        //矢印ボタンが押されたときの挙動を登録
        if (leftArrowButton != null) leftArrowButton.onClick.AddListener(OnClickLeftArrow);
        if (rightArrowButton != null) rightArrowButton.onClick.AddListener(OnClickRightArrow);

        for (int i = 0; i < indicatorDots.Count; i++)
        {
            int index = i;
            if (indicatorDots[i] != null)
            {
                // トグルの値（ON/OFF）が変わった瞬間を検知する
                indicatorDots[i].onClick.AddListener(() => OnClickDot(index));
            }
        }
    }

    void Update()
    {
        //マウスから指が離されるまで、動かせない処理
        if (!canPlay)
        {
            if(UnityEngine.InputSystem.Pointer.current != null && !UnityEngine.InputSystem.Pointer.current.press.isPressed)
            {
                canPlay = true;
            }
        }

        //目標座標に向かってコンテナ(ステージ画像)を移動させる
        if (stageContainer != null)
        {
            stageContainer.anchoredPosition = Vector2.Lerp(
                stageContainer.anchoredPosition,
                targetPosition,
                Time.deltaTime * moveSpeed
            );
        }
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

        //スライドの計算(左右どちらに動くか)
        float targetX = (-index * stageSpacing) - 300f;
        if(stageContainer != null)
        {
            targetPosition = new Vector2(targetX, stageContainer.anchoredPosition.y);
        }

        UpdateStageUI();
    }

    private void UpdateStageUI()
    {
        //1.ステージ名テキストの更新
        if (stageNameText != null && currentStageIndex < stages.Count)
        {
            stageNameText.text = stages[currentStageIndex].stageDisplayName;
        }

        //2.矢印ボタンの有効・無効切り替え
        if (leftArrowButton != null) leftArrowButton.interactable = (currentStageIndex > 0);
        if (rightArrowButton != null) rightArrowButton.interactable = (currentStageIndex < stages.Count - 1);

        //3.下のドット(ラジオボタン)の色を切り替える
        for (int i = 0; i < indicatorDots.Count; i++)
        {
            if (indicatorDots[i] != null)
            {
                ColorBlock colors = indicatorDots[i].colors;
                //選択中なら白、それ以外は灰色
                colors.normalColor = (i == currentStageIndex) ? Color.white : new Color(0f, 0f, 0f, 1f);
                colors.selectedColor = colors.normalColor;
                colors.pressedColor = colors.normalColor;
                indicatorDots[i].colors = colors;
            }
        }

        //宝石の取得状況の切り替え
        if (currentStageIndex < stages.Count)
        {
            int stageNum = currentStageIndex + 1;
            StageData currentStage = stages[currentStageIndex];

            //白宝石
            if (centerWhiteGemImage != null)
            {
                int hasWhiteGem = PlayerPrefs.GetInt($"Stage_{stageNum}_Gem_White_Collected", 0);
                centerWhiteGemImage.sprite = (hasWhiteGem == 1) ? currentStage.whiteGemColorSprite : currentStage.whiteGemGraySprite;
            }

            //黒宝石
            if (centerBlackGemImage != null)
            {
                int hasBlackGem = PlayerPrefs.GetInt($"Stage_{stageNum}_Gem_Black_Collected", 0);
                centerBlackGemImage.sprite = (hasBlackGem == 1) ? currentStage.blackGemColorSprite : currentStage.blackGemGraySprite;
            }
        }
    }

    

    //中央にあるステージ画像を押したときステージ遷移
    public void OnClickCurrentStagePlay()
    {
        //クリックが画面遷移後残らないように
        if (!canPlay) return;

        if (currentStageIndex < stages.Count)
        {
            string targetScene = stages[currentStageIndex].sceneName;

            if (!string.IsNullOrEmpty(targetScene))
            {
                Debug.Log($"[StageSelect] {targetScene} へ遷移します（ローディング画面経由）");

                SceneNavigator.LoadTargetScene(targetScene);
            }
            else
            {
                Debug.LogWarning("遷移先のScene名が設定されていません！");
            }
        }
    }

    [ContextMenu("Delete Save Data")]
    public void DeleteSaveData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("セーブデータを初期化しました");

        StageSelectPanel[] allPanels = Object.FindObjectsByType<StageSelectPanel>(FindObjectsInactive.Include);
        foreach(var panel in allPanels)
        {
            panel.UpdateItemUI();
        }

        UpdateStageUI();
    }
}