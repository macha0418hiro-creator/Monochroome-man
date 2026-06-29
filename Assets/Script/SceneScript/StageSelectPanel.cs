using Unity.VisualScripting;
using UnityEngine;

public class StageSelectPanel : MonoBehaviour
{
    [Header("チェックするステージ番号")]
    [SerializeField] private int stageNumber;

    [Header("クリア時に表示するUI")]
    [SerializeField] private GameObject clearIndicator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int isCleared = PlayerPrefs.GetInt($"Stage_{stageNumber}_Cleared", 0);

        if(isCleared == 1)
        {
            //クリア済みならUIを表示
            if(clearIndicator != null)
            {
                clearIndicator.SetActive(true);
            }
        }
        else
        {
            //未クリアなら非表示
            if(clearIndicator != null)
            {
                clearIndicator.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
