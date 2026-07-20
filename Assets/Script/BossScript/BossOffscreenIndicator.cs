using Unity.VisualScripting;
using UnityEngine;

public class BossOffscreenIndicator : MonoBehaviour
{
    [Header("追跡対象のボス")]
    [SerializeField] private Transform bossTransform;

    [Header("使用するカメラ")]
    [SerializeField] private Camera mainCamera;

    [Header("ボスアイコン")]
    [SerializeField] private RectTransform indicatorUI;

    [Header("画面端とアイコンとの余白")]
    [SerializeField] private float edgePadding = 50f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if(mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (indicatorUI)
        {
            indicatorUI.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if(bossTransform == null || !bossTransform.gameObject.activeInHierarchy)
        {
            if(indicatorUI != null && indicatorUI.gameObject.activeSelf)
            {
                indicatorUI.gameObject.SetActive(false);
            }
            return;
        }

        //ボスを画面を基準に座標をとる。画面内の座標を右上を(1,1)、左下を(0,0)とする
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(bossTransform.position);

        bool isOffscreen = viewportPos.x < 0f || viewportPos.x > 1f || viewportPos.y < 0f || viewportPos.y > 1f;

        if (isOffscreen)
        {
            if (!indicatorUI.gameObject.activeSelf)
            {
                indicatorUI.gameObject.SetActive(true);
            }

            //アイコンが画面外に行かないように制限をつける
            Vector3 screenPos = mainCamera.WorldToScreenPoint(bossTransform.position);

            if(viewportPos.z < 0)
            {
                screenPos *= -1f;
            }

            float minX = edgePadding;
            float maxX = Screen.width - edgePadding;
            float minY = edgePadding;
            float maxY = Screen.height - edgePadding;

            screenPos.x = Mathf.Clamp(screenPos.x, minX, maxX);
            screenPos.y = Mathf.Clamp(screenPos.y, minY, maxY);

            //UIの座標更新
            indicatorUI.position = screenPos;

            //アイコンが矢印の場合その方向へ画像を向かせる(まだ未定)
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            Vector3 dir = screenPos - screenCenter;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            indicatorUI.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }
        else
        {
            //画面内にボスがいるときアイコンを消す
            if (indicatorUI.gameObject.activeSelf)
            {
                indicatorUI.gameObject.SetActive(false);
            }
        }
    }
}
