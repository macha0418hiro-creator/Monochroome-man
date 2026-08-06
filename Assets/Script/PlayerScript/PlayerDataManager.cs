using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    [Header("引き継ぐプレイヤーデータ")]
    public PlayerAttributeController.PlayerColor CurrentColor { get; set; } = PlayerAttributeController.PlayerColor.White;
    public bool CanSwitchColor { get; set; } = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいでも破棄しない
        }
        else
        {
            Destroy(gameObject); // すでに存在する場合は重複しないよう削除
        }
    }
}