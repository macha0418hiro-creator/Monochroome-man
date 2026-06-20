using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneNavigator
{
    public static string NextSceneName { get; private set; } = "Stage1";

    //ロード画面へ遷移(行先の記憶もする)
    public static void LoadTargetScene(string targetSceneName)
    {
        NextSceneName = targetSceneName;

        SceneManager.LoadScene("LoadingScene");
    }
}


