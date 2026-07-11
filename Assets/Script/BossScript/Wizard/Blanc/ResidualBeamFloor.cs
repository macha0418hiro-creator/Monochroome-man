using UnityEngine;

public class ResidualBeamFloor : MonoBehaviour
{
    [SerializeField] private float duration = 3.0f; //床が燃えている時間

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, duration);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
