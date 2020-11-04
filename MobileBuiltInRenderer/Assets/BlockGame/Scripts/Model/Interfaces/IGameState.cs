namespace BlockGame.Scripts.Model.Interfaces
{
    public enum BlockColor
    {
        Red,
        Green,
        Yellow,
        Orange,
        Blue,
        Cyan,
        Purple,
    }

    public enum GridType
    {
        Attached,
        Detached
    }
        
    public interface IGameState
    {
        IPartialGrid attachedGrid { get; set; }
        IPartialGrid detachedGrid { get; set; }

        bool TestAndApplyDetachedGridCollisions();
    }
}