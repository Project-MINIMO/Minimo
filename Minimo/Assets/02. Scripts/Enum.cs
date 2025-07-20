public enum TaskState
{
    Empty,
    Pending,
    Complete,
    Produce,
}

public enum NotifyType
{
    CapacityLack,
    
    Success,
    SlotLack,
    InvalidOption,
    MissMinimo,
    MissRecipe,
    MaterialLack,
    
    GoldLack
}

public enum UseCashType
{
    Produce,
    ProduceExpand,
    ProduceMaterial,
}

public enum MinimoState
{
    Idle,
    Walk,
    Work,
    Drag
}

public enum ResourceType
{
    Resource,
    SpecialResource
}

public enum ItemType
{
    Food,
    Flower,
    Amulet,
    Etc,
}

public enum InputState
{
    None,      
    Drag,  
    DragEnd,
    ClickDown,
    ClickUp,   
    LongPress,
    Zoom
}


public enum ItemProperty
{
    None,
    Love,
    Friendship,
    Memory,
    Peace,
    Hope,
    Courage
}

public enum TileState
{
    Empty,
    Installed
}

public enum TileType
{
    Ground,
    Water
}

public enum ProduceState
{
    Idle,
    Produce,
    Complete,
}