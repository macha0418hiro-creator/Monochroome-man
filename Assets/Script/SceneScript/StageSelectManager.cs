using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class StageSelectManager : MonoBehaviour
{
    [System.Serializable]
    public struct StageData
    {
        public string stageDisplayName; //画面に表示する名前
        public string sceneName;        //実際のScene名
    }

    [Header("UIの参照")]
    [SerializeField] private RectTransform stageContainer;    //StageContainerを入れる
    [SerializeField] private TMP_Text stageNameText;          //TextMeshProUGUIに対応
    [SerializeField] private Button leftArrowButton;          //LeftArrowButtonを入れる
    [SerializeField] private Button rightArrowButton;         //RightArrowButtonを入れる
    [SerializeField] private List<Toggle> indicatorDots;      //下のドットボタンを順番にすべて入れる

    [Header("ステージデータの設定")]
    [SerializeField] private List<StageData> stages;          
    [SerializeField] private float stageSpacing = 600f;       //ステージ画像1枚分(横幅＋余白)の移動距離
    [SerializeField] private float moveSpeed = 10f;           //ステージ画像が動くスピード

    private int currentStageIndex = 0;                        // 現在選択されているステージの番号
    private Vector2 targetPosition;                           // コンテナの目標座標

    void Start()
    {
        targetPosition = stageContainer.anchoredPosition;
        UpdateStageUI();

        if (leftArrowButton != null) leftArrowButton.onClick.AddListener(OnClickLeftArrow);
        if (rightArrowButton != null) rightArrowButton.onClick.AddListener(OnClickRightArrow);

        for (int i = 0; i < indicatorDots.Count; i++)
        {
            int index = i;
            if (indicatorDots[i] != null)
            {
                // トグルの値（ON/OFF）が変わった瞬間を検知する
                indicatorDots[i].onValueChanged.AddListener((isOn) => {
                    // クリックされてONになった瞬間だけステージを切り替える
                    if (isOn) OnClickDot(index);
                });
            }
        }
    }

    void Update()
    {
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
        float targetX = -index * stageSpacing;
        targetPosition = new Vector2(targetX, stageContainer.anchoredPosition.y);

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
                //現在のステージ番号と同じトグルだけをON(選択中)にする
                indicatorDots[i].SetIsOnWithoutNotify(i == currentStageIndex);
            }
        }
    }

    //中央にあるステージ画像をタップした時に呼ばれる関数
    //インスペクターの各ステージボタン(StageButton_1など)の OnClick()にこの関数を登録します
    public void OnClickCurrentStagePlay()
    {
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
}