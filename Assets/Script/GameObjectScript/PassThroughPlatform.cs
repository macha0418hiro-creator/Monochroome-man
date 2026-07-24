using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PassThroughPlatform : MonoBehaviour
{
    private Collider2D platformCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        platformCollider = GetComponent<CompositeCollider2D>();
    }

    // Update is called once per frame
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(Keyboard.current != null &&
              (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed))
            {
                //一時的にプレイヤーと足場との当たり判定を無効化する
                StartCoroutine(DisableCollisionRoutine(collision.collider));
            }
        }
    }

    private IEnumerator DisableCollisionRoutine(Collider2D playerCollider)
    {
        //プレイヤーと足場の当たり判定を無効化
        Physics2D.IgnoreCollision(playerCollider, platformCollider, true);

        yield return new WaitForSeconds(0.5f);

        //当たり判定を戻す
        Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
    }
}
