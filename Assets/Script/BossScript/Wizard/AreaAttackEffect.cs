using System.Collections;
using UnityEngine;

public class AreaAttackEffect : MonoBehaviour
{
    [System.Serializable]
    public struct VisualSetup
    {
        public Sprite warningSprite;    //予備動作時のSprite
        public Sprite activeSprite;     //攻撃時Sprite
    }

    [Header("ボスの色に合わせた見た目の設定")]
    [SerializeField] private VisualSetup setupForWhiteBoss; //白用のSprite
    [SerializeField] private VisualSetup setupForBlackBoss; //黒用のSprite

    [Header("エリア攻撃の設定")]
    [SerializeField] private float attackRadius = 5f;       //円の半径
    [SerializeField] private float warningDuration = 1.5f;  //予備動作時間
    [SerializeField] private float attackDuration = 1.0f;   //攻撃の持続時間

    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;    //白の攻撃範囲
    private PolygonCollider2D donutCollider;    //黒の攻撃範囲

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        donutCollider = GetComponent<PolygonCollider2D>();

        //予備動作のためにダメージ判定を無効化
        if(circleCollider != null) circleCollider.enabled = false;
        if(donutCollider != null) donutCollider.enabled = false;
    }

    public IEnumerator ExecuteAreaAttackRoutine(bool isWhiteBoss)
    {
        transform.localScale = new Vector3(attackRadius * 2, attackRadius * 2, 1);

        //色によって予備動作のSpriteを表示
        VisualSetup currentSetup = isWhiteBoss ? setupForWhiteBoss : setupForBlackBoss;
        spriteRenderer.sprite = currentSetup.warningSprite;

        yield return new WaitForSeconds(warningDuration);

        //攻撃時のSpriteに変更
        spriteRenderer.sprite = currentSetup.activeSprite;

        if (isWhiteBoss)
        {
            if (circleCollider != null) circleCollider.enabled = true;
        }
        else
        {
            if (donutCollider != null) donutCollider.enabled = true;
        }

        yield return new WaitForSeconds(attackDuration);

        Destroy(gameObject);
    }
}
