using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [Header("ポーズ画面UI")]
    [SerializeField] private GameObject pauseScreen;

    [Header("ステージ選択画面")]
    [SerializeField] private string stageSelectScenenName = "StageSelect";

    public static bool IsPaused { get; private set; } = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(pauseScreen != null)
        {
            pauseScreen.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (IsPaused)
            {
                ContinueGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    //ゲームを一時停止する処理
    public void PauseGame()
    {
        IsPaused = true;

        if(pauseScreen != null)
        {
            pauseScreen.SetActive(true);
        }

        Time.timeScale = 0f;    //時間の停止
    }

    //ゲームを再開する処理
    public void ContinueGame()
    {
        IsPaused = false;

        if(pauseScreen != null)
        {
            pauseScreen.SetActive(false);
        }

        Time.timeScale = 1f;    //時間の再開
    }

    public void OnClickFinishButton()
    {
        Time.timeScale = 1f;
        IsPaused = false;

        SceneNavigator.LoadTargetScene(stageSelectScenenName);
    }
}
