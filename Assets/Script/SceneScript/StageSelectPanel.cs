using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectPanel : MonoBehaviour
{
    [Header("チェックするステージ番号")]
    [SerializeField] private int stageNumber;

    [Header("クリア時に表示するUI")]
    [SerializeField] private GameObject clearIndicator;

    private void OnEnable()
    {
        UpdateItemUI();
    }

    public void UpdateItemUI()
    {
        if (clearIndicator != null)
        {
            int isCleared = PlayerPrefs.GetInt($"Stage_{stageNumber}_Cleared", 0);
            clearIndicator.SetActive(isCleared == 1);
        }
    }
}
