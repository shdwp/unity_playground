using BlockGame.Scripts.Contexts;

namespace BlockGame.Scripts.Model.Interfaces
{
    /// <summary>
    /// Class that handles persistency - both loading and storing state from persistent storage
    /// </summary>
    public interface IIGamePersistentState
    {
        /// <summary>
        /// Spawner type
        /// </summary>
        GridSpawnerType spawnerType { get; set; }
        
        /// <summary>
        /// Whether there is a previous session that user can continue
        /// </summary>
        bool canContinue { get; }

        /// <summary>
        /// Save game state into persistent storage
        /// </summary>
        /// <param name="state"></param>
        /// <param name="spawnerType"></param>
        void StoreState(IGameFieldState state, GridSpawnerType spawnerType);

        /// <summary>
        /// Store spawner type
        /// </summary>
        /// <param name="type"></param>
        void StoreSpawnerType(GridSpawnerType type);
        
        /// <summary>
        /// Restore game state from data in persistent storage
        /// </summary>
        /// <param name="state"></param>
        void RestoreState(IGameFieldState state);
        
        /// <summary>
        /// Clear any state from persistent storage so that game may start anew
        /// </summary>
        void ClearState();
    }
}