using UnityEngine;
using UnityEngine.Tilemaps;

public class TileAlphaController : MonoBehaviour
{
    // 인스펙터에서 할당할 Tilemap과 Collider2D 참조
    public Tilemap tilemap;
    public Collider2D areaCollider;

    // 미리 정의한 투명/불투명 색상 (RGB는 그대로 유지하고 알파값만 변경)
    private Color transparentColor = new Color(1f, 1f, 1f, 0f);
    private Color opaqueColor = new Color(1f, 1f, 1f, 1f);

    void Start()
    {
        // 초기 설정: 모든 타일을 투명하게 만들고, 컬라이더 영역 안의 타일은 불투명하게 변경
        UpdateTileAlphas();
    }

    void UpdateTileAlphas()
    {
        // 타일맵 내 사용된 셀 범위를 가져옴
        BoundsInt bounds = tilemap.cellBounds;
        
        // 1. 모든 타일을 투명하게 설정
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                tilemap.SetColor(pos, transparentColor);
            }
        }

        // 2. 각 타일의 world position (중심점)을 구해서, 2D 콜라이더와 겹치는지 체크 후 불투명하게 변경
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                // 타일의 World Space 중앙 좌표 구하기
                Vector3 tileWorldCenter = tilemap.GetCellCenterWorld(pos);
                if (areaCollider.OverlapPoint(tileWorldCenter))
                {
                    tilemap.SetColor(pos, opaqueColor);
                }
            }
        }
    }

    // 만약 카메라나 콜라이더가 움직여서 매 프레임 갱신이 필요하다면, Update() 내에서 UpdateTileAlphas()를 호출하거나
    // 필요한 이벤트에 맞게 호출하면 돼.
    void Update()
    {
        UpdateTileAlphas();
    }
    
}