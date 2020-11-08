namespace BlockGame.Scripts.Model.Interfaces
{
    public interface IIGamePersistentState
    {
        GridSpawnerType spawnerType { get; set; }
        bool canContinue { get; }

        void StoreSpawnerType(GridSpawnerType type);
        void StoreState(IGameState state, GridSpawnerType spawnerType);
        void RestoreState(IGameState state);
        void ClearState();
    }
}