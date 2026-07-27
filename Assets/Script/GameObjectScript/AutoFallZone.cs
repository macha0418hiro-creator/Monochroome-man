using UnityEngine;

public class AutoFallZone : MonoBehaviour
{
    [Header("参照する背景画像")]
    [SerializeField] private SpriteRenderer boundsBackground;

    [Header("背景の底からのマージン")]
    [SerializeField] private float fallOffset = 1.0f;

    [Header("与えるダメージ量")]
    [SerializeField] private int fallDamage = 1;

    [Header("監視対象のプレイヤー")]
    [SerializeField] private Transform playerTransform;

    private void Update()
    {
        if (boundsBackground == null || playerTransform == null) return;

        //背景画像の「一番下のY座標」を取得し、少し下に判定ラインを設定
        float fallThresholdY = boundsBackground.bounds.min.y - fallOffset;

        //プレイヤーのY座標が判定ラインを下回ったら転落
        if (playerTransform.position.y < fallThresholdY)
        {
            TriggerFallDamage();
        }
    }

    private void TriggerFallDamage()
    {
        //プレイヤーの各種スクリプトを取得
        PlayerContoroller playerController = playerTransform.GetComponent<PlayerContoroller>();
        PlayerHealth playerHealth = playerTransform.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(fallDamage);
        }
        
        Debug.Log($"背景範囲外へ転落！ {fallDamage} のダメージ");

        //復帰処理
        if (playerController != null)
        {
            playerController.RespawnToLastGround();
        }
    }

    //開発中の確認用(落下判定ラインを赤色で表示)
    private void OnDrawGizmosSelected()
    {
        if (boundsBackground != null)
        {
            Gizmos.color = Color.red;
            float fallY = boundsBackground.bounds.min.y - fallOffset;
            Vector3 start = new Vector3(boundsBackground.bounds.min.x - 10f, fallY, 0f);
            Vector3 end = new Vector3(boundsBackground.bounds.max.x + 10f, fallY, 0f);
            Gizmos.DrawLine(start, end);
        }
    }
}