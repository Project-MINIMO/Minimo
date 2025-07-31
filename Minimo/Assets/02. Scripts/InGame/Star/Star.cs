using UnityEngine;

public class Star : InteractObject
{
    [SerializeField] private Sprite[] _sprites;

    private StarSpawner _spawner;
    private ConstellationPanel _constellationPanel;

    public void Initialize(StarSpawner spawner, ConstellationPanel constellationPanel)
    {
        _spawner = spawner;
        _constellationPanel = constellationPanel;
        
        GetComponent<SpriteRenderer>().sprite = _sprites[Random.Range(0, _sprites.Length)];
    }

    public override void OnDrag()
    {
        _constellationPanel.OpenPanel();
        
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = transform.position.z;
        transform.position = mouseWorldPos;
        
        _spawner.UpdateConnections(this, mouseWorldPos);
    }
    
    public override void OnDragEnd()
    {
        _constellationPanel.ClosePanel();
    }
    
    public override void OnLongPress() { }
    
    public override void OnClickUp() { }
}
