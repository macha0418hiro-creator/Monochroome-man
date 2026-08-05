using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] private float destroyDelay = 3f; //アニメーションの長さに合わせる
    void Start() => Destroy(gameObject, destroyDelay);
}