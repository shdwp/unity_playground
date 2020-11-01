using System;
using RollingDownDemo.Scripts.GameplayScene;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RollingDownDemo.Scripts.DrawScene
{
    /// <summary>
    /// General drawing scene controller - implements transition logic
    /// </summary>
    public class DrawSceneController: MonoBehaviour
    {
        public UserDrawingController drawingController;
        
        public Button proceedButton;
        public Button resetButton;

        private void Start()
        {
            proceedButton.onClick.AddListener(ProceedAction);
            resetButton.onClick.AddListener(ResetAction);
        }

        private void ProceedAction()
        {
            // get mesh to be passed after scene loading
            var mesh = drawingController.mesh;

            // load gameplay scene and wait for completion
            SceneManager.LoadSceneAsync("MeshDrawingGameplayScene").completed += (_) =>
            {
                var controller = FindObjectOfType<GameplaySceneController>();
                
                // pass the drawn mesh to respective scene controller
                controller.SetupPlayerMesh(mesh);
            };
        }

        private void ResetAction()
        {
            drawingController.Reset();
        }
    }
}