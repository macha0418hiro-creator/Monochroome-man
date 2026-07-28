using UnityEngine;

public class PlayerItemHandler : MonoBehaviour
{
    private PlayerAttributeController attributeController;
    private PlayerHealth playerHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        attributeController = GetComponent<PlayerAttributeController>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //アイテム入手時の処理
    public void UseItem(ItemData item)
    {
        if (item == null) return;

        Debug.Log($"アイテム【{item.itemName}】の効果を発動！");

        switch (item.itemType)
        {
            case ItemType.PaintBlack:
                if(attributeController != null)
                {
                    attributeController.SetColor(PlayerAttributeController.PlayerColor.Black);
                }
                break;

            case ItemType.PaintWhite:
                if(attributeController != null)
                {
                    attributeController.SetColor(PlayerAttributeController.PlayerColor.White);
                }
                break;

            case ItemType.MonochromeBrush:
                if (attributeController != null)
                {
                    attributeController.UnlockColorSwitch();
                }
                break;

            case ItemType.HealHeart:
                /* 
                if (playerHealth != null)
                {
                    playerHealth.Heal(1);
                }
                */
                break;
        }
    }
}
