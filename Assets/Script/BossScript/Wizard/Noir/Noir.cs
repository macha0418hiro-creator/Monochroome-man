using UnityEngine;
using System.Collections;

public class Noir : BaseBossWizard
{
    [Header("変化技Object")]
    [SerializeField] private GameObject areaAttackObject;

    [Header("専用技Object")]
    [SerializeField] private GameObject explosiveBulletObject;

    protected override IEnumerator CustomAttackWithVariation()
    {
        Debug.Log("Noir変化技");

        GameObject attackObj = Instantiate(areaAttackObject, transform.position, Quaternion.identity);

        Transform bossSensor = this.transform.Find("DamageSensor");
        attackObj.layer = bossSensor.gameObject.layer;

        AreaAttackEffect effectScript = attackObj.GetComponent<AreaAttackEffect>();
        if (effectScript != null)
        {
            //AreaAttackEffectの処理が終わるまで待機
            yield return StartCoroutine(effectScript.ExecuteAreaAttackRoutine(false));
        }

        yield return new WaitForSeconds(1.2f);
        isActionRunning = false;
    }

    protected override IEnumerator UniqueSpecialAttack()
    {
        Debug.Log("Noir専用技");

        if (explosiveBulletObject != null)
        {
            yield return new WaitForSeconds(1.0f); // 溜め時間

            Quaternion downRotation = Quaternion.Euler(0, 0, -90f);

            GameObject bulletObj = Instantiate(explosiveBulletObject, (transform.position - Vector3.up * 2), downRotation);

            Transform bossSensor = this.transform.Find("DamageSensor");
            bulletObj.layer = bossSensor.gameObject.layer;

            ExplosiveBullet bulletScript = bulletObj.GetComponent<ExplosiveBullet>();
            Rigidbody2D bulletRb = bulletObj.GetComponent<Rigidbody2D>();

            //ExplosiveBulletの処理を一時停止
            if (bulletScript != null && bulletRb != null)
            {
                bulletScript.enabled = false;           // Startによる即時移動を防ぐ
                bulletRb.linearVelocity = Vector2.zero; // 完全に静止
            }

            //ため時間
            float chargeTime = 1.5f;
            yield return new WaitForSeconds(chargeTime);

            //ExplosiveBulletの処理再開
            if (bulletObj != null && bulletScript != null)
            {
                bulletScript.enabled = true;

                // スクリプトのStartが走らない場合を考慮して直接速度を与える
                bulletRb.linearVelocity = bulletObj.transform.right * 8f; 
            }
        }

        yield return new WaitForSeconds(2.5f);
        isActionRunning = false;
    }
}
