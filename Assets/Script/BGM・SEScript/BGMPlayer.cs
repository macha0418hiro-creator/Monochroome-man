using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    [Header("このシーンで再生したいBGM")]
    [SerializeField] private SoundManager.BGMType bgmType;

    [Header("フェードイン時間(秒)")]
    [SerializeField] private float fadeDuration = 0.5f;

    private void Start()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBGM(bgmType, fadeDuration);
        }
    }
}