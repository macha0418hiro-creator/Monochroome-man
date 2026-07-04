using UnityEngine;
using System.Collections;

public class Blanc : BaseBossWizard
{
    protected override IEnumerator CustomAttackWithVariation()
    {
        Debug.Log("Blanc変化技");

        yield return new WaitForSeconds(1.2f);
        isActionRunning = false;
    }

    protected override IEnumerator UniqueSpecialAttack()
    {
        Debug.Log("Blanc専用技");

        yield return new WaitForSeconds(2.5f);
        isActionRunning = false;
    }
}