using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class StarTester : MonoBehaviour
{
    [SerializeField] private Sprite[] _starSprites;
    
    private float _starScale = 0.3f;
    
    private StarManager _starManager;

    private void Start()
    {
        _starManager = GetComponent<StarManager>();
        _starScale = 0.3f;
    }

    public void ApplySprites()
    {
        if (_starSprites == null || _starSprites.Length == 0)
        {
            Debug.LogWarning("StarSprites가 비어 있습니다.");
            return;
        }
        
        var validSprites = _starSprites.Where(s => s != null).ToArray();
        if (validSprites.Length == 0)
        {
            Debug.LogWarning("유효한 스프라이트가 없습니다.");
            return;
        }
        
        foreach (var star in _starManager.Stars)
        {
            if (star == null) continue;
            var sr = star.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = validSprites[Random.Range(0, validSprites.Length)];
            }
        }
    }
    
    public void ApplyScale(float scale)
    {
        if (_starManager == null) return;

        foreach (var star in _starManager.Stars)
        {
            if (star == null) continue;
            star.StandardScale = scale;
        }
    }

    public float GetScale() => _starScale;
    public void SetScale(float scale) => _starScale = scale;
}
