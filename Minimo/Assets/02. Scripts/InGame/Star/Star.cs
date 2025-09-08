using System;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class Star : InteractObject
{
    public override bool IsUseDrag => true;
    public Color ActiveColor { get; private set; }
    public static Sprite[] _sprites;
    [SerializeField] private Color[] _colors;
    
    private StarManager _manager;
    private ConstellationPanel _constellationPanel;
    private bool _isSpawned;
    
    private const float ScaleSpeed = 5f;
    private const float MinScale = 0.9f;
    private const float MaxScale = 1.1f;
    private const float StandardScale = 0.3f;
    private const float RotationSpeed = 30;
    private float _timeOffset = -1;

    public static bool IsForeground;
    public static float ScaleMultiplier = 1;

    public void Initialize(StarManager manager, ConstellationPanel constellationPanel)
    {
        _manager = manager;
        _constellationPanel = constellationPanel;
        
        var randomIndex = Random.Range(0, _sprites.Length);
        GetComponent<SpriteRenderer>().sprite = _sprites[randomIndex];
        ActiveColor = _colors[randomIndex];
        transform.DOScale(new Vector3(0.3f, 0.3f, 0.3f), 0.3f).SetEase(Ease.InOutElastic)
            .OnComplete(() =>
            {
                _timeOffset = Random.Range(0f, 100f);
                transform
                    .DORotate(new Vector3(0, 0, 360f), 1f / (RotationSpeed / 360f), RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Restart);
            });
    }

    private void OnEnable()
    {
        var layerName = IsForeground ? "Village" : "Default";
        var order = IsForeground ? -1 : 1000;

        var sr = GetComponent<SpriteRenderer>();
        sr.sortingLayerName = layerName;
        sr.sortingOrder = order;
    }

    private void Update()
    {
        if (_timeOffset < 0) return;
        
        var scale = StandardScale * ScaleMultiplier * Mathf.Lerp(MinScale, MaxScale, (Mathf.Sin(Time.time * ScaleSpeed + _timeOffset) + 1f) / 2f);
        transform.localScale = new Vector3(scale, scale, scale);
    }

    public override void OnDrag()
    {
        _constellationPanel.OpenPanel();
        
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = transform.position.z;
        transform.position = mouseWorldPos;
        
        _manager.UpdateConnections(this, mouseWorldPos);
    }
    
    public override void OnDragEnd()
    {
        _constellationPanel.ClosePanel();
    }
    
    public override void OnLongPress() { }
    
    public override void OnClickUp() { }
}
