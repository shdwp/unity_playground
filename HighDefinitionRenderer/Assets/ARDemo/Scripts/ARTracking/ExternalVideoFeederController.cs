using System;
using maxstAR;
using UnityEngine;
using UnityEngine.Video;

namespace ARDemo.Scripts.ARTracking
{
    /// <summary>
    /// Controller which feeds frames from video player to MaxstAR as a substitute for camera
    /// </summary>
    [RequireComponent(typeof(VideoPlayer))]
    public class ExternalVideoFeederController : MonoBehaviour
    {
        private VideoPlayer _player;
        
        // texture which will be used to read frame data from gpu
        private Texture2D _frameTex;

        private void Awake()
        {
            _player = GetComponent<VideoPlayer>();
            _player.sendFrameReadyEvents = true;
            _player.frameReady += OnNewFrame;
        }

        private void Start()
        {
            _frameTex = new Texture2D(_player.targetTexture.width, _player.targetTexture.height);
        }

        private void OnNewFrame(VideoPlayer _, long frameIdx)
        {
            var rt = _player.targetTexture;
            
            // read frame data
            RenderTexture.active = rt;
            _frameTex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            _frameTex.Apply();
            RenderTexture.active = null;

            // pass to AR camera API
            var data = _frameTex.GetRawTextureData();
            CameraDevice.GetInstance().SetNewFrame(data, data.Length, rt.width, rt.height, ColorFormat.RGBA8888);
        }
    }
}