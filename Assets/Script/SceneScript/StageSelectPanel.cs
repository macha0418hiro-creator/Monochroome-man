using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectPanel : MonoBehaviour
{
    [Header("チェックするステージ番号")]
    [SerializeField] private int stageNumber;

    [Header("クリア時に表示するUI")]
    [SerializeField] private GameObject clearIndicator;

    [Header("宝石UIのImageコンポーネント")]
    [SerializeField] private Image whiteGemImage;
    [SerializeField] private Image blackGemImage;

    [Header("白宝石Sprite")]
    [SerializeField] private Sprite whiteGemColorSprite;
    [SerializeField] private Sprite whiteGemGraySprite;

    [Header("黒宝石Sprite")]
    [SerializeField] private Sprite blackGemColorSprite;
    [SerializeField] private Sprite blackGemGraySprite;

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

        // --- 白の宝石の判定とSprite差し替え ---
        if (whiteGemImage != null)
        {
            int hasWhiteGem = PlayerPrefs.GetInt($"Stage_{stageNumber}_Gem_White_Collected", 0);
            whiteGemImage.sprite = (hasWhiteGem == 1) ? whiteGemColorSprite : whiteGemGraySprite;
        }

        // --- 黒の宝石の判定とSprite差し替え ---
        if (blackGemImage != null)
        {
            int hasBlackGem = PlayerPrefs.GetInt($"Stage_{stageNumber}_Gem_Black_Collected", 0);
            blackGemImage.sprite = (hasBlackGem == 1) ? blackGemColorSprite : blackGemGraySprite;
        }
    }
}
