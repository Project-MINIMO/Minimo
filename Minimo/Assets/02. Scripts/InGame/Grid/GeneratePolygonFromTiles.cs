using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSegment 
{
    public Vector2 Start;
    public Vector2 End;
    public LineSegment(Vector2 s, Vector2 e) { Start = s; End = e; }
}

public class ColliderGenerator 
{
    // 아이소메트릭 타일의 크기 (여기서는 1 x 0.5로 가정)
    private static float TileWidth = 1f;
    private static float TileHeight = 0.5f;

    /// <summary>
    /// 주어진 타일 집합(상대 좌표, Vector2Int)을 이용해 각 타일의 다이아몬드 꼭지점들을 모으고,
    /// 볼록 껍질(Convex Hull)을 계산한 후 건물 로컬 좌표계에 맞게 재중심화한 외곽선(폴리곤)을 반환한다.
    /// </summary>
    public static List<Vector2> GenerateIsoPolygonCentered(HashSet<Vector2Int> tileSet)
    {
        // 1. 각 타일의 다이아몬드 꼭지점들을 모두 모으기
        List<Vector2> vertices = new List<Vector2>();
        foreach (var tile in tileSet)
        {
            Vector2 center = GetIsoCenter(tile);
            // 다이아몬드 꼭지점 (시계방향으로)
            Vector2 top = center + new Vector2(0, TileHeight / 2f);
            Vector2 right = center + new Vector2(TileWidth / 2f, 0);
            Vector2 bottom = center + new Vector2(0, -TileHeight / 2f);
            Vector2 left = center + new Vector2(-TileWidth / 2f, 0);
            vertices.Add(top);
            vertices.Add(right);
            vertices.Add(bottom);
            vertices.Add(left);
        }
        
        // 2. 볼록 껍질(Convex Hull)을 계산 (Monotone Chain 알고리즘)
        List<Vector2> hull = ComputeConvexHull(vertices);
        
        // 3. 타일 집합의 평균 아이소 중심(건물 기준점)을 구함
        Vector2 sum = Vector2.zero;
        foreach (var tile in tileSet)
        {
            sum += GetIsoCenter(tile);
        }
        Vector2 avgCenter = sum / tileSet.Count;
        
        // 4. 재중심화: hull의 모든 점에서 avgCenter를 빼서 건물 로컬 좌표로 변환
        for (int i = 0; i < hull.Count; i++)
        {
            hull[i] -= avgCenter;
        }
        
        return hull;
    }

    /// <summary>
    /// Monotone Chain 알고리즘을 사용하여 볼록 껍질(Convex Hull)을 계산한다.
    /// </summary>
    public static List<Vector2> ComputeConvexHull(List<Vector2> points)
    {
        if (points == null || points.Count <= 1)
            return points;
        
        List<Vector2> sorted = new List<Vector2>(points);
        sorted.Sort((a, b) => {
            if (Mathf.Approximately(a.x, b.x))
                return a.y.CompareTo(b.y);
            return a.x.CompareTo(b.x);
        });
        
        List<Vector2> lower = new List<Vector2>();
        foreach (var p in sorted)
        {
            while (lower.Count >= 2 && Cross(lower[lower.Count - 2], lower[lower.Count - 1], p) <= 0)
                lower.RemoveAt(lower.Count - 1);
            lower.Add(p);
        }
        
        List<Vector2> upper = new List<Vector2>();
        for (int i = sorted.Count - 1; i >= 0; i--)
        {
            Vector2 p = sorted[i];
            while (upper.Count >= 2 && Cross(upper[upper.Count - 2], upper[upper.Count - 1], p) <= 0)
                upper.RemoveAt(upper.Count - 1);
            upper.Add(p);
        }
        
        upper.RemoveAt(upper.Count - 1);
        lower.RemoveAt(lower.Count - 1);
        
        List<Vector2> hull = new List<Vector2>();
        hull.AddRange(lower);
        hull.AddRange(upper);
        return hull;
    }

    static float Cross(Vector2 O, Vector2 A, Vector2 B)
    {
        return (A.x - O.x) * (B.y - O.y) - (A.y - O.y) * (B.x - O.x);
    }

    /// <summary>
    /// 타일 좌표(상대, Vector2Int)를 아이소메트릭 좌표로 변환한다.
    /// 여기서 원하는 결과를 얻기 위해, 단일 타일 (-1, -1)의 경우 center = (0, -0.75)를 만들어야 하므로
    /// 아래와 같이 (TileHeight/2)를 빼준다.
    /// </summary>
    static Vector2 GetIsoCenter(Vector2Int tile)
    {
        float cx = (tile.x - tile.y) * (TileWidth / 2f);
        float cy = (tile.x + tile.y) * (TileHeight / 2f) - (TileHeight / 2f);
        Vector2 center = new Vector2(cx, cy);
        return center;
    }

    // (필요에 따라) Ramer–Douglas–Peucker 알고리즘을 사용하여 다각형을 단순화하는 함수
    public static List<Vector2> SimplifyPolygon(List<Vector2> points, float tolerance)
    {
        if (points == null || points.Count < 3)
            return points;
    
        int index = -1;
        float maxDistance = 0f;
        for (int i = 1; i < points.Count - 1; i++)
        {
            float distance = PerpendicularDistance(points[i], points[0], points[points.Count - 1]);
            if (distance > maxDistance)
            {
                index = i;
                maxDistance = distance;
            }
        }
    
        if (maxDistance > tolerance)
        {
            List<Vector2> leftPart = SimplifyPolygon(points.GetRange(0, index + 1), tolerance);
            List<Vector2> rightPart = SimplifyPolygon(points.GetRange(index, points.Count - index), tolerance);
            List<Vector2> result = new List<Vector2>(leftPart);
            result.RemoveAt(result.Count - 1);
            result.AddRange(rightPart);
            return result;
        }
        else
        {
            return new List<Vector2> { points[0], points[points.Count - 1] };
        }
    }
    
    private static float PerpendicularDistance(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
    {
        float area = Mathf.Abs(0.5f * (lineStart.x * lineEnd.y + lineEnd.x * point.y + point.x * lineStart.y 
                                       - lineEnd.x * lineStart.y - point.x * lineEnd.y - lineStart.x * point.y));
        float bottom = Vector2.Distance(lineStart, lineEnd);
        float height = (bottom == 0) ? 0 : area * 2f / bottom;
        return height;
    }
}
