using UnityEngine;

public class FrogTongueVisual : MonoBehaviour
{
    [Header("ベロのスプライトレンダラー")]
    [SerializeField] private SpriteRenderer tongueTipRenderer;   // ベロの先端
    [SerializeField] private SpriteRenderer tongueShaftRenderer; // ベロの伸びる部分（シャフト）

    [Header("白カエル用のベロ素材")]
    [SerializeField] private Sprite whiteTongueTip;
    [SerializeField] private Sprite whiteTongueShaft;

    [Header("黒カエル用のベロ素材")]
    [SerializeField] private Sprite blackTongueTip;
    [SerializeField] private Sprite blackTongueShaft;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //EnemyAttributeControlleから呼び出される関数
    public void ChangeTongueColor(EnemyAttributeController.EnemyColor color)
    {
        if (color == EnemyAttributeController.EnemyColor.White)
        {
            if (tongueTipRenderer != null) tongueTipRenderer.sprite = whiteTongueTip;
            if (tongueShaftRenderer != null) tongueShaftRenderer.sprite = whiteTongueShaft;

            SetLayerRecursively(gameObject, LayerMask.NameToLayer("EnemySensorWhite"));
        }
        else if (color == EnemyAttributeController.EnemyColor.Black)
        {
            if (tongueTipRenderer != null) tongueTipRenderer.sprite = blackTongueTip;
            if (tongueShaftRenderer != null) tongueShaftRenderer.sprite = blackTongueShaft;

            SetLayerRecursively(gameObject, LayerMask.NameToLayer("EnemySensorBlack"));
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (newLayer == -1) return; // レイヤーが存在しない場合はスキップ

        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
