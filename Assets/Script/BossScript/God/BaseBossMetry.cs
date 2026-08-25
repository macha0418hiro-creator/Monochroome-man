using System.Collections;
using UnityEngine;

public abstract class BaseBossMetry : MonoBehaviour
{
    [Header("共通参照")]
    [SerializeField] protected Transform stageCenter;
    [SerializeField] protected Transform playerTransform;

    [Header("コンポーネント参照 (空なら子オブジェクトから自動取得)")]
    [SerializeField] protected EnemyHealth enemyHealth;
    [SerializeField] protected EnemyAttributeController attributeController;

    [Header("行動設定")]
    [SerializeField] protected float attackInterval = 2.0f;

    protected bool isActionRunning = false;
    protected bool isDead = false;

    protected virtual void Awake()
    {
        // まず自身（Asymmetry）から EnemyHealth を探す
        enemyHealth = GetComponent<EnemyHealth>();

        // 自身になければ、子（DamageSensor）から探す（フォールバック）
        if (enemyHealth == null)
        {
            enemyHealth = GetComponentInChildren<EnemyHealth>();
        }

        if (attributeController == null)
        {
            attributeController = GetComponentInChildren<EnemyAttributeController>();
        }
    }

    protected virtual void OnEnable()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnDied += HandleDied;
        }
    }

    protected virtual void OnDisable()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnDied -= HandleDied;
        }
    }

    protected virtual void Start()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
        }

        StartCoroutine(BossBehaviorLoop());
    }

    protected IEnumerator BossBehaviorLoop()
    {
        yield return new WaitForSeconds(1.0f);

        while (!isDead)
        {
            if (!isActionRunning)
            {
                isActionRunning = true;
                yield return StartCoroutine(ExecuteAttackPattern());
                yield return new WaitForSeconds(attackInterval);
            }
            yield return null;
        }
    }

    protected abstract IEnumerator ExecuteAttackPattern();

    protected virtual void HandleDied()
    {
        isDead = true;
        StopAllCoroutines();
    }
}