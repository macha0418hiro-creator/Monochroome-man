using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectPanel : MonoBehaviour
{
    [Header("チェックするステージ番号")]
    [SerializeField] private int stageNumber;

    [Header("クリア時に表示するUI")]
    [SerializeField] private GameObject clearIndicator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
