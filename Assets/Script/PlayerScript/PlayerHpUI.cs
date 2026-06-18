using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEngine.Assemblies;

public class PlayerHpUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("ハートのImageコンポーネントを左から順に表示")]
    [SerializeField] private List<Image> heartImages = new List<Image>();

    [Header("ハートの画像素材(sprite)")]
    [SerializeField] private Sprite whiteHeartFull;  // 白属性：満タン
    [SerializeField] private Sprite whiteHeartEmpty; // 白属性：空
    [Space(10)]
    [SerializeField] private Sprite blackHeartFull;  // 黒属性：満タン
    [SerializeField] private Sprite blackHeartEmpty; // 黒属性：空

    // 現在の属性に合わせて使う画像を格納する変数
    private Sprite currentFullSprite;
    private Sprite currentEmptySprite;
    private int lastHp;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        // ゲーム開始時の初期値（白属性のセット）
        currentFullSprite = whiteHeartFull;
        currentEmptySprite = whiteHeartEmpty;

        // 最初は登録されたハートのイメージの数（＝最大HP）を満タンとして記憶しておく
        if (heartImages != null && heartImages.Count > 0)
        {
            lastHp = heartImages.Count;
        }
    }

    // プレイヤーの色が変わった時にHPの見た目を変える
    public void ChangeUiColor(PlayerAttributeController.PlayerColor newrColor)
    {
        if(newrColor == PlayerAttributeController.PlayerColor.White)
        {
            currentFullSprite = whiteHeartFull;
            currentEmptySprite = whiteHeartEmpty;
        }
        else
        {
            currentFullSprite = blackHeartFull;
            currentEmptySprite= blackHeartEmpty;
        }

        // 画像の定義が変わったので、現在のHPのまま見た目を更新
        UpdateHpUI(lastHp);
    }

    // HPが変わった時に呼ばれる
    public void UpdateHpUI(int currentHp)
    {
        lastHp = currentHp;

        for (int i = 0; i < heartImages.Count; i++)
        {
            if(i < currentHp)
            {
                heartImages[i].sprite = currentFullSprite;  // 満タンのハート
            }
            else
            {
                heartImages[i].sprite = currentEmptySprite; // 空のハート
            }
        }
    }
}
