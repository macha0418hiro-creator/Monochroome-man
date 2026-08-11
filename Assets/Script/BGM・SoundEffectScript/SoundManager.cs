using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    // --------------------------------------------------
    //  SE（効果音）の設定
    // --------------------------------------------------
    public enum SEType
    {
        Attack,         //攻撃
        Jump,           //ジャンプ
        Get,            //アイテム入手
        ColorChange,    //色変更
        Damage,         //ダメージ
        Disappearance,  //消滅
        ButtonClick,    //ボタン押下
        DogClick,       //矢印・Dog押下時
        Win,            //勝利
        Lose,           //敗北

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