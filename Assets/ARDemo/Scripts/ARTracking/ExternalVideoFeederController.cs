using maxstAR;
using UnityEngine;
using UnityEngine.Video;

namespace ARDemo.Scripts.ARTracking
{
    [RequireComponent(typeof(VideoPlayer))]
    public class ExternalVideoFeederController : MonoBehaviour
    {
        private VideoPlayer _player;
        private Texture2D _frameTex;

        private void Awake()
        {
            _player = GetComponent<VideoPlayer>();
            _player.sendFrameReadyEvents = true;
            _player.frameReady += OnNewFrame;
        }

        private void OnNewFrame(VideoPlayer _, long frameIdx)
        {
            var rt = _player.targetTexture;

            if (_frameTex == null)
            {
                _frameTex = new Texture2D(rt.width, rt.height);
            }

            RenderTexture.active = rt;
            _frameTex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            _frameTex.Apply();
            RenderTexture.active = null;

            var data = _frameTex.GetRawTextureData();
            CameraDevice.GetInstance().SetNewFrame(data, data.Length, rt.width, rt.height, ColorFormat.RGBA8888);
        }
    }
}