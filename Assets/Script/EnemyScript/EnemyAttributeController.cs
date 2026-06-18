using System;
using UnityEngine;

public class EnemyAttributeController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public enum EnemyColor
    {
        White,
        Black
    }

    [Header("エネミーの属性")]
    [SerializeField] EnemyColor currentAttribute = EnemyColor.White;

    [Header("立ち絵(Sprite)の設定")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite whiteSprite;
    [SerializeField] private Sprite blackSprite;

    [Header("連動させるセンサー")]
    [SerializeField] private GameObject damageSensor;

    //他の処理(Script)から現在の色を受け取れるための処理
    public EnemyColor CurrentAttribute => currentAttribute;

    private Animator animator;


    void Start()
    {
        animator = GetComponent<Animator>();

        ApplyAttribute(currentAttribute);
    }

    // Update is called once per frame
    void Update()
    {
        //アニメーターに現在の色（白 = 0, 黒 = 1）を伝える
        if (animator != null)
        {
            int index = (currentAttribute == EnemyColor.White) ? 0 : 1;
            animator.SetFloat("colorIndex", index);
        }
}

    public void ApplyAttribute(EnemyColor newAttribute)
    {
        currentAttribute = newAttribute;

        // 1. 属性に合わせて見た目を切り替える
        if (spriteRenderer != null)
        {
            if (currentAttribute == EnemyColor.White)
            {
                spriteRenderer.sprite = whiteSprite;
            }
            else if (currentAttribute == EnemyColor.Black)
            {
                spriteRenderer.sprite = blackSprite;
            }
        }

        // 2. 当たり判定（レイヤー）を切り替える
        string layerNmae = currentAttribute == EnemyColor.White ? "EnemyWhite" : "EnemyBlack";

        int layerID = LayerMask.NameToLayer(layerNmae);

        if (layerID != -1) // レイヤーがUnity側に存在する場合のみレイヤーを変更
        {
            gameObject.layer = layerID;
        }

        // 3.ダメージセンサー(レイヤー)を切り替える
        if(damageSensor != null)
        {
            string sensorLayerName = currentAttribute == EnemyColor.White ? "EnemySensorWhite" : "EnemySensorBlack";
            int sensorLayerID = LayerMask.NameToLayer(sensorLayerName);

            if(sensorLayerID != -1)
            {
                damageSensor.layer = sensorLayerID;
            }
        }

        Debug.Log($"[{gameObject.name}]属性を【{currentAttribute}】に設定し、レイヤーを切り替えました");

    }
}

