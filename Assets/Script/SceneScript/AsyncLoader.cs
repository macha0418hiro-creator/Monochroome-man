using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class AsyncLoader : MonoBehaviour
{
    [Header("UIの設定")]
    [SerializeField] private Slider progressBar;        //ロードバー
    [SerializeField] private TMP_Text loadingText;      //テキスト
    [SerializeField] private GameObject textAnyKey;     //文字Object

    private  bool isLoadCompleted = false;
    private AsyncOperation asyncOperation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (loadingText != null) loadingText.text = "";
        if(textAnyKey != null) textAnyKey.SetActive(false);
        StartCoroutine(LoadSceneRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        if(isLoadCompleted)
        {
            var keyboard = Keyboard.current;
            var pointer = Pointer.current;

            //フレームが切り替わってからクリック、スペースを押されたかの判定
            bool isSpacePressed = Keyboard.current != null && keyboard.spaceKey.wasPressedThisFrame;
            bool isClickPressed = Pointer.current != null && pointer.press.wasPressedThisFrame;

            if (isSpacePressed || isClickPressed)
            {
                StartCoroutine(ActivateSceneRoutine());
            }
        }
    }

    private IEnumerator LoadSceneRoutine()
    {
        yield return new WaitForSeconds(0.5f);  //画面が急に変わらないように溜を作る

        //SceneNavigatorに保存された次の画面情報を読み込む
        asyncOperation = SceneManager.LoadSceneAsync(SceneNavigator.NextSceneName);

        //勝手に遷移が起こらないよう止める
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            //ロードの進捗率を受け取る(allowSceneActivationがfalseの時、90%で止まる)
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);

            //画面に反映
            if(progressBar != null) progressBar.value = progress;

            if (loadingText != null && !isLoadCompleted)
            {
                loadingText.text = $"Loading... {(progress * 100):F0}%";
            }

            if(asyncOperation.progress >= 0.9f)
            {
                isLoadCompleted = true;
                if (loadingText != null) loadingText.text = "COMPLETE";
                if (textAnyKey != null) textAnyKey.SetActive(true); 
            }

            yield return null;
        }
    }

    private IEnumerator ActivateSceneRoutine()
    {
        if(asyncOperation != null)
        {
            asyncOperation.allowSceneActivation = true;
            yield return asyncOperation;
        }
    }
}
