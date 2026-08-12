using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Mixer の設定")]
    [SerializeField] private AudioMixer audioMixer;

    // AudioMixerのExposed Parameter名
    private const string MASTER_PARAM = "MasterVolume";
    private const string BGM_PARAM = "BGMVolume";
    private const string SE_PARAM = "SEVolume";

    // PlayerPrefsの保存用キー
    private const string MASTER_KEY = "Vol_Master";
    private const string BGM_KEY = "Vol_BGM";
    private const string SE_KEY = "Vol_SE";

    // --------------------------------------------------
    //  SE（効果音）の設定
    // --------------------------------------------------
    public enum SEType
    {
        //Player
        Attack,         //攻撃
        Jump,           //ジャンプ
        Get,            //アイテム入手
        ColorChange,    //色変更
        Damage,         //ダメージ
        Disappearance,  //消滅

        //System
        ButtonClick,    //ボタン押下
        DogClick,       //矢印・Dog押下時
        Win,            //勝利
        Lose,           //敗北

        //Enemy
        Explosion,      //爆発
        Beam,           //ビーム

    }

    [System.Serializable]
    public struct SESoundData
    {
        public SEType type;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume;
    }

    // --------------------------------------------------
    // 🎵 BGM（背景音楽）の設定
    // --------------------------------------------------
    public enum BGMType
    {
        Title,       //タイトル画面
        StageSelect, //ステージ選択画面
        Stage1,      //ステージ1用BGM
        Stage2,      //ステージ2用BGM
        Clear        //クリア画面
    }

    [System.Serializable]
    public struct BGMSoundData
    {
        public BGMType type;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume;
    }

    [Header("SEの設定")]
    [SerializeField] private List<SESoundData> seList;
    [SerializeField] private AudioSource seAudioSource;

    [Header("BGMの設定")]
    [SerializeField] private List<BGMSoundData> bgmList;
    [SerializeField] private AudioSource bgmAudioSource;

    private Dictionary<SEType, SESoundData> seDictionary = new Dictionary<SEType, SESoundData>();
    private Dictionary<BGMType, BGMSoundData> bgmDictionary = new Dictionary<BGMType, BGMSoundData>();

    private BGMType? currentBGMType = null;
    private Coroutine bgmFadeCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitDictionaries();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 起動時に保存された音量を読み込んで適用
        LoadAndApplyVolumeSettings();
    }

    private void InitDictionaries()
    {
        seDictionary.Clear();
        foreach (var data in seList)
        {
            if (!seDictionary.ContainsKey(data.type))
            {
                seDictionary.Add(data.type, data);
            }
        }

        bgmDictionary.Clear();
        foreach (var data in bgmList)
        {
            if (!bgmDictionary.ContainsKey(data.type))
            {
                bgmDictionary.Add(data.type, data);
            }
        }
    }

    // --------------------------------------------------
    // 🎛️ 音量コントロール（0.0001f 〜 1.0f のスライダー値をdBに変換）
    // --------------------------------------------------
    public void SetMasterVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        if (audioMixer != null)
        {
            audioMixer.SetFloat(MASTER_PARAM, Mathf.Log10(volume) * 20);
        }
        PlayerPrefs.SetFloat(MASTER_KEY, volume);
    }

    public void SetBGMVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        if (audioMixer != null)
        {
            audioMixer.SetFloat(BGM_PARAM, Mathf.Log10(volume) * 20);
        }
        PlayerPrefs.SetFloat(BGM_KEY, volume);
    }

    public void SetSEVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        if (audioMixer != null)
        {
            audioMixer.SetFloat(SE_PARAM, Mathf.Log10(volume) * 20);
        }
        PlayerPrefs.SetFloat(SE_KEY, volume);
    }

    public float GetMasterVolume() => PlayerPrefs.GetFloat(MASTER_KEY, 1f);
    public float GetBGMVolume() => PlayerPrefs.GetFloat(BGM_KEY, 1f);
    public float GetSEVolume() => PlayerPrefs.GetFloat(SE_KEY, 1f);

    private void LoadAndApplyVolumeSettings()
    {
        SetMasterVolume(GetMasterVolume());
        SetBGMVolume(GetBGMVolume());
        SetSEVolume(GetSEVolume());
    }

    // --------------------------------------------------
    //  SEの再生処理
    // --------------------------------------------------
    public void PlaySE(SEType type)
    {
        if (seDictionary.TryGetValue(type, out SESoundData data))
        {
            if (data.clip != null && seAudioSource != null)
            {
                seAudioSource.PlayOneShot(data.clip, data.volume);
            }
        }
        else
        {
            Debug.LogWarning($"SE {type} がSoundManagerに登録されていません。");
        }
    }

    // --------------------------------------------------
    // 🎵 BGMの再生・フェード処理
    // --------------------------------------------------

    /// <summary>
    /// BGMを再生（fadeDuration > 0 でフェード切り替え）
    /// </summary>
    public void PlayBGM(BGMType type, float fadeDuration = 0.5f)
    {
        //既に同じBGMが流れている場合は何もしない
        if (currentBGMType == type && bgmAudioSource.isPlaying)
        {
            return;
        }

        if (bgmDictionary.TryGetValue(type, out BGMSoundData data))
        {
            if (data.clip == null || bgmAudioSource == null) return;

            if (bgmFadeCoroutine != null)
            {
                StopCoroutine(bgmFadeCoroutine);
            }

            bgmFadeCoroutine = StartCoroutine(ChangeBGMCoroutine(data, fadeDuration));
        }
        else
        {
            Debug.LogWarning($"BGM {type} がSoundManagerに登録されていません。");
        }
    }

    /// <summary>
    /// BGMを停止する
    /// </summary>
    public void StopBGM(float fadeDuration = 0.5f)
    {
        if (bgmAudioSource == null || !bgmAudioSource.isPlaying) return;

        if (bgmFadeCoroutine != null)
        {
            StopCoroutine(bgmFadeCoroutine);
        }

        bgmFadeCoroutine = StartCoroutine(StopBGMCoroutine(fadeDuration));
    }

    //BGMの切り替え用コルーチン（クロスフェード）
    private IEnumerator ChangeBGMCoroutine(BGMSoundData nextData, float fadeDuration)
    {
        float startVolume = bgmAudioSource.volume;

        // フェードアウト
        if (bgmAudioSource.isPlaying && fadeDuration > 0f)
        {
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                bgmAudioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
                yield return null;
            }
        }

        //曲の差し替え
        bgmAudioSource.Stop();
        bgmAudioSource.clip = nextData.clip;
        bgmAudioSource.loop = true; //BGMは常にループ再生
        bgmAudioSource.Play();
        currentBGMType = nextData.type;

        //フェードイン
        if (fadeDuration > 0f)
        {
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                bgmAudioSource.volume = Mathf.Lerp(0f, nextData.volume, t / fadeDuration);
                yield return null;
            }
        }

        bgmAudioSource.volume = nextData.volume;
    }

    //BGMの停止用コルーチン
    private IEnumerator StopBGMCoroutine(float fadeDuration)
    {
        float startVolume = bgmAudioSource.volume;

        if (fadeDuration > 0f)
        {
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                bgmAudioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
                yield return null;
            }
        }

        bgmAudioSource.Stop();
        bgmAudioSource.volume = 0f;
        currentBGMType = null;
    }
}