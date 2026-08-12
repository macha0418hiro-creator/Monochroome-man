using UnityEngine;
using UnityEngine.UI;

public class SettingsWindow : MonoBehaviour
{
    [Header("音量スライダー")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;

    [Header("閉じるボタン")]
    [SerializeField] private Button closeButton;

    private void OnEnable()
    {
        //画面が開いたタイミングで各スライダーの表示を現在の音量設定に合わせる
        if (SoundManager.Instance != null)
        {
            if (masterSlider != null) masterSlider.value = SoundManager.Instance.GetMasterVolume();
            if (bgmSlider != null) bgmSlider.value = SoundManager.Instance.GetBGMVolume();
            if (seSlider != null) seSlider.value = SoundManager.Instance.GetSEVolume();
        }
    }

    private void Start()
    {
        //スライダーの最小値・最大値を初期化し、イベントを登録
        InitSlider(masterSlider, OnMasterSliderChanged);
        InitSlider(bgmSlider, OnBGMSliderChanged);
        InitSlider(seSlider, OnSESliderChanged);

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseSettings);
        }
    }

    private void InitSlider(Slider slider, UnityEngine.Events.UnityAction<float> action)
    {
        if (slider == null) return;
        slider.minValue = 0.0001f;
        slider.maxValue = 1f;
        slider.onValueChanged.AddListener(action);
    }

    private void OnMasterSliderChanged(float value)
    {
        SoundManager.Instance?.SetMasterVolume(value);
    }

    private void OnBGMSliderChanged(float value)
    {
        SoundManager.Instance?.SetBGMVolume(value);
    }

    private void OnSESliderChanged(float value)
    {
        SoundManager.Instance?.SetSEVolume(value);
    }

    public void CloseSettings()
    {
        //閉じるボタンのSEを鳴らして画面を非表示化
        SoundManager.Instance?.PlaySE(SoundManager.SEType.ButtonClick);
        gameObject.SetActive(false);
    }
}