using UnityEngine;
using UnityEngine.InputSystem;

public class TreasureChest : MonoBehaviour
{
    [Header("この宝箱に入っているアイテムデータ")]
    [SerializeField] private ItemData itemInside;

    [Header("開いた後の宝箱の画像")]
    [SerializeField] private Sprite openedSprite;

    private SpriteRenderer spriteRenderer;
    private bool isPlayerNear = false;
    private bool isOpened = false;

    private void Start()
    {
        //自分のSpriteRendererコンポーネントを取得
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        //プレイヤーが近くにいて、未開封で、Eキーが押された時
        if (isPlayerNear && !isOpened && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            OpenChest();
        }
    }

    private void OpenChest()
    {
        isOpened = true;

        //画像を開いた状態(openedSprite)に差し替える
        if (openedSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = openedSprite;
        }

        //アイテム獲得処理
        if (itemInside != null)
        {
            Debug.Log($"宝箱を開けた！ アイテム『{itemInside.itemName}』（ID: {itemInside.itemID}）を手に入れた！");

            // 後々、属性解放やインベントリシステムと連携させる場所です
        }
        else
        {
            Debug.LogWarning("宝箱は空っぽでした…（ItemDataが未設定です）");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}