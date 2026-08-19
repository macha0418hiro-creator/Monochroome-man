using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class SemiCircleCollider2D : MonoBehaviour
{
    public enum Side { Left, Right }

    [Header("半円の設定")]
    [SerializeField] private Side side = Side.Left; // 左半分か右半分か
    [SerializeField] private float radius = 1.0f;    // 半径
    [SerializeField] private int segments = 20;      // 円弧のなめらかさ（頂点数）

    private void Awake()
    {
        CreateSemiCircle();
    }

    // Inspectorで値を変更したときにリアルタイム更新
    private void OnValidate()
    {
        CreateSemiCircle();
    }

    private void CreateSemiCircle()
    {
        PolygonCollider2D poly = GetComponent<PolygonCollider2D>();
        Vector2[] points = new Vector2[segments + 2]; // 扇形の頂点リスト

        // 中心点（原点）
        points[0] = Vector2.zero;

        // 左右に応じた角度の範囲を設定
        // 左半円: 90度 〜 270度（π/2 〜 3π/2）
        // 右半円: -90度 〜 90度（-π/2 〜 π/2）
        float startAngle = (side == Side.Left) ? Mathf.PI / 2f : -Mathf.PI / 2f;
        float endAngle = (side == Side.Left) ? 3f * Mathf.PI / 2f : Mathf.PI / 2f;

        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            float angle = Mathf.Lerp(startAngle, endAngle, t);

            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            points[i + 1] = new Vector2(x, y);
        }

        poly.points = points;
    }
}