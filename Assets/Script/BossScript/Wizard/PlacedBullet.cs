using UnityEngine;
using System.Collections;

public class PlacedBullet : MonoBehaviour
{
    [Header("移動速度")]
    [SerializeField] private float speed = 8f;

    private bool isLaunched = false;    //設置済みか判定
    private Vector2 targetDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    //弾を配置する処理
    public void Launch(Vector3 playerPosition)
    {
        //プレイヤーの方向を記録
        targetDirection = (playerPosition - transform.position).normalized;

        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        isLaunched = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isLaunched)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
