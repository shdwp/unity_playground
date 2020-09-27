using System;
using ARDemo.Scripts.ARTracking;
using ARDemo.Scripts.Game;
using maxstAR;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

namespace ARDemo.Scripts
{
    // @TODO: split into two separate controllers - one for state flow and other for UI bindings
    public class GameFlowController : MonoBehaviour
    {
        [Header("UI bindings")]
        public Text statusText;
        public Button startButton;
        public Button restartButton;
        public Button pauseButton;
        public Text pauseButtonText;
        public Text resultsText;

        [Header("World")]
        public Camera playerCamera;
        public Transform trackableWorld;
        public GameObject _pref;

        [Header("DI")]
        public GameModelController gameModel;
        public ImageTrackerController tracker;

        private enum GameState
        {
            WaitingForTracking,
            WaitingForUserToStart,
            PausedByUser,
            PausedByTracking,
            PausedByUserAndTracking,
            InProgress,
            Finished,
        }

        private GameState _state = GameState.WaitingForTracking;

        private void Start()
        {
            tracker.startedTracking += TrackingStarted;
            tracker.stoppedTracking += TrackingStopped;

            startButton.onClick.AddListener(UserClickedStart);
            restartButton.onClick.AddListener(UserClickedRestart);
            pauseButton.onClick.AddListener(UserClickedPauseResume);
            
            UpdateUIAccordingToState(_state);
        }

        private void Update()
        {
            statusText.text = $"Score: {gameModel.score}\nTime left: 00:{gameModel.timeLeft.TotalSeconds:0.0}";

            if (_state == GameState.InProgress)
            {

                if (gameModel.isFinished)
                {
                    TransitionToState(GameState.Finished);
                }

                if (Input.GetMouseButtonDown(0))
                {
                    var ray = playerCamera.ScreenPointToRay(Input.mousePosition);
                    var plane = new Plane(trackableWorld.transform.rotation * Vector3.up,
                        trackableWorld.transform.position);

                    float dist;
                    if (plane.Raycast(ray, out dist))
                    {
                        var hitIdx = gameModel.TestUserHit(ray.GetPoint(dist), 0.03f);
                        if (hitIdx.HasValue)
                        {
                            gameModel.CommitUserHit(hitIdx.Value);
                        }
                    }
                }
            }
        }

        private void TrackingStarted()
        {
            switch (_state)
            {
                case GameState.PausedByTracking:
                    TransitionToState(GameState.InProgress);
                    break;
                    
                case GameState.PausedByUserAndTracking:
                    TransitionToState(GameState.PausedByUser);
                    break;
                    
                case GameState.WaitingForTracking:
                    TransitionToState(GameState.WaitingForUserToStart);
                    break;
            }
        }

        private void TrackingStopped()
        {
            switch (_state)
            {
                case GameState.WaitingForUserToStart:
                    TransitionToState(GameState.WaitingForTracking);
                    break;
                
                case GameState.PausedByUser:
                    TransitionToState(GameState.PausedByUserAndTracking);
                    break;
                
                case GameState.InProgress:
                    TransitionToState(GameState.PausedByTracking);
                    break;
            }
        }

        private void UserClickedStart()
        {
            switch (_state)
            {
                case GameState.WaitingForUserToStart:
                    TransitionToState(GameState.InProgress);
                    break;
            }
        }

        private void UserClickedPauseResume()
        {
            switch (_state)
            {
                case GameState.InProgress:
                    TransitionToState(GameState.PausedByUser);
                    break;
                
                case GameState.PausedByTracking:
                    TransitionToState(GameState.PausedByUserAndTracking);
                    break;
                
                case GameState.PausedByUserAndTracking:
                    TransitionToState(GameState.PausedByTracking);
                    break;
                
                case GameState.PausedByUser:
                    TransitionToState(GameState.InProgress);
                    break;
            }
        }

        private void UserClickedRestart()
        {
            switch (_state)
            {
                case GameState.Finished:
                    TransitionToState(GameState.InProgress);
                    break;
                    
                case GameState.InProgress:
                    gameModel.GameRestart();
                    break;
            }
        }

        private void TransitionToState(GameState newState)
        {
            Debug.Log($"Transitioning from {_state} to {newState}");

            switch (_state)
            {
                case GameState.WaitingForUserToStart:
                    if (newState == GameState.InProgress)
                    {
                        gameModel.GameStart();
                    }
                    break;
                
                case GameState.InProgress:
                    switch (newState)
                    {
                        case GameState.PausedByUser:
                        case GameState.PausedByTracking:
                        case GameState.PausedByUserAndTracking:
                            gameModel.GamePause();
                            break;
                    }
                    break;
                
                case GameState.PausedByUser:
                case GameState.PausedByTracking:
                case GameState.PausedByUserAndTracking:
                    if (newState == GameState.InProgress)
                    {
                        gameModel.GameResume();
                    }
                    break;
                
                case GameState.Finished:
                    switch (newState)
                    {
                        case GameState.InProgress:
                            gameModel.GameRestart();
                            break;
                    }
                    break;
            }
            
            UpdateUIAccordingToState(newState);
            _state = newState;
        }

        private void UpdateUIAccordingToState(GameState newState)
        {
            switch (newState)
            {
                case GameState.WaitingForTracking:
                    resultsText.gameObject.SetActive(false);
                    startButton.gameObject.SetActive(false);
                    statusText.gameObject.SetActive(false);
                    pauseButton.gameObject.SetActive(false);
                    restartButton.gameObject.SetActive(false);
                    break;
                
                case GameState.WaitingForUserToStart:
                    resultsText.gameObject.SetActive(false);
                    startButton.gameObject.SetActive(true);
                    statusText.gameObject.SetActive(false);
                    pauseButton.gameObject.SetActive(false);
                    restartButton.gameObject.SetActive(false);
                    break;
                
                case GameState.InProgress:
                    resultsText.gameObject.SetActive(false);
                    startButton.gameObject.SetActive(false);
                    statusText.gameObject.SetActive(true);
                    pauseButton.gameObject.SetActive(true);
                    restartButton.gameObject.SetActive(true);
                    pauseButtonText.text = "Pause";
                    break;
                
                case GameState.PausedByUser:
                    pauseButtonText.text = "Resume";
                    break;
                
                case GameState.Finished:
                    statusText.gameObject.SetActive(false);
                    resultsText.gameObject.SetActive(true);
                    startButton.gameObject.SetActive(false);
                    pauseButton.gameObject.SetActive(false);
                    resultsText.text = $"Game finished, your score: {gameModel.score}.";
                    break;
            }
        }
    }
}