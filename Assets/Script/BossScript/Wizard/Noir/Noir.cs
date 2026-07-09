using UnityEngine;
using System.Collections;

public class Noir : BaseBossWizard
{
    [Header("変化技Object")]
    [SerializeField] private GameObject areaAttackObject;

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

        yield return new WaitForSeconds(2.5f);
        isActionRunning = false;
    }
}
