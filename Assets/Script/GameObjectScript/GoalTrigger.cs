using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GoalTrigger : MonoBehaviour
{
    [Header("ステージ番号")]
    [SerializeField] private int stageNumber;

    private bool isPlayerInGoal = false;

    void Update()
    {
        if(isPlayerInGoal && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            ClearStage();
        }
    }

    private void ClearStage()
    {
        //Debug.Log($"ステージ{stageNumber}をクリアしました");

        //PlayerPrefsにステージがクリアを保存
        PlayerPrefs.SetInt($"Stage_{stageNumber}_Cleared", 1);
        PlayerPrefs.Save();

        if (StageClearUI.Instance != null)
        {
            StageClearUI.Instance.ShowClearUI();
        }
        else
        {
            //Debug.LogError("StageClearUI がシーン内に見つかりません！");
        }
    }

    //プレイヤーがゴール内にいるか判定
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInGoal = true;
            //Debug.Log("ゴール内にいます。Eキーでクリア");

            // 頭上マーク表示
            PlayerHeadPromptUI.Instance?.ShowPrompt();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInGoal = false;

            PlayerHeadPromptUI.Instance?.HidePrompt();
        }
    }
}
