using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InfiniteScroller : MonoBehaviour
{
    [Tooltip("스크롤할 자식 타일(로컬 좌표로 배치해 두세요)")]
    [SerializeField] private List<Transform> tiles;

    [Tooltip("타일 하나의 가로 폭(스프라이트 Bounds 기준)")]
    [SerializeField] private float tileWidth;

    private Camera cam;
    private float cameraHalfWidth;

    void Start()
    {
        cam = Camera.main;
        if (!cam.orthographic)
            Debug.LogWarning("이 스크립트는 Orthographic 카메라 전용입니다.");

        // 카메라 Orthographic 반경(가로)
        cameraHalfWidth = cam.orthographicSize * cam.aspect;

        // tileWidth가 에디터에 세팅 안 됐다면 첫 타일에서 가져오기
        if (tileWidth <= 0 && tiles.Count > 0)
        {
            var sr = tiles[0].GetComponent<SpriteRenderer>();
            tileWidth = sr.bounds.size.x;
        }
    }

    void Update()
    {
        float leftBoundary = cam.transform.position.x - cameraHalfWidth;
        float rightBoundary = cam.transform.position.x + cameraHalfWidth;

        foreach (var tile in tiles)
        {
            // 타일의 오른쪽(+) 엣지가 왼쪽 경계보다 왼쪽에 완전히 벗어났는지
            if (tile.position.x + tileWidth * 0.5f < leftBoundary)
            {
                // 가장 오른쪽에 있던 타일의 오른쪽 엣지 좌표를 찾고,
                float maxRight = float.MinValue;
                foreach (var t in tiles)
                    maxRight = Mathf.Max(maxRight, t.position.x + tileWidth * 0.5f);

                // 이 타일을 그 위치에서 한 타일만큼 더 오른쪽으로 이동
                float newX = maxRight + tileWidth * 0.5f;
                tile.position = new Vector3(newX, tile.position.y, tile.position.z);
            }

            // (반대 방향 스크롤이 필요하면 여기에 오른쪽 경계 벗어났을 때 처리 로직 추가)
        }
    }
}
