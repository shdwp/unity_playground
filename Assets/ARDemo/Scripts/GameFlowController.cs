using System;
using ARDemo.Scripts.ARTracking;
using ARDemo.Scripts.Game;
using maxstAR;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

namespace ARDemo.Scripts
{
    /// <summary>
    /// Controller which controls the flow of the game and also glues the UI together (not very SOLID, should be split in the future).
    /// </summary>
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

        [Header("SFX bindings")] 
        public AudioSource uiAudioSource;
        public AudioSource hitAudioSource;
        public AudioSource finishAudioSource;

        [Header("World")]
        public Camera playerCamera;
        public Transform trackableWorld;

        [Header("DI")]
        public GameModelController gameModel;
        public ImageTrackerController tracker;

        [Header("Settings")] public float hitThreshold = 0.03f;

        private enum GameState
        {
            // game just started and waiting on tracking to begin
            WaitingForTracking,
            // tracking established, waiting for user to start the game
            WaitingForUserToStart,
            // game is in progress
            InProgress,
            // game paused by user
            PausedByUser,
            // game is paused because tracking has been lost
            PausedByTracking,
            // game is paused by user and also because no tracking
            PausedByUserAndTracking,
            // game has been finished and waiting for the user to restart
            Finished,
        }

        private GameState _state;

        private void Start()
        {
            // set state based on tracker state
            _state = tracker.isTracking ? GameState.WaitingForUserToStart : GameState.WaitingForTracking;
            
            // listen to tracking notifications
            tracker.startedTracking += TrackingStarted;
            tracker.stoppedTracking += TrackingStopped;

            // listen to UI notifications
            startButton.onClick.AddListener(UserClickedStart);
            restartButton.onClick.AddListener(UserClickedRestart);
            pauseButton.onClick.AddListener(UserClickedPauseResume);
            
            // setup starting UI
            UpdateUIAccordingToState(_state);
        }

        private void Update()
        {
            // update status text
            // @TODO: receive tracking loss info
            statusText.text = $"Score: {gameModel.score}\nTime left: 00:{gameModel.timeLeft.TotalSeconds:0.0}";

            if (_state == GameState.InProgress)
            {
                if (gameModel.isFinished)
                {
                    // @TODO: move this check to game model delegate
                    // model notifies that game has been finished, move to finished state
                    TransitionToState(GameState.Finished);
                }

                if (Input.GetMouseButtonDown(0))
                {
                    // user attempted to hit target
                    // @TODO: ignore when UI is in the way
                    var ray = playerCamera.ScreenPointToRay(Input.mousePosition);
                    
                    // plane at real-world marker
                    var plane = new Plane(trackableWorld.transform.rotation * Vector3.up, trackableWorld.transform.position);

                    float dist;
                    if (plane.Raycast(ray, out dist))
                    {
                        // test the hit and commit if it has been successful
                        var hitIdx = gameModel.TestUserHit(ray.GetPoint(dist), hitThreshold);
                        if (hitIdx.HasValue)
                        {
                            // play sound cue
                            hitAudioSource.Play();
                            gameModel.CommitUserHit(hitIdx.Value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tracker notifies that it now successfully tracks the marker
        /// </summary>
        private void TrackingStarted()
        {
            switch (_state)
            {
                case GameState.WaitingForTracking:
                    // game was in initial state waiting for tracking and now can be started by user
                    TransitionToState(GameState.WaitingForUserToStart);
                    break;
                
                case GameState.PausedByTracking:
                    // if game was previously paused because of the tracking loss it can be resumed
                    TransitionToState(GameState.InProgress);
                    break;
                    
                case GameState.PausedByUserAndTracking:
                    // if game was both paused by loss of tracking and paused by user it is now
                    // only paused by user
                    TransitionToState(GameState.PausedByUser);
                    break;
            }
        }

        /// <summary>
        /// Tracker lost marker
        /// </summary>
        private void TrackingStopped()
        {
            switch (_state)
            {
                case GameState.WaitingForUserToStart:
                    // game wasn't started, and tracking was lost, go back to initial state of waiting for it
                    TransitionToState(GameState.WaitingForTracking);
                    break;
                
                case GameState.PausedByUser:
                    // game was paused by user, but now also lacks tracking
                    TransitionToState(GameState.PausedByUserAndTracking);
                    break;
                
                case GameState.InProgress:
                    // game was in progress, but tracking has been lost
                    TransitionToState(GameState.PausedByTracking);
                    break;
            }
        }

        /// <summary>
        /// Action for start button
        /// </summary>
        private void UserClickedStart()
        {
            switch (_state)
            {
                case GameState.WaitingForUserToStart:
                    // only valid case is when game just started and tracks the marker
                    TransitionToState(GameState.InProgress);
                    break;
            }
            
            // play sound cue
            uiAudioSource.Play();
        }

        /// <summary>
        /// Action for pause/resume button
        /// </summary>
        private void UserClickedPauseResume()
        {
            switch (_state)
            {
                case GameState.InProgress:
                    // game is in progress, so - pause
                    TransitionToState(GameState.PausedByUser);
                    break;
                
                case GameState.PausedByTracking:
                    // game is paused by tracking - set that it also paused by user
                    TransitionToState(GameState.PausedByUserAndTracking);
                    break;
                
                case GameState.PausedByUserAndTracking:
                    // game is paused by user and tracking - set it so its only paused by tracking
                    TransitionToState(GameState.PausedByTracking);
                    break;
                
                case GameState.PausedByUser:
                    // game is paused by user and can actually be unpaused
                    TransitionToState(GameState.InProgress);
                    break;
            }
            
            // play sound cue
            uiAudioSource.Play();
        }

        /// <summary>
        /// Action for restart button
        /// </summary>
        private void UserClickedRestart()
        {
            switch (_state)
            {
                case GameState.Finished:
                    // restart the game after it has been finished
                    TransitionToState(GameState.InProgress);
                    break;
                    
                case GameState.InProgress:
                    // restart the game in the process - no state management needed
                    gameModel.GameRestart();
                    break;
            }
            
            // play sound cue
            uiAudioSource.Play();
        }

        /// <summary>
        /// Transition from old state (`_state`) to new state. Runs required models code based on state transition matrix
        /// </summary>
        /// <param name="newState"></param>
        private void TransitionToState(GameState newState)
        {
            Debug.Log($"Transitioning from {_state} to {newState}");

            switch (_state)
            {
                case GameState.WaitingForUserToStart:
                    if (newState == GameState.InProgress)
                    {
                        // game was waiting on user to start and it happened
                        gameModel.GameStart();
                    }
                    break;
                
                case GameState.InProgress:
                    switch (newState)
                    {
                        case GameState.PausedByUser:
                        case GameState.PausedByTracking:
                        case GameState.PausedByUserAndTracking:
                            // game was in progress but paused by either user or tracking (or both)
                            gameModel.GamePause();
                            break;
                        
                        case GameState.Finished:
                            // game just finished
                            finishAudioSource.Play();
                            break;
                    }
                    break;
                
                case GameState.PausedByUser:
                case GameState.PausedByTracking:
                case GameState.PausedByUserAndTracking:
                    if (newState == GameState.InProgress)
                    {
                        // game was paused but now has been resumed
                        gameModel.GameResume();
                    }
                    break;
                
                case GameState.Finished:
                    switch (newState)
                    {
                        case GameState.InProgress:
                            // game has been finished but now being restarted
                            gameModel.GameRestart();
                            break;
                    }
                    break;
            }
            
            UpdateUIAccordingToState(newState);
            _state = newState;
        }

        /// <summary>
        /// Update UI so it matches new state
        /// </summary>
        /// <param name="newState"></param>
        private void UpdateUIAccordingToState(GameState newState)
        {
            switch (newState)
            {
                case GameState.WaitingForTracking:
                    startButton.gameObject.SetActive(false);
                    statusText.gameObject.SetActive(false);
                    pauseButton.gameObject.SetActive(false);
                    restartButton.gameObject.SetActive(false);
                    resultsText.gameObject.SetActive(true);
                    resultsText.text = "Waiting for marker...";
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
                    // when game is in progress this button is Pause
                    pauseButtonText.text = "Pause";
                    break;
                
                case GameState.PausedByUser:
                    // when game is paused by user this button is Resume
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