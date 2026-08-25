using UnityEngine;

public class PlayerHeadPromptUI : MonoBehaviour
{
    public static PlayerHeadPromptUI Instance { get; private set; }

    [Header("頭上に表示するキー画像UI (CanvasやSprite)")]
    [SerializeField] private GameObject promptUIObject;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        HidePrompt();
    }

    // プレイヤーの反転処理（Update等）の後に実行して向きを補正する
    private void LateUpdate()
    {
        if (promptUIObject != null && promptUIObject.activeSelf)
        {
            // 親（プレイヤー）の Scale.x がマイナスなら、自身の Scale.x もマイナスにして相殺する
            // （マイナス × マイナス ＝ プラスの向きに固定される）
            Vector3 currentScale = transform.localScale;
            float parentScaleX = transform.parent != null ? transform.parent.localScale.x : 1f;

            if (parentScaleX < 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(currentScale.x), currentScale.y, currentScale.z);
            }
            else
            {
                transform.localScale = new Vector3(Mathf.Abs(currentScale.x), currentScale.y, currentScale.z);
            }
        }
    }

    /// <summary>
    /// 頭上UIを表示する
    /// </summary>
    public void ShowPrompt()
    {
        if (promptUIObject != null)
        {
            promptUIObject.SetActive(true);
        }
    }

    /// <summary>
    /// 頭上UIを非表示にする
    /// </summary>
    public void HidePrompt()
    {
        if (promptUIObject != null)
        {
            promptUIObject.SetActive(false);
        }
    }
}