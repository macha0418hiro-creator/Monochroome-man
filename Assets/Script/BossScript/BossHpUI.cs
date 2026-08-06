using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHpUI : MonoBehaviour
{
    [Header("連動させるボスオブジェクト")]
    [SerializeField] private EnemyHealth targetBoss;

    [Header("表示設定")]
    [SerializeField] private string bossName = "ボス";

    [Header("UIコンポーネント")]
    [SerializeField] private GameObject hpPanel;     // BossHpPanel全体
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TMP_Text bossNameText;

    private void Awake()
    {
        //ゲーム開始時はUIを隠しておく
        if (hpPanel != null) hpPanel.SetActive(false);
    }

    private void OnEnable()
    {
        RegisterBossEvents();
    }

    private void OnDisable()
    {
        UnregisterBossEvents();
    }

    private void RegisterBossEvents()
    {
        if (targetBoss != null)
        {
            //出現・ダメージ・死亡のイベントを監視
            targetBoss.OnSpawned += HandleSpawned;
            targetBoss.OnHpChanged += HandleHpChanged;
            targetBoss.OnDied += HandleDied;

            //すでにボスがアクティブな状態でシーン開始された場合のケア
            if (targetBoss.gameObject.activeInHierarchy)
            {
                SetupUI();
            }
        }
    }

    private void UnregisterBossEvents()
    {
        if (targetBoss != null)
        {
            targetBoss.OnSpawned -= HandleSpawned;
            targetBoss.OnHpChanged -= HandleHpChanged;
            targetBoss.OnDied -= HandleDied;
        }
    }

    //ボスが出現した時の処理
    private void HandleSpawned()
    {
        SetupUI();
    }

    private void SetupUI()
    {
        if (hpPanel != null) hpPanel.SetActive(true);
        if (bossNameText != null) bossNameText.text = bossName;

        if (hpSlider != null)
        {
            hpSlider.maxValue = targetBoss.MaxHp;
            hpSlider.value = targetBoss.CurrentHp;
        }
    }

    private void HandleHpChanged(int newHp)
    {
        if (hpSlider != null)
        {
            hpSlider.value = newHp;
        }
    }

    private void HandleDied()
    {
        if (hpPanel != null)
        {
            hpPanel.SetActive(false);
        }
    }
}