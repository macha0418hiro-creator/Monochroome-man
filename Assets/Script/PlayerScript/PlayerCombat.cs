using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("攻撃判定範囲ゲームオブジェクト")]
    [SerializeField] private GameObject attackHitBox;

    [Header("攻撃判定時間")]
    [SerializeField] private float attackDuration = 0.2f;

    [Header("攻撃クールタイム")]
    [SerializeField] private float attackCooldown = 0.5f;

    private bool isAttacking = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(attackHitBox != null)
        {
            attackHitBox.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseManager.IsPaused) return;

        if(Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (!isAttacking)
            {
                StartCoroutine(PerformAttackRoutine());
            }
        }
    }

    //攻撃処理
    private IEnumerator PerformAttackRoutine()
    {
        isAttacking = true;
        Debug.Log("プレイヤーが攻撃した");

        if(attackHitBox != null)
        {
            //プレイヤーと同じレイヤーに書き換える
            attackHitBox.layer = this.gameObject.layer;

            attackHitBox.SetActive(true);
        }

        yield return new WaitForSeconds(attackDuration);

        if(attackHitBox != null)
        {
            attackHitBox.SetActive(false);
        }

        yield return new WaitForSeconds(attackCooldown - attackDuration);

        isAttacking = false;
    }
}
