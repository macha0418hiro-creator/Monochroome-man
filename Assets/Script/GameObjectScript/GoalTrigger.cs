using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GoalTrigger : MonoBehaviour
{
    [Header("ステージ番号")]
    [SerializeField] private int stageNumber;

    private bool isPlayerInGoal = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayerInGoal && Keyboard.current != null && Keyboard.current.wKey.wasPressedThisFrame)
        {
            ClearStage();
        }
    }

    private void ClearStage()
    {
        Debug.Log($"ステージ{stageNumber}をクリアしました");

        //PlayerPrefsにステージがクリアを保存
        PlayerPrefs.SetInt($"Stage_{stageNumber}_Cleared", 1);
        PlayerPrefs.Save();

        if (StageClearUI.Instance != null)
        {
            StageClearUI.Instance.ShowClearUI();
        }
        else
        {
            Debug.LogError("StageClearUI がシーン内に見つかりません！");
        }
    }

    //プレイヤーがゴール内にいるか判定
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInGoal = true;
            Debug.Log("ゴール内にいます。Wキーでクリア");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInGoal = false;
        }
    }
}
