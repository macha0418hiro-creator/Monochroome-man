using UnityEngine;

public class BossBattleManager : MonoBehaviour
{
    [Header("登場させるボス")]
    [SerializeField] private GameObject bossObject;

    [Header("入り口を塞ぐオブジェクト")]
    [SerializeField] private GameObject bossRoomWall;

    private bool isBattleStarted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //ボスと壁を初期は非表示
    private void Awake()
    {
        if (bossObject != null) bossObject.SetActive(false);
        if(bossRoomWall != null) bossRoomWall.SetActive(false);
    }

    //プレイヤーがボス部屋に入ったら実行
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !isBattleStarted)
        {
            isBattleStarted = true;
            StartBossSequence();
        }
    }

    //ボス戦開始
    private void StartBossSequence()
    {
        Debug.Log("ボス戦開始");

        if(bossRoomWall != null)
        {
            bossRoomWall.SetActive(true);
        }

        if(bossObject != null)
        {
            bossObject.SetActive(true);

            BaseBossWizard bossScript = bossObject.GetComponent<BaseBossWizard>();
            if(bossScript != null)
            {
                bossScript.StartBossBattle();
            }
        }
    }
}
