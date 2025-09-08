using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class StarTester : MonoBehaviour
{
    [SerializeField] private Material _customMaterial;
    [SerializeField] private Material _baseMaterial;
    [SerializeField] private Sprite[] _starSprites;
    
    private float _starScale = 0.3f;
    public float GetScale() => _starScale;
    public void SetScale(float scale) => _starScale = scale;
    
    private Color _lineColor = Color.white;
    public Color GetLineColor() => _lineColor;
    public void SetLineColor(Color c) => _lineColor = c;
    
    private float _lineWidth = 1f;
    public float GetLineWidth() => _lineWidth;
    public void SetLineWidth(float w) => _lineWidth = w;
    
    private bool _useCustomMaterial = false;
    public bool  GetUseCustomMaterial() => _useCustomMaterial;
    public void  SetUseCustomMaterial(bool v) => _useCustomMaterial = v;
    
    private Sprite _lineSprite;
    public Sprite GetLineSprite() => _lineSprite;
    public void SetLineSprite(Sprite s) => _lineSprite = s;
    
    private bool _rotateClockwise = true;
    public bool  GetRotateClockwise() => _rotateClockwise;
    public void  SetRotateClockwise(bool v) => _rotateClockwise = v;
    
    private float _rotateSpeed = 0f;
    public float GetRotateSpeed() => _rotateSpeed;
    public void  SetRotateSpeed(float v) => _rotateSpeed = Mathf.Max(0f, v);
    
    private bool _isForeground;
    public bool GetIsForeground() => _isForeground;
    public void SetIsForeground(bool v)
    {
        _isForeground = v;
        ApplyLayerSettings();
    }
    
    private StarManager _starManager;

    private void Start()
    {
        _starManager = GetComponent<StarManager>();
        _starScale = Star.StandardScale;
        
        var validSprites = _starSprites.Where(s => s != null).ToArray();
        if (validSprites.Length == 0)
        {
            Debug.LogWarning("유효한 스프라이트가 없습니다.");
            return;
        }
        Star._sprites = _starSprites;
    }

    private void Update()
    {
        if (_rotateSpeed <= 0f) return;

        var dir = _rotateClockwise ? -1f : 1f;
        transform.Rotate(0f, 0f, dir * _rotateSpeed * Time.deltaTime);
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

        Star._sprites = validSprites;
    }
    
    public void ApplyScale(float scale)
    {
        Star.StandardScale = scale;
    }
    
    public void ApplyLineColor()
    {
        LineObject.LineColor = _lineColor;
        
        foreach (var line in _starManager.Lines)
        {
            if (line == null) continue;
            line.ApplyLineColorMultiplier(_lineColor);
        }
    }
    
    public void ApplyLineWidth()
    {
        LineObject.LineWidth = _lineWidth;
        
        foreach (var line in _starManager.Lines)
        {
            if (line == null) continue;
            line.ApplyLineWidth(_lineWidth);
        }
    }
    
    public void ApplyLineMaterialMode()
    {
        var matToApply = _useCustomMaterial ? _customMaterial : _baseMaterial;
        
        foreach (var line in _starManager.Lines)
        {
            if (line == null) continue;
            line.ApplyLineMaterial(matToApply);
        }
    }

    public void ApplyLineSpriteToMaterial()
    {
        if (!_useCustomMaterial) return;
        
        var tex = _lineSprite ? _lineSprite.texture : null;
        
        if (_customMaterial.HasProperty("_BaseMap"))
            _customMaterial.SetTexture("_BaseMap", tex);
        if (_customMaterial.HasProperty("_MainTex"))
            _customMaterial.SetTexture("_MainTex", tex);

        if (tex != null) tex.wrapMode = TextureWrapMode.Repeat;
    }
    
    public void ApplyLayerSettings()
    {
        if (_starManager == null) return;

        var layerName = _isForeground ? "Village" : "Default";
        var order = _isForeground ? -1 : 1000;
        
        Star.IsForeground = _isForeground;
        LineObject.IsForeground = _isForeground;
        
        foreach (var star in _starManager.Stars)
        {
            if (star == null) continue;
            var sr = star.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingLayerName = layerName;
                sr.sortingOrder = order;
            }
        }

        foreach (var line in _starManager.Lines)
        {
            if (line == null) continue;
            var lr = line.GetComponent<LineRenderer>();
            if (lr != null)
            {
                lr.sortingLayerName = layerName;
                lr.sortingOrder = order;
            }
        }
    }
}
