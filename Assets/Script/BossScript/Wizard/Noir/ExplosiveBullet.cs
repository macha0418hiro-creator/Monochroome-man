using UnityEngine;

public class ExplosiveBullet : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private GameObject explosionEffectObject;

    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if((collision.CompareTag("Player") || collision.CompareTag("Ground")))
        {
            Explode();
        }
    }

    private void Explode()
    {
        if(explosionEffectObject != null)
        {
            GameObject explosion = Instantiate(explosionEffectObject, transform.position, Quaternion.identity);
            explosion.layer = this.gameObject.layer;
        }

        Destroy(gameObject);
    }
}
