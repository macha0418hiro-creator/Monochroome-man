using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class ItemGetUI : MonoBehaviour
{
    public static ItemGetUI Instance;   //広域変数みたいなもの

    [SerializeField] private GameObject popupPanel;
    [SerializeField] private Image itemIconImage;
    [SerializeField] private TextMeshProUGUI itemTitleText;         //アイテム名
    [SerializeField] private TextMeshProUGUI itemDescriptionText;   //アイテム説明

    private ItemData currentItem;
    private GameObject playerObj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        Instance = this;
        popupPanel.SetActive(false);
    }

    //アイテム入手画面を表示して時間停止する処理
    public void ShowItemGetPopup(ItemData item, GameObject player)
    {
        currentItem = item;
        playerObj = player;

        itemIconImage.sprite = item.itemIcon;
        itemTitleText.text = $"{item.itemName} をGet!";
        itemDescriptionText.text = item.itemText;

        popupPanel.SetActive(true);
        Time.timeScale = 0f;
        PauseManager.IsPaused = true;
    }

    //OKボタン押下後にアイテム処理をする
    public void OnClickOKButton()
    {
        popupPanel.SetActive(false);
        Time.timeScale = 1f;
        PauseManager.IsPaused = false;

        if (playerObj != null)
        {
            PlayerItemHandler itemHandler = playerObj.GetComponentInParent<PlayerItemHandler>();
            if(itemHandler != null)
            {
                itemHandler.UseItem(currentItem);
            }
        }
    }
}
