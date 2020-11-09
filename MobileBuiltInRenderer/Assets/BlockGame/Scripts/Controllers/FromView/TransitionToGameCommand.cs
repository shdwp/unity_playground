using BlockGame.Scripts.Contexts;
using BlockGame.Scripts.Model.Interfaces;
using strange.extensions.command.impl;
using UnityEngine.SceneManagement;

namespace BlockGame.Scripts.Controllers.FromView
{
    /// <summary>
    /// Command that transitions application to game scene.
    /// It accepts two parameters - bool `clearState` (whether the persistent state should be clear prior to the move),
    /// and `spawnerType` (which grid spawner type will be used if state was cleared; ignored if `clearState` is false).
    /// </summary>
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
                persistentState.StoreSpawnerType(spawnerType);
            }
            
            SceneManager.LoadSceneAsync("BlockGame/Scenes/BlockGameScene");
        }
    }
}