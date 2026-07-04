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
    public void OnClickFinish()
    {
        Debug.Log("ゲームが終了されました");

        //Unityエディタ上の再生を止める
        UnityEditor.EditorApplication.isPlaying = false;

        //アプリ自体を落とす(アプリとして完成していたら)
        Application.Quit();
    }
}
