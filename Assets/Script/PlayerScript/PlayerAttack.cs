using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("攻撃力")]
    [SerializeField] private int power = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHealth enemy = collision.gameObject.GetComponent<EnemyHealth>();

        if(enemy != null)
        {
            enemy.TakeDamage(power);
            Destroy(gameObject);
        }
    }
}
