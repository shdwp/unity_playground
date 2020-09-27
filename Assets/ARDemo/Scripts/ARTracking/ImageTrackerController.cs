using System.IO;
using maxstAR;
using UnityEngine;

namespace ARDemo.Scripts.ARTracking
{
    public class ImageTrackerController : MonoBehaviour
    {
        public delegate void StartedTracking();
        public delegate void StoppedTracking();
        
        public ImageTrackableBehaviour trackable;
        public CameraBackgroundBehaviour backgroundBehaviour;

        public StartedTracking startedTracking;
        public StoppedTracking stoppedTracking;
        
        private bool _isTracking = false;
        private TrackerManager _tracker;
        private CameraDevice _camera;

        private void Awake()
        {
            _tracker = TrackerManager.GetInstance();
            _camera = CameraDevice.GetInstance();
        }

        private void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        
            _tracker.StartTracker(TrackerManager.TRACKER_TYPE_IMAGE);
            _tracker.AddTrackerData(Path.Combine(Application.streamingAssetsPath, trackable.TrackerDataFileName));
            _tracker.LoadTrackerData();

            _camera.Start();
        }

        private void Update()
        {
            trackable.OnTrackFail();
            
            TrackingState state = TrackerManager.GetInstance().UpdateTrackingState();
            if (state == null)
            {
                return;
            }

            backgroundBehaviour.UpdateCameraBackgroundImage(state);
            
            var result = state.GetTrackingResult();
            if (result.GetCount() <= 0)
            {
                if (_isTracking)
                {
                    stoppedTracking.Invoke();
                }
                
                _isTracking = false;
                return;
            }
            else
            {
                if (!_isTracking)
                {
                    startedTracking.Invoke();
                }
                
                _isTracking = true;
            }

            var recognizedTrackable = result.GetTrackable(0);
            trackable.OnTrackSuccess(recognizedTrackable.GetId(), recognizedTrackable.GetName(), recognizedTrackable.GetPose());
        }
        
        private void OnDestroy()
        {
            _tracker.SetTrackingOption(TrackerManager.TrackingOption.NORMAL_TRACKING);
            _tracker.StopTracker();
            _tracker.DestroyTracker();
            _camera.Stop();
        }
    }
}