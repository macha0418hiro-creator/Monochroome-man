using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class Blanc : BaseBossWizard
{
    [Header("変化技Object")]
    [SerializeField] private GameObject areaAttackObject;

    [Header("専用技Object")]
    [SerializeField] private GameObject residualBeamObject;
    [SerializeField] private GameObject beamFloorObject;

    [SerializeField] private float floorOffset = 0f;

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
            float actualFloorY = floorY + floorOffset;
            
            Vector3 bossPos = transform.position;

            //ビームの最終的な中心点(Y座標)をはじめから計算しておく
            float finalBeamCenterY = (bossPos.y + actualFloorY) / 2f;
            //ビームが完全に伸びきったときの最大の長さ(高さの差分)
            float totalBeamLength = bossPos.y - actualFloorY;

            GameObject beamObj = Instantiate(residualBeamObject, bossPos, Quaternion.identity);

            Transform bossSensor = this.transform.Find("DamageSensor");
            beamObj.layer = bossSensor.gameObject.layer;

            //画像サイズを正確にとるための処理
            float spriteHeight = 1f;
            SpriteRenderer beamSR = beamObj.GetComponent<SpriteRenderer>();
            if (beamSR != null && beamSR.sprite != null)
            {
                spriteHeight = beamSR.sprite.bounds.size.y;
            }

            float targetScaleY = totalBeamLength / spriteHeight;

            Vector3 originalScale = beamObj.transform.localScale;
            beamObj.transform.localScale = new Vector3(originalScale.x, 0f, originalScale.z);

            Collider2D beamCollider = beamObj.GetComponent<Collider2D>();
            if (beamCollider != null) beamCollider.enabled = true;

            //ビームを伸ばす(予兆から本番へ)
            float duration = 0.5f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                if (beamObj != null)
                {
                    beamObj.transform.localScale = new Vector3(originalScale.x, targetScaleY * progress, originalScale.z);

                    float currentCenterY = Mathf.Lerp(bossPos.y, finalBeamCenterY, progress);
                    beamObj.transform.position = new Vector3(bossPos.x, currentCenterY, bossPos.z);
                }
                yield return null;
            }

            //完全に伸びきったらビームの位置を固定
            if (beamObj != null)
            {
                beamObj.transform.position = new Vector3(bossPos.x, finalBeamCenterY, bossPos.z);
                beamObj.transform.localScale = new Vector3(originalScale.x, targetScaleY, originalScale.z);
            }

            if(beamFloorObject != null)
            {
                Vector3 floorPosition = new Vector3(bossPos.x, actualFloorY, 0f);

                GameObject floorFireObj = Instantiate(beamFloorObject, floorPosition, Quaternion.identity);
                floorFireObj.layer = bossSensor.gameObject.layer;
            }

            // ビームが接地して燃え続ける時間を待つ
            yield return new WaitForSeconds(2.0f);
        }

        yield return new WaitForSeconds(2.5f);
        isActionRunning = false;
    }
}