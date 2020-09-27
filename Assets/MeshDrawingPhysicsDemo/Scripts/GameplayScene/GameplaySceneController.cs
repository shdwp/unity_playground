using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RollingDownDemo.Scripts.GameplayScene
{
    /// <summary>
    /// Scene controller for gameplay scene - general transition logic
    /// </summary>
    public class GameplaySceneController: MonoBehaviour
    {
        public GameObject playerObject;
        public Text levelCompletedText;
        public Button restartButton;

        // stored player mesh, used to set up again after scene reload due to restarts
        private Mesh _playerMesh;
        
        /// <summary>
        /// Setup player mesh
        /// </summary>
        /// <param name="mesh">Mesh to use</param>
        public void SetupPlayerMesh(Mesh mesh)
        {
            var filter = playerObject.GetComponent<MeshFilter>();
            var collider = playerObject.GetComponent<MeshCollider>();

            filter.sharedMesh = mesh;
            collider.sharedMesh = mesh;
            _playerMesh = mesh;
        }

        /// <summary>
        /// Level completed
        /// </summary>
        public void LevelCompleted()
        {
            levelCompletedText.enabled = true;
        }

        private void Awake()
        {
            // set something in the _playerMesh so that restart will actually work
            // when scene has been started from the editor
            _playerMesh = playerObject.GetComponent<MeshFilter>().sharedMesh;
        }

        private void Start()
        {
            restartButton.onClick.AddListener(RestartAction);
        }

        /// <summary>
        /// Level restart which reloads scene
        /// </summary>
        private void RestartAction()
        {
            SceneManager.LoadSceneAsync("MeshDrawingGameplayScene").completed += (_) =>
            {
                // set previous scene mesh to the new scene instance
                var controller = FindObjectOfType<GameplaySceneController>();
                controller.SetupPlayerMesh(_playerMesh);
            };
        }
    }
}