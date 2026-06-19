using System;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class FrogAttack : MonoBehaviour
{
    [Header("攻撃関連")]
    [SerializeField] private float attackRange = 3f;    //攻撃を始める距離
    [SerializeField] private float attackSpeed = 0.5f;  //攻撃のスピード
    [SerializeField] private float attackCooldown = 3f; //次に攻撃するまでの間隔
    [SerializeField] private Transform tongueObject;    //舌のObject

    [Header("角度制限設定")]
    [Range(0f, 90f)]
    [SerializeField] private float maxAttackAngle = 45f;

    private LineRenderer lineRenderer;
    private Transform playerTransform;
    private FrogMove frogMove;          //攻撃中移動処理を止めるため
    private bool isAttacking = false;   //攻撃の最中か判断用

    public bool IsAttacking => isAttacking;
    public float AttackRange => attackRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        frogMove = GetComponent<FrogMove>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player != null) playerTransform = player.transform;

        if(lineRenderer != null) lineRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //FrogMoveから合図をもらうと攻撃する
    public void TryAttack()
    {
        if (!isAttacking)
        {
            StartCoroutine(TongueAttackRoutine());
        }
    }

    private IEnumerator TongueAttackRoutine()
    {
        isAttacking = true;

        //攻撃前にプレイヤーの方向を向く
        if (frogMove != null) frogMove.TargetPlayerDirection();

        Vector3 startPosition = transform.position;

        //角度制限内でプレイヤーの位置を計算する
        Vector3 targetPosition = CalculateClampedTargetPosition(startPosition);

        if(lineRenderer != null) lineRenderer.enabled = true;
        if(tongueObject != null)
        {
            tongueObject.gameObject.SetActive(true);
            tongueObject.position = startPosition;
        }

        //舌を伸ばす処理
        float progress = 0f;
        while (progress < 1f)
        {
            progress += Time.deltaTime * (attackSpeed / attackRange);
            Vector3 currentObjectPos = Vector3.Lerp(startPosition, targetPosition, progress);

            if (tongueObject != null) tongueObject.position = currentObjectPos;
            if (lineRenderer != null)
            {
                lineRenderer.SetPosition(0, transform.position);    //スタートする頂点
                lineRenderer.SetPosition(1, currentObjectPos);      //終わりの頂点
            }
            yield return null;
        }

        //舌を戻す処理
        progress = 0f;
        Vector3 reachedPosition = tongueObject != null ? tongueObject.position : targetPosition;
        while(progress < 1f)
        {
            progress += Time.deltaTime * (attackSpeed / attackRange);
            Vector3 currentObjectPos = Vector3.Lerp(reachedPosition, transform.position, progress);

            if(tongueObject != null) tongueObject.position = currentObjectPos;
            if(lineRenderer != null)
            {
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, currentObjectPos);
            }
            yield return null;
        }

        if (lineRenderer != null) lineRenderer.enabled = false;
        if(tongueObject != null) tongueObject.gameObject.SetActive(false);

        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;

        if (frogMove != null) frogMove.ResetJumpTimer();
    }

    //制限角度内で舌を伸ばせる範囲を割り出す
    private Vector3 CalculateClampedTargetPosition(Vector3 startPos)
    {
        if (playerTransform == null) return startPos + Vector3.left * attackRange;

        //カエルからプレイヤーへの方向
        Vector3 directionToPlayer = playerTransform.position - startPos;
        directionToPlayer.z = 0f;

        //カエルの現在の方向(正面)
        Vector3 facingDirection = (transform.localScale.x < 0f) ? Vector3.right : Vector3.left;

        if (playerTransform.position.y <= startPos.y)
        {
            return startPos + facingDirection * attackRange;
        }
        else
        {
            float currenAngle = Vector3.Angle(facingDirection, directionToPlayer);

            if (currenAngle > maxAttackAngle)
            {
                Quaternion rotation = Quaternion.Euler(0, 0, maxAttackAngle * 1f);

                if (transform.localScale.x >= 0f)
                {
                    rotation = Quaternion.Euler(0, 0, -maxAttackAngle * 1f);
                }

                Vector3 clampedDirection = rotation * facingDirection;

                return startPos + clampedDirection.normalized * attackRange;
            }
            else
            {
                return startPos + directionToPlayer.normalized * attackRange;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 startPos = transform.position;
        bool isRight = transform.localScale.x < 0f;
        Vector3 facingDirection = (transform.localScale.x < 0f) ? Vector3.right : Vector3.left;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(startPos, startPos + facingDirection * attackRange);

        float rotSign = isRight ? 1f : -1f;
        Vector3 upperDirection = Quaternion.Euler(0, 0, maxAttackAngle * rotSign) * facingDirection;
        Gizmos.DrawLine(startPos, startPos + upperDirection * attackRange);
    }
}



