using strange.extensions.command.impl;
using UnityEngine.SceneManagement;

namespace BlockGame.Scripts.Controllers.FromView
{
    /// <summary>
    /// Command to transition application to menu scene
    /// </summary>
    public class TransitionToMenuCommand: Command
    {
        public override void Execute()
        {
            SceneManager.LoadSceneAsync("BlockGame/Scenes/BlockGameMenuScene");
        }
    }
}