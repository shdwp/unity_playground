using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.Interfaces;
using strange.extensions.command.impl;
using UnityEngine.SceneManagement;

namespace BlockGame.Scripts.Controllers.FromView
{
    /// <summary>
    /// Command that saves current state of the game to persistent storage
    /// </summary>
    public class StoreStateCommand: Command
    {
        [Inject] public IIGamePersistentState persistentState { get; set; }
        [Inject] public IGameFieldState state { get; set; }
        [Inject] public IGridSpawner<CellDataModel> spawner { get; set; }
        
        public override void Execute()
        {
            persistentState.StoreState(state, spawner.type);
        }
    }
}