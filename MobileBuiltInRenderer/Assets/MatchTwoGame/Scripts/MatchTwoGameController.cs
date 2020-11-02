using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MatchTwoGame.Scripts.Backend;
using MatchTwoGame.Scripts.Tiles;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace MatchTwoGame.Scripts
{
    /// <summary>
    /// Top level game controller - manages overall game flow
    /// </summary>
    public class MatchTwoGameController : MonoBehaviour
    {
        // DI
        public TilesArrayController tilesArrayController;
        public Animator shadowAnimator;
        public Text scoreText, timeText;

        // whether input is currently accepted from the user
        private bool _inputBlocked = true;
        
        // while JSON can technically hold any number of pictograms, game only supports three
        private int _pictogramsCount = 3;
        
        private int _score = 0;
        private BackendClient _client;

        // mapping of tile indexes to pictogram indexes. Used to check whether 
        // clicked tiles actually match
        private int[] _tilePictogramMapping;
        
        // list of tile indexes that has been completed
        private List<int> _completedTiles = new List<int>();
        
        // list of tile indexes that are currently flipped face up. Will transition to _completedTiles
        // if matching
        private List<int> _flippedTiles = new List<int>();
        
        // cached textures for pictograms
        private Texture2D[] _pictogramTextures;
        
        private void Awake()
        {
            // create backend client and subscribe to its events
            Debug.Log($"GameController: Instantiated BackendClient");
            _client = new BackendClient();
            _client.schemeDownloaded += list =>
            {
                Debug.Log($"GameController: downloaded scheme {list.Count} textures");
                // create texture cache array
                _pictogramTextures = new Texture2D[list.Count];
                
                // show unloaded tiles
                tilesArrayController.ShowUnloadedTiles();
            };
            
            _client.textureDownloaded += (tex, progress) =>
            {
                Debug.Log($"GameController: downloaded texture, {progress} left");
                // load tiles which pictogram (according to mapping) has been loaded
                tilesArrayController.LoadTile(progress, tex);
                
                // cache texture
                _pictogramTextures[progress] = tex;

                if (progress == 0)
                {
                    // all pictograms have been loaded
                    
                    // animate the shadows appearance
                    shadowAnimator.enabled = true;
                    
                    // start the game
                    StartCoroutine(GameStartCoroutine());
                }
            };
            
            _client.downloadFailed += _ => Debug.LogError($"Download failed!");
        }

        private void Start()
        {
            // setup initial random pictogram mapping
            _tilePictogramMapping = RandomPictogramIndexes();
            tilesArrayController.tilePictogramMapping = _tilePictogramMapping;
            
            Debug.Log($"GameController: Starting data download coroutine");
            // start download on scene start
            StartCoroutine(_client.DataDownloadCoroutine());
        }

        private void Update()
        {
            // update time label
            timeText.text = $"Time: {Time.timeSinceLevelLoad:F0}";
        }

        /// <summary>
        /// Process user event of tile touch
        /// </summary>
        /// <param name="idx"></param>
        public void UserTouchedTile(int idx)
        {
            if (_inputBlocked)
            {
                Debug.Log($"GameController: User touched tile {idx}, but input was blocked");
                return;
            }
            
            if (!_flippedTiles.Contains(idx) && !_completedTiles.Contains(idx))
            {
                _inputBlocked = true;
                Debug.Log($"GameController: User touched tile {idx}, starting flip coroutine");
                StartCoroutine(TileFlippedCoroutine(idx));
            }
            else
            {
                Debug.Log($"GameController: User touched tile {idx}, but it was already flipped/completed");
            }
        }

        /// <summary>
        /// Game start coroutine - waits for two seconds, then flips all tiles face down and then unblocks user input
        /// </summary>
        /// <returns></returns>
        private IEnumerator GameStartCoroutine()
        {
            _completedTiles.Clear();
            _flippedTiles.Clear();
            
            Debug.Log($"GameController: Game starting, waiting 2 seconds");
            yield return new WaitForSeconds(2f);
            
            Debug.Log($"GameController: Game starting, flipping all tiles face down");
            yield return StartCoroutine(tilesArrayController.SetAllTilesFlipped(true));
            
            _inputBlocked = false;
        }

        /// <summary>
        /// Game restart coroutine - waits for one second, flips all tiles face down, randomized pictogram mappings,
        /// updates tiles while they're hidden, flips them up to show to user and then runs game start routine
        /// </summary>
        /// <returns></returns>
        private IEnumerator GameRestart()
        {
            Debug.Log($"GameController: Game restarting, waiting 1 second");
            // give user a "win" screen of sorts
            yield return new WaitForSeconds(1f);
            
            Debug.Log($"GameController: Game restarting, flipping all tiles face down");
            // flip tiles face down
            yield return StartCoroutine(tilesArrayController.SetAllTilesFlipped(true));
            
            // update random mappings
            _tilePictogramMapping = RandomPictogramIndexes();
            tilesArrayController.tilePictogramMapping = _tilePictogramMapping;

            Debug.Log($"GameController: Game restarting, loading updated pictograms");
            // set correct textures according to updated mappings, skipping animation
            for (int i = 0; i < _pictogramTextures.Length; i++)
            {
                tilesArrayController.LoadTile(i, _pictogramTextures[i], false);
            }
            
            Debug.Log($"GameController: Game restarting, flipping tiles face up");
            // flip the tiles back on, restoring the state to the initial one before game start
            yield return StartCoroutine(tilesArrayController.SetAllTilesFlipped(false));
            
            // start the round as usual
            yield return StartCoroutine(GameStartCoroutine());
        }

        /// <summary>
        /// Coroutine for when user flips the tile.
        ///
        /// Will wait for two flipped tiles, and then if they match will add those to _completedTiles array.
        /// If every tile has been completed runs game restart coroutine.
        /// Blocks user input until important animations are being performed.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        private IEnumerator TileFlippedCoroutine(int idx)
        {
            // flip the tile, but don't wait on animation completion (for responsiveness sake)
            var flipCoroutine = StartCoroutine(tilesArrayController.FlipTile(idx));
            Debug.Log($"GameController: flipping tile {idx}");
            
            // add tile to flipped tiles array
            _flippedTiles.Add(idx);

            if (_flippedTiles.Count >= 2)
            {
                Debug.Log($"GameController: two tiles flipped, waiting for animation to complete");
                // two cards has been flipped, wait for the second one animation to be completed
                yield return flipCoroutine;
                
                if (_tilePictogramMapping[_flippedTiles[0]] == _tilePictogramMapping[_flippedTiles[1]])
                {
                    Debug.Log($"GameController: two tiles flipped, matching");
                    // two tiles pictograms match - add them to completed array and bump score
                    _completedTiles.AddRange(_flippedTiles);
                    _score++;
                    UpdateScoreText();

                    if (_completedTiles.Count == tilesArrayController.tileCount)
                    {
                        // all tiles have been completed - run game restart coroutine,
                        // input will be unblocked by it when ready
                        StartCoroutine(GameRestart());
                    }
                    else
                    {
                        Debug.Log($"GameController: two tiles flipped, unblocking input");
                        // some tiles are not completed just yet, unblock the input
                        _inputBlocked = false;
                    }
                }
                else
                {
                    Debug.Log($"GameController: two tiles flipped, cards not matching, flipping back, unblocking input");
                    // tile pictograms don't match - flip them face down and unblock input
                    foreach (var flippedTileIdx in _flippedTiles)
                    {
                        StartCoroutine(tilesArrayController.FlipTile(flippedTileIdx));
                    }
                    
                    _inputBlocked = false;
                }

                // either way clear flipped tiles
                _flippedTiles.Clear();
            }
            else
            {
                Debug.Log($"GameController: one tile flipped, unblocking input");
                // if only one tile has been flipped immediately unblock input (for responsiveness sake)
                _inputBlocked = false;
            }
        }

        /// <summary>
        /// Update score label
        /// </summary>
        private void UpdateScoreText()
        {
            scoreText.text = $"Completed: {_score}";
        }

        /// <summary>
        /// Generate array of random tile-index-to-pictogram-index mapping
        /// </summary>
        /// <returns></returns>
        private int[] RandomPictogramIndexes()
        {
            var indexes = new List<int>();
            
            // two tiles per one pictogram
            indexes.AddRange(Enumerable.Range(0, _pictogramsCount));
            indexes.AddRange(Enumerable.Range(0, _pictogramsCount));
            
            return indexes.OrderBy(_ => Random.Range(0, 100)).ToArray();
        }
    }
}