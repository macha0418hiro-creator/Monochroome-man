using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class OpenSettingsButton : MonoBehaviour
{
    [Header("表示させたい設定画面パネル (SettingsPanel)")]
    [SerializeField] private SettingsWindow settingsWindow;

    private void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(OpenSettings);
    }

    private void OpenSettings()
    {
        SoundManager.Instance?.PlaySE(SoundManager.SEType.ButtonClick);

        if (settingsWindow != null)
        {
            settingsWindow.gameObject.SetActive(true);
        }
        else
        {
            //インスペクター未割り当ての場合、シーン内から自動で非アクティブなSettingsWindowを探す
            SettingsWindow foundWindow = FindAnyObjectByType<SettingsWindow>(FindObjectsInactive.Include);
            if (foundWindow != null)
            {
                foundWindow.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("設定パネル (SettingsWindow) が見つかりません。");
            }
        }
    }
}