using System.IO;
using maxstAR;
using UnityEngine;

namespace ARDemo.Scripts.ARTracking
{
    /// <summary>
    /// Controller to manage tracking and sending position info to attached object (`trackable`)
    /// </summary>
    public class ImageTrackerController : MonoBehaviour
    {
        /// <summary>
        /// Started tracking marker through camera
        /// </summary>
        public delegate void StartedTracking();
        
        /// <summary>
        /// Tracking was interrupted
        /// </summary>
        public delegate void StoppedTracking();
        
        /// <summary>
        /// Object which will receive marker position information
        /// </summary>
        public ImageTrackableBehaviour trackable;
        
        /// <summary>
        /// Camera background behaviour from MaxstAR camera prefab
        /// </summary>
        public CameraBackgroundBehaviour backgroundBehaviour;

        /// <summary>
        /// Started tracking delegate object
        /// </summary>
        public StartedTracking startedTracking;
        
        /// <summary>
        /// Started tracking delegate object
        /// </summary>
        public StoppedTracking stoppedTracking;
        
        /// <summary>
        /// Whether tracker currently tracks the marker
        /// </summary>
        public bool isTracking = false;
        
        private TrackerManager _tracker;
        private CameraDevice _camera;

        private void Awake()
        {
            _tracker = TrackerManager.GetInstance();
            _camera = CameraDevice.GetInstance();
        }

        private void Start()
        {
            // required for maxst to correctly function
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        
            // start tracker and load marker image data
            _tracker.StartTracker(TrackerManager.TRACKER_TYPE_IMAGE);
            _tracker.AddTrackerData(Path.Combine(Application.streamingAssetsPath, trackable.TrackerDataFileName));
            _tracker.LoadTrackerData();

            // start camera
            _camera.Start();
        }

        private void Update()
        {
            // reset trackable
            trackable.OnTrackFail();
            
            // get tracker state
            TrackingState state = _tracker.UpdateTrackingState();
            if (state == null)
            {
                // tracker is not running
                return;
            }

            backgroundBehaviour.UpdateCameraBackgroundImage(state);
            
            // get tracking data
            var result = state.GetTrackingResult();
            if (result.GetCount() <= 0)
            {
                // tracking marker has not been found, notify delegates that tracking has been stopped if needed
                if (isTracking)
                {
                    stoppedTracking.Invoke();
                }
                
                isTracking = false;
            }
            else
            {
                // marker has been found, notify delegates if needed
                if (!isTracking)
                {
                    startedTracking.Invoke();
                }
                
                isTracking = true;
                
                // pass tracking information onto trackable object for it to position itself
                var recognizedTrackable = result.GetTrackable(0);
                trackable.OnTrackSuccess(recognizedTrackable.GetId(), recognizedTrackable.GetName(), recognizedTrackable.GetPose());
            }
        }
        
        private void OnDestroy()
        {
            // cleanup tracker and camera
            _tracker.SetTrackingOption(TrackerManager.TrackingOption.NORMAL_TRACKING);
            _tracker.StopTracker();
            _tracker.DestroyTracker();
            _camera.Stop();
        }
    }
}