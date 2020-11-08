using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.Interfaces;
using strange.extensions.command.impl;
using UnityEngine.SceneManagement;

namespace BlockGame.Scripts.Controllers.FromView
{
    public class StoreStateCommand: Command
    {
        [Inject] public IIGamePersistentState persistentState { get; set; }
        [Inject] public IGameState state { get; set; }
        [Inject] public IGridSpawner<BlockDataModel> spawner { get; set; }
        
        public override void Execute()
        {
            persistentState.StoreState(state, spawner.type);
        }
    }
}