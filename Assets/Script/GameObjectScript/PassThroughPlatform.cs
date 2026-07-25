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

    //イベントの購読登録
    private void OnEnable()
    {
        PlayerAttributeController.OnAttributeChanged += HandleAttributeChanged;
    }

    //イベントの購読解除
    private void OnDisable()
    {
        PlayerAttributeController.OnAttributeChanged -= HandleAttributeChanged;
    }

    //プレイヤーの属性が変更された時に当たり判定を更新
    private void HandleAttributeChanged(GameObject player)
    {
        Collider2D playerCollider = player.GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            UpdateCollisionState(playerCollider);
        }
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            UpdateCollisionState(other);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            UpdateCollisionState(collision.collider);
        }
    }

    //レイヤーすり抜け判定
    private void UpdateCollisionState(Collider2D playerCollider)
    {
        //レイヤー同士がすり抜けるならtrue
        bool isLayerIgnored = Physics2D.GetIgnoreLayerCollision(playerCollider.gameObject.layer, gameObject.layer);
        //true→判定無効化    false→判定有効化
        Physics2D.IgnoreCollision(playerCollider, platformCollider, isLayerIgnored);
    }

    private IEnumerator DisableCollisionRoutine(Collider2D playerCollider)
    {
        //プレイヤーと足場の当たり判定を無効化
        Physics2D.IgnoreCollision(playerCollider, platformCollider, true);

        yield return new WaitForSeconds(0.5f);

        //再度当たり判定確認
        UpdateCollisionState(playerCollider);
    }
}
