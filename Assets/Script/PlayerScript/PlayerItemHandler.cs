using UnityEngine;

public class PlayerItemHandler : MonoBehaviour
{
    private PlayerAttributeController attributeController;
    private PlayerHealth playerHealth;

    private void Start()
    {
        attributeController = GetComponent<PlayerAttributeController>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    //アイテム入手時の処理
    public void UseItem(ItemData item)
    {
        if (item == null) return;

        //Debug.Log($"アイテム【{item.itemName}】の効果を発動！");

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
                if (playerHealth != null)
                {
                    bool wasHealed = playerHealth.Heal(1);
                    if (!wasHealed)
                    {
                        return;
                    }
                }
                break;
        }
    }
}
