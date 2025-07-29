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
    
    GoldLack,
    
    CannotEraseTile,
    DeselectTile,
}

public enum QuestState
{
    Completed,
    InProgress,
    Locked,
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
    Work,
    Hide,
    None,
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

public enum PopUpType
{
    MinimoAssign,
    MinimoUnassign,
    MinimoShift,
    StorageExpand,
    StorageExpandResult,
    MinimoExpand,
    MinimoExpandResult,
}