using UnityEngine;
using System.Collections;

public class Blanc : BaseBossWizard
{
    [Header("変化技Object")]
    [SerializeField] private GameObject areaAttackObject;

    [Header("専用技Object")]
    [SerializeField] private GameObject residualBeamObject;

    protected override IEnumerator CustomAttackWithVariation()
    {
        Debug.Log("Blanc変化技");

        GameObject attackObj = Instantiate(areaAttackObject, transform.position, Quaternion.identity);

        Transform bossSensor = this.transform.Find("DamageSensor");
        attackObj.layer = bossSensor.gameObject.layer;

        AreaAttackEffect effectScript = attackObj.GetComponent<AreaAttackEffect>();
        if (effectScript != null)
        {
            //AreaAttackEffectの処理が終わるまで待機
            yield return StartCoroutine(effectScript.ExecuteAreaAttackRoutine(true));
        }

        yield return new WaitForSeconds(1.2f);
        isActionRunning = false;
    }

    protected override IEnumerator UniqueSpecialAttack()
    {
        Debug.Log("Blanc専用技");
        
        if (residualBeamObject != null)
        {
            yield return new WaitForSeconds(0.8f); // 溜め時間

            float floorY = 9f; //ステージの床のY座標
            float spawnY = (transform.position.y + floorY) / 2f;    //ビームを伸ばすための中間地点
            Vector3 targetFloorPosition = new Vector3(transform.position.x, floorY, 0f);

            GameObject beamFloorObj = Instantiate(residualBeamObject, targetFloorPosition, Quaternion.identity);

            Transform bossSensor = this.transform.Find("DamageSensor");
            beamFloorObj.layer = bossSensor.gameObject.layer;

            Vector3 originalScale = beamFloorObj.transform.localScale;
            beamFloorObj.transform.localScale = new Vector3(originalScale.x, 0f, originalScale.z);

            Collider2D beamCollider = beamFloorObj.GetComponent<Collider2D>();
            if (beamCollider != null) beamCollider.enabled = false;

            //ビームを伸ばす(予兆から本番へ)
            float duration = 0.5f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                if (beamFloorObj != null)
                {
                    beamFloorObj.transform.localScale = new Vector3(originalScale.x, originalScale.y * progress, originalScale.z);
                }
                yield return null;
            }

            //完全に伸びきったら、ダメージ判定
            if (beamFloorObj != null && beamCollider != null)
            {
                beamCollider.enabled = true;
            }

            // ビームが接地して燃え続ける時間を待つ
            yield return new WaitForSeconds(2.0f);
        }

        yield return new WaitForSeconds(2.5f);
        isActionRunning = false;
    }
}