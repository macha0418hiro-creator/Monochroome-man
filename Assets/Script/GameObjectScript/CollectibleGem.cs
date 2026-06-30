using Unity.VisualScripting;
using UnityEngine;

public class CollectibleGem : MonoBehaviour
{
    [Header("宝石が属するステージ")]
    [SerializeField] private int stageNumber;

    public enum GemType { White, Black }
    [Header("宝石の種類")]
    [SerializeField] private GemType gemType;

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
        if (collision.CompareTag("Player"))
        {
            Collect();
        }
    }

    private void Collect()
    {
        string saveKey = $"Stage_{stageNumber}_Gem_{gemType}_Collected";

        PlayerPrefs.SetInt(saveKey, 1);
        PlayerPrefs.Save();

        Debug.Log($"ステージ{stageNumber}の{gemType}の宝石をゲット");

        Destroy(gameObject);
    }
}
