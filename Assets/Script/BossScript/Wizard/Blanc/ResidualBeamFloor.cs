using UnityEngine;

public class ResidualBeamFloor : MonoBehaviour
{
    [SerializeField] private float duration = 3.0f; //床が燃えている時間

    void Start()
    {
        Destroy(gameObject, duration);
    }

    void Update()
    {
        
    }
}
