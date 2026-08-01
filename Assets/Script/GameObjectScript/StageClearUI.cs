using UnityEngine;
using UnityEngine.SceneManagement;

public class StageClearUI : MonoBehaviour
{
    public static StageClearUI Instance;

    [Header("クリア画面のパネル")]
    [SerializeField] private GameObject clearPanel;

    [Header("遷移先のステージセレクトシーン名")]
    [SerializeField] private string selectSceneName = "StageSelect";

    private void Awake()
    {
        Instance = this;
        if (clearPanel != null)
        {
            clearPanel.SetActive(false);
        }
    }

    //クリア画面を表示する(GoalTriggerから呼ばれる)
    public void ShowClearUI()
    {
        if (clearPanel != null)
        {
            clearPanel.SetActive(true);
        }

        // 時間停止＆操作無効化
        Time.timeScale = 0f;
        PauseManager.IsPaused = true;
    }

    //ステージセレクト画面へ遷移する処理
    public void OnClickStageSelectButton()
    {
        // 時間停止とポーズ状態を元に戻す
        Time.timeScale = 1f;
        PauseManager.IsPaused = false;

        // シーン遷移
        SceneManager.LoadScene(selectSceneName);
    }
}