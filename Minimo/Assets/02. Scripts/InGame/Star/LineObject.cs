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

        _collider = GetComponent<BoxCollider>();
    }

    public void Initialize(Vector3 start, Vector3 end, Color startColor, Color endColor, Star a, Star b, StarSpawner spawner)
    {
        _a = a;
        _b = b;
        _spawner = spawner;

        _line.SetPosition(0, start);
        _line.SetPosition(1, end);
        _line.startColor = startColor;
        _line.endColor = endColor;

        UpdateCollider(start, end);
    }

    private void UpdateCollider(Vector3 start, Vector3 end)
    {
        var direction = end - start;
        var length = direction.magnitude;
        var center = (start + end) / 2f;

        // 객체 자체의 위치와 회전을 선에 맞춤
        transform.position = center;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

        // BoxCollider는 로컬 공간 기준으로 설정됨
        _collider.size = new Vector3(0.1f, length, 0.1f); // Y축 방향으로 길게
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