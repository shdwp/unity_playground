using BlockGame.Scripts.Model.Interfaces;
using strange.extensions.command.impl;
using UnityEngine.SceneManagement;

namespace BlockGame.Scripts.Controllers.FromView
{
    public class TransitionToGameCommand: Command
    {
        [Inject] public bool clearState { get; set; }
        [Inject] public GridSpawnerType spawnerType { get; set; }
        [Inject] public IIGamePersistentState persistentState { get; set; }

        public override void Execute()
        {
            if (clearState)
            {
                persistentState.ClearState();
                persistentState.spawnerType = spawnerType;
            }
            
            SceneManager.LoadSceneAsync("BlockGame/Scenes/BlockGameScene");
        }
    }
}