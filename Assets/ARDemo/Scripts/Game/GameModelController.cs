using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ARDemo.Scripts.TargetTrack;
using UnityEngine;

namespace ARDemo.Scripts.Game
{
    public class GameModelController : MonoBehaviour
    {
        public Transform tracksRoot;
        public Transform world;
        public GameTargetController targetPrefab;

        public int gameLengthSeconds = 60;

        private bool _started = false, _paused = false, _finished = false;
        public bool isInProgress => _started && !_paused && !_finished;
        public bool isFinished => _finished;

        private Stopwatch _gameStopwatch = new Stopwatch();
        public TimeSpan timeLeft => TimeSpan.FromSeconds(60) - _gameStopwatch.Elapsed;

        public int score = 0;

        private Dictionary<int, TargetRacetrackModel> _tracks;
        private Dictionary<int, GameTargetController> _targets;
        
        public void GameStart()
        {
            _targets = new Dictionary<int, GameTargetController>();
            _tracks = new Dictionary<int, TargetRacetrackModel>();
            
            for (var i = 0; i < tracksRoot.childCount; i++)
            {
                var child = tracksRoot.GetChild(i);
                
                if (child.gameObject.activeSelf)
                {
                    var track = TargetRacetrackModel.PositionFromRooTransform(tracksRoot.GetChild(i));
                    _tracks[i] = track;
                    SpawnNewTargetOnTrackIdx(i);
                }
            }
            
            _gameStopwatch = Stopwatch.StartNew();
            _started = true;
        }

        public void GamePause()
        {
            _gameStopwatch.Stop();
            _paused = true;
        }

        public void GameResume()
        {
            _gameStopwatch.Start();
            _paused = false;
        }

        public void GameRestart()
        {
            foreach (var target in _targets.Values)
            {
                Destroy(target.gameObject);
            }
            
            GameStart();
            score = 0;
            _paused = false;
            _finished = false;
        }

        public int? TestUserHit(Vector3 position, float treshold)
        {
            foreach (var kv in _targets)
            {
                if (kv.Value != null)
                {
                    if (Vector3.Distance(kv.Value.transform.position, position) < treshold)
                    {
                        return kv.Key;
                    }
                }
            }

            return null;
        }

        public void CommitUserHit(int idx)
        {
            score++;
            Destroy(_targets[idx].gameObject);
            _targets[idx] = null;
            StartCoroutine(CoroutineSpawnTargetAfterHit(idx));
        }

        private void Update()
        {
            if (isInProgress && timeLeft <= TimeSpan.Zero)
            {
                _finished = true;
                _paused = true;
            }
        }

        private IEnumerator CoroutineSpawnTargetAfterHit(int idx)
        {
            // @TODO: respect game pause
            yield return new WaitForSeconds(2f);
            SpawnNewTargetOnTrackIdx(idx);
            yield return null;
        }

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