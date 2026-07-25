using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("GameOverのルートパネル")]
    [SerializeField] private GameObject gameOverPanel;

    private void Start()
    {
        // ゲーム開始時は必ず非表示
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    //プレイヤーのHPが0になったら呼び出すメソッド
    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // 必要に応じてゲームの一時停止を行う場合
        // Time.timeScale = 0f;
    }

    //Retryボタンを押した時の処理
    public void OnRetryButton()
    {
        Time.timeScale = 1f;    // 一時停止していた場合を考慮して時間を戻す
        // 一時停止していた場合を考慮して時間を戻す
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //FINISHボタンを押した時の処理
    public void OnFinishButton()
    {
        Time.timeScale = 1f;    // 一時停止していた場合を考慮して時間を戻す

        SceneManager.LoadScene("StageSelect");
    }
}