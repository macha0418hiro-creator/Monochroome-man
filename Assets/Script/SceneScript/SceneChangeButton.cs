using System;
using UnityEngine;

public class SceneChangeButton : MonoBehaviour
{
    [Header("ボタンが押されたとき遷移するScene名")]
    [SerializeField] private string targetSceneName;

    public void OnClickSceneChange()
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            //Debug.LogWarning("遷移先のScene名が空です!設定してください。");
            return;
        }

        SceneNavigator.LoadTargetScene(targetSceneName);
    }
}
