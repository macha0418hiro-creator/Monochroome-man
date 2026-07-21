using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "GameData/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("アイテム識別ID")]
    public string itemID;

    [Header("表示名")]
    public string itemName;

    [Header("アイコン画像")]
    public Sprite itemIcon;

    [Header("アイテム説明")]
    [TextArea] public string itemText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
