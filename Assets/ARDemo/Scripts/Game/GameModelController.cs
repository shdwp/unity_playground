using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ARDemo.Scripts.TargetRacetrack;
using UnityEngine;

namespace ARDemo.Scripts.Game
{
    /// <summary>
    /// Model class for game process, designed to be attached to an empty component (for DI-sake)
    /// </summary>
    public class GameModelController : MonoBehaviour
    {
        /// <summary>
        /// Root transform that contains track information. Should, in respect, contain number of tracks,
        /// each of which contain number of points (in coordinates relative to marker position)
        /// </summary>
        public Transform tracksRoot;
        
        /// <summary>
        /// World transform (which will be positioned to match AR marker)
        /// </summary>
        public Transform world;
        
        /// <summary>
        /// Target object prefab (for the user to hit)
        /// </summary>
        public GameTargetController targetPrefab;

        /// <summary>
        /// Length of the game in seconds
        /// </summary>
        public int gameLengthInSeconds = 60;

        private bool _started = false, _paused = false, _finished = false;
        
        /// <summary>
        /// Whether game is currently in progress (started, not paused, not finished)
        /// </summary>
        public bool isInProgress => _started && !_paused && !_finished;
        
        /// <summary>
        /// Whether game is finished (time run out)
        /// </summary>
        public bool isFinished => _finished;

        private Stopwatch _gameStopwatch = new Stopwatch();
        
        /// <summary>
        /// Amount of time left in this playthrough
        /// </summary>
        public TimeSpan timeLeft => TimeSpan.FromSeconds(gameLengthInSeconds) - _gameStopwatch.Elapsed;

        /// <summary>
        /// Score (incremented with each commited hit)
        /// </summary>
        public int score = 0;

        private Dictionary<int, TargetRacetrackModel> _tracks;
        private Dictionary<int, GameTargetController> _targets;
        
        /// <summary>
        /// Start the game - read tracks info, spawn initial set of targets, start the timer.
        /// Existing targets should be removed, score reset and _finished, _paused set to false if
        /// this is called as a restart.
        /// </summary>
        public void GameStart()
        {
            _targets = new Dictionary<int, GameTargetController>();
            _tracks = new Dictionary<int, TargetRacetrackModel>();
            
            // create track models from tracks object info
            for (var i = 0; i < tracksRoot.childCount; i++)
            {
                var child = tracksRoot.GetChild(i);
                
                if (child.gameObject.activeSelf)
                {
                    var track = TargetRacetrackModel.PositionFromRooTransform(tracksRoot.GetChild(i));
                    _tracks[i] = track;
                    
                    // immediately spawn target for each track as the game starts
                    SpawnNewTargetOnTrackIdx(i);
                }
            }
            
            // start stopwatch that will be used to keep track of the play time
            _gameStopwatch = Stopwatch.StartNew();
            _started = true;
        }

        /// <summary>
        /// Pause the game. Stops the timer and stops the targets from moving
        /// </summary>
        public void GamePause()
        {
            _gameStopwatch.Stop();
            _paused = true;
        }

        /// <summary>
        /// Resume previously paused game
        /// </summary>
        public void GameResume()
        {
            _gameStopwatch.Start();
            _paused = false;
        }

        /// <summary>
        /// Restart game
        /// </summary>
        public void GameRestart()
        {
            // stop spawn coroutines from spawning targets scheduled for previous game
            StopAllCoroutines();
            
            // remove remaining targets
            foreach (var target in _targets.Values)
            {
                if (target != null)
                {
                    Destroy(target.gameObject);
                }
            }
            
            // reset stats
            score = 0;
            _paused = false;
            _finished = false;
            
            // run game start as if the model was new
            GameStart();
        }

        /// <summary>
        /// Test whether there is a target at world-space position.
        /// Returns index which can be passed to `CommitUserHit` to actually affect the game.
        /// </summary>
        /// <param name="position">Position in world-space</param>
        /// <param name="threshold">Maximum distance to target for the hit to count</param>
        /// <returns>Internal index of the target</returns>
        public int? TestUserHit(Vector3 position, float threshold)
        {
            foreach (var kv in _targets)
            {
                if (kv.Value != null)
                {
                    if (Vector3.Distance(kv.Value.transform.position, position) < threshold)
                    {
                        return kv.Key;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Commit user hit that was previously tested by `TestUserHit`
        /// </summary>
        /// <param name="idx">Internal index received from `TestUserHit`</param>
        public void CommitUserHit(int idx)
        {
            // increment score 
            score++;
            
            // destroy target
            Destroy(_targets[idx].gameObject);
            _targets[idx] = null;
            
            // start coroutine that will eventually respawn target on the same track
            StartCoroutine(CoroutineSpawnTargetAfterHit(idx));
        }

        private void Update()
        {
            // check if the session play time is over
            if (isInProgress && timeLeft <= TimeSpan.Zero)
            {
                _finished = true;
                _paused = true;
            }
        }

        /// <summary>
        /// Coroutine to respawn targets that were hit by user - waits 2 seconds and then spawns new one.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        private IEnumerator CoroutineSpawnTargetAfterHit(int idx)
        {
            // @TODO: respect game pause
            yield return new WaitForSeconds(2f);
            SpawnNewTargetOnTrackIdx(idx);
            yield return null;
        }

        /// <summary>
        /// Spawn target on the track idx
        /// </summary>
        /// <param name="idx">Index of the track</param>
        private void SpawnNewTargetOnTrackIdx(int idx)
        {
            var target = Instantiate(targetPrefab, world);
            target.gameModel = this;
            target.racetrack = _tracks[idx].Clone();
            target.transform.localPosition = target.racetrack.position;
            _targets[idx] = target;
        }
    }
}