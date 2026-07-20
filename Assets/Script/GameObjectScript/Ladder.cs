using UnityEngine;
using UnityEngine.Tilemaps;

public class Ladder : MonoBehaviour
{
    private Tilemap tilemap;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        tilemap = GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //ハシゴが掴めるか判定
    public bool CanClimb(GameObject playerObject)
    {
        bool isLayerIgnored = Physics2D.GetIgnoreLayerCollision(playerObject.layer, gameObject.layer);
        return !isLayerIgnored;
    }

    //ハシゴの掴む位置を返す処理
    public float GetCenterX(Vector3 playerPos)
    {
        if(tilemap != null)
        {
            Vector3Int cellPos = tilemap.WorldToCell(playerPos);        //Tilemapのマス目でプレイヤー座標をとる
            Vector3 cellCenter = tilemap.GetCellCenterWorld(cellPos);   //マス目の中心のゲーム座標を取得

            return cellCenter.x;
        }

        return transform.position.x;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var handler = collision.GetComponent<PlayerLadderHandler>();
            if(handler != null)
            {
                handler.SetNearLadder(this, true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var handler = collision.GetComponent<PlayerLadderHandler>();
            if (handler != null)
            {
                handler.SetNearLadder(this, false);
            }
        }
    }
}
