using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class BaseBossWizard : MonoBehaviour
{
    [Header("テレポート位置の候補")]
    [SerializeField] protected List<Transform> teleportPoints;

    [Header("行動の間隔")]
    [SerializeField] private float intervalTime = 1.5f;

    [Header("攻撃A(追尾弾)のGameObject")]
    [SerializeField] private GameObject homingBulletObject;

    [Header("攻撃B(配置弾)のGameObject")]
    [SerializeField] private GameObject placedBulletObject;

    protected int lastAttackIndex = -1;     //前回使った技を記録
    protected int lastTeleportIndex = -1;   //前回のテレポート場所を記録
    protected bool isActionRunning = false;

    //ボスの行動(ループ)に入れる
    protected virtual void Start()
    {
        StartCoroutine(BossActionLoop());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator BossActionLoop()
    {
        yield　return new WaitForSeconds(intervalTime);

        while (true)
        {
            //ポーズ中は行動しない
            if (PauseManager.IsPaused)
            {
                yield return null;
                continue;
            }

            //攻撃を実行
            isActionRunning = true;
            ChooseNextAttack();

            //攻撃が終わるのを待つ
            while (isActionRunning) yield return null;

            //攻撃の間隔
            yield return new WaitForSeconds(intervalTime);

            //テレポートを実行
            isActionRunning = true;
            TeleportToRandomPoint();

            //テレポートが終わるのを待つ
            while(isActionRunning) yield return null;

            yield return new WaitForSeconds(intervalTime);
        }
    }
    

    //攻撃をランダムに決める処理
    private void ChooseNextAttack()
    {
        int totalAttacks = 4;
        List<int> availableAttacks = new List<int>();

        for (int i = 0; i < totalAttacks; i++) 
        {
            //前回と同じ技でないか判定
            if(i != lastAttackIndex)
            {
                availableAttacks.Add(i);
            }
        }

        //前回と同じでない技の中からランダムに選ばれる
        int nextAttack = availableAttacks[Random.Range(0, availableAttacks.Count)];
        lastAttackIndex = nextAttack;

        ExecuteAttack(nextAttack);
    }

    private void ExecuteAttack(int index)
    {
        switch (index)
        {
            case 0: StartCoroutine(AttackA()); break;   //共通技
            case 1: StartCoroutine(AttackB()); break;   //共通技
            case 2: StartCoroutine(CustomAttackWithVariation()); break;    //変化のある共通技
            case 3: StartCoroutine(UniqueSpecialAttack()); break;           //キャラ専用技
        }
    }

    //ランダムな位置へテレポート
    private void TeleportToRandomPoint()
    {
        if(teleportPoints == null || teleportPoints.Count == 0)
        {
            isActionRunning = false;
            return;
        }

        List<int> availablePoints = new List<int>();
        for(int i = 0; i < teleportPoints.Count; i++)
        {
            //前回と同じ位置でないか判定
            if (i != lastTeleportIndex)
            {
                availablePoints.Add(i);
            }
        }

        //前回と同じでない位置の中からランダムに選ばれる
        int nextPointIndex = availablePoints[Random.Range(0, availablePoints.Count)];
        lastTeleportIndex = nextPointIndex;

        StartCoroutine(TeleportRoutine(teleportPoints[nextPointIndex].position));
    }

    private IEnumerator TeleportRoutine(Vector3 targetPosition)
    {
        Debug.Log($"{gameObject.name} がテレポートを開始します。");

        // 座標を書き換え（一瞬で移動）
        transform.position = targetPosition;

        yield return new WaitForSeconds(0.5f); // テレポート直後の硬直時間

        Debug.Log($"{gameObject.name} がテレポートを完了しました。");
        isActionRunning = false;
    }

    protected IEnumerator AttackA()
    {
        Debug.Log($"{gameObject.name} の基本攻撃A");

        if (homingBulletObject != null)
        {
            for (int i = 0; i < 3; i++)
            {
                //弾を生成する処理
                GameObject bulletObj = Instantiate(homingBulletObject, transform.position, Quaternion.identity);

                //レイヤーの同期
                bulletObj.layer = this.gameObject.layer;

                yield return new WaitForSeconds(0.3f);
            }
        }

        yield return new WaitForSeconds(1.0f);
        isActionRunning = false;
    }

    protected IEnumerator AttackB()
    {
        Debug.Log($"{gameObject.name} の基本攻撃B");

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && placedBulletObject != null)
        {
            Transform playerTransform = player.transform;

            //生成した弾をリストで記憶
            List<PlacedBullet> spawnedBullets = new List<PlacedBullet>();

            int bulletCount = 5;
            float radius = 2f;

            //ボスの周囲の空中に弾を配置
            for (int i = 0; i < bulletCount; i++)
            {
                //弾の座標を作る処理
                float angle = (i - (bulletCount - 1) / 2f) * 30f;
                Vector3 offset = Quaternion.Euler(0, 0, angle) * Vector3.up * radius;
                Vector3 spawnPosition = transform.position + offset;

                GameObject bulletObj = Instantiate(placedBulletObject, spawnPosition, Quaternion.identity);
                bulletObj.layer = this.gameObject.layer;

                //生成した弾を取得してリストに貯める
                PlacedBullet bulletScript = bulletObj.GetComponent<PlacedBullet>();
                if (bulletScript != null)
                {
                    spawnedBullets.Add(bulletScript);
                }
            }

            yield return new WaitForSeconds(1.0f);

            //発射の合図を送る処理
            foreach (PlacedBullet bullet in spawnedBullets)
            {
                if (bullet != null && playerTransform != null)
                {
                    // 弾側のスクリプトにプレイヤーの現在位置を渡して攻撃
                    bullet.Launch(playerTransform.position);
                }
            }
        }

        yield return new WaitForSeconds(1.0f);
        isActionRunning = false;
    }

    protected abstract IEnumerator CustomAttackWithVariation();
    protected abstract IEnumerator UniqueSpecialAttack();
}


