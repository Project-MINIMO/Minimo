using UnityEngine;

[RequireComponent(typeof(LineRenderer), typeof(BoxCollider))]
public class LineObject : MonoBehaviour
{
    private LineRenderer _line;
    private BoxCollider _collider;
    private Star _a, _b;
    private StarSpawner _spawner;

    private void Awake()
    {
        _line = GetComponent<LineRenderer>();
        _line.useWorldSpace = true;

        // 렌더링 문제 방지 설정
        _line.alignment = LineAlignment.TransformZ;
        _line.textureMode = LineTextureMode.Tile;
        _line.numCapVertices = 2;
        _line.numCornerVertices = 2;
        _line.widthMultiplier = 1.0f;
        _line.sortingLayerName = "Default";
        _line.sortingOrder = 10;

        _collider = GetComponent<BoxCollider>();
    }

    public void Initialize(Vector3 start, Vector3 end, Color startColor, Color endColor, Star a, Star b, StarSpawner spawner)
    {
        _a = a;
        _b = b;
        _spawner = spawner;

        // 중간점 추가 (세 개의 점)
        Vector3 middle = (start + end) * 0.5f;

        _line.positionCount = 3;
        _line.SetPosition(0, start);
        _line.SetPosition(1, middle);
        _line.SetPosition(2, end);

        // -----------------------------
        // 투명도 및 색상 그라디언트 설정
        // -----------------------------
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(startColor, 0f),
                new GradientColorKey(startColor, 0.5f),
                new GradientColorKey(endColor, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0f, 0f),    // 시작: 투명
                new GradientAlphaKey(1f, 0.5f),  // 중간: 불투명
                new GradientAlphaKey(0f, 1f)     // 끝: 투명
            }
        );
        _line.colorGradient = gradient;

        // -----------------------------
        // 굵기 곡선 설정 (중앙이 더 두껍게)
        // -----------------------------
        _line.widthCurve = new AnimationCurve(
            new Keyframe(0f, 0.001f),
            new Keyframe(0.5f, 0.02f),
            new Keyframe(1f, 0.001f)
        );

        UpdateCollider(start, end);
    }

    private void UpdateCollider(Vector3 start, Vector3 end)
    {
        var direction = end - start;
        var length = direction.magnitude;
        var center = (start + end) * 0.5f;

        // 객체 위치와 회전 설정
        transform.position = center;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

        // 콜라이더 크기 설정 (Y축 기준)
        _collider.size = new Vector3(0.1f, length, 0.1f);
        _collider.center = Vector3.zero;
        _collider.isTrigger = true;
    }

    private void OnMouseDown()
    {
        Debug.Log("Line clicked!");
        _spawner.RemoveLine(this);
    }

    public bool Contains(Star s) => _a == s || _b == s;
}
