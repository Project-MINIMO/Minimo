public enum TaskState
{
    Empty,
    Pending,
    Complete,
    Produce,
}

public enum NotifyType
{
    StorageCapacityLack,
    MinimoCapacityLack,
    
    Success,
    SlotLack,
    InvalidOption,
    MissMinimo,
    MissRecipe,
    MaterialLack,
    
    GoldLack,
    ItemLack,
    
    CannotReplaceTile,
    CannotEraseTile,
    CannotInstallWaterTile,
    DeselectTile,
    
    DeleteBuildingFailProduce,
    DeleteBuildingFailMinimo,
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
    Swim,
    Happy,
    Acquire,
    Idle,
    Work,
    None,
}

public enum StrayMinimoState
{
    Idle,
    Plunder,
    Hide,
    None,
}

public enum VisitMinimoState
{
    Idle,
    Hide,
    None
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

public enum InputTargetType
{
    None,
    UI,
    Object,
    Camera
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