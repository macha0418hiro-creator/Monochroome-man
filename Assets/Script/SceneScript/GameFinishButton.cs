using UnityEngine;

public class GameFinishButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //ゲーム終了処理
    // ゲーム終了ボタンに割り当てているメソッド
    public void OnClickGameFinish()
    {
#if UNITY_EDITOR
        // Unityエディタ上で実行中の場合は、再生を停止する
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // ビルドされたゲーム本編で実行中の場合は、アプリを終了する
        Application.Quit();
#endif
    }
}
