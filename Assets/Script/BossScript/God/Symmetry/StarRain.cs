using UnityEngine;

public class StarRain : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private float fallSpeed = 7.0f; // 落下速度
    [SerializeField] private float lifeTime = 5.0f;  // 生存時間(秒)

    private void Start()
    {
        // 一定時間後に自動消滅
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // 下方向へ直線移動
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // プレイヤーまたは地面(Ground)に接触した際に削除
        if (collision.CompareTag("Player") || collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}