using System;
using UnityEngine;

namespace RollingDownDemo.Scripts.DrawScene
{
    /// <summary>
    /// Controller that passes user input to DrawnMeshBuilder
    /// </summary>
    public class UserDrawingController : MonoBehaviour
    {
        /// <summary>
        /// minimum draw distance, translates to minimum polygon length (in unity scale)
        /// </summary>
        public float drawDistanceTreshold = 0.001f;
        
        /// <summary>
        /// generated polygon thickness (in unity scale)
        /// </summary>
        public float drawThickness = 0.01f;
        
        /// <summary>
        /// Preview controller for mesh that is being drawn
        /// </summary>
        public DrawnMeshBuilderPreviewController previewController;
        
        // mesh builder instance, shared by this and preview controller
        private DrawnMeshBuilder _builder;
        
        // bool to decide when to start and when to continue segment (based on user input)
        private bool _shouldStartSegment = true;
        
        // last user input position in drawing area coordinates
        private Vector2? _lastDAreaInputPos;
        
        // drwaing area coordinate rect
        private Rect? _drawingAreaRect;
        
        // last Screen width/height used to track window resize and update drawing area rect when needed
        private int _lastScreenWidth, _lastScreenHeight;

        // generated mesh getter for convenience sake
        public Mesh mesh => _builder.mesh;

        /// <summary>
        /// Reset user drawing
        /// </summary>
        public void Reset()
        {
            _builder.Reset();
        }

        private void Awake()
        {
            _builder = new DrawnMeshBuilder();
            previewController.builder = _builder;
        }

        private void Update()
        {
            if (!_drawingAreaRect.HasValue || Screen.width != _lastScreenWidth || Screen.height != _lastScreenHeight)
            {
                // update drawing area rectangle based on screen size
                var min = (float)Math.Min(Screen.width, Screen.height);
                
                _drawingAreaRect = new Rect(
                    (1f - min / Screen.width) / 2f, 
                    (1f - min / Screen.height) / 2f, 
                    min / Screen.width,
                    min / Screen.height
                );
                
                _lastScreenWidth = Screen.width;
                _lastScreenHeight = Screen.height;
            }
            
            if (Input.GetMouseButton(0))
            {
                // transform mouse position to drawing area coordinate space
                var drawingAreaPos = ScreenToDrawingAreaPos(Input.mousePosition);
            
                // check if this is just first input
                if (_lastDAreaInputPos.HasValue)
                {
                    // check if drawn distance gets over threshold
                    if ((_lastDAreaInputPos.Value - drawingAreaPos).magnitude > drawDistanceTreshold)
                    {
                        // add line to mesh builder
                        PlotLine(_lastDAreaInputPos.Value, drawingAreaPos);
                        _lastDAreaInputPos = drawingAreaPos;
                    }
                }
                else
                {
                    _lastDAreaInputPos = drawingAreaPos;
                }
            }
            else
            {
                // user released mouse button, reset variables so next
                // time we start anew
                _shouldStartSegment = true;
                _lastDAreaInputPos = null;
            }
        }

        /// <summary>
        /// Adds segment to the mesh based on drawing area line.
        /// </summary>
        /// <param name="startPos">pos in drawing area coordinate space. Ignored if this is line continuation</param>
        /// <param name="endPos">pos in drawing area coordinate space</param>
        private void PlotLine(Vector2 startPos, Vector2 endPos)
        {
            if (!_shouldStartSegment)
            {
                // this is a continuation of user input, meaning that we should
                // merely add to the existing segment
                _builder.AddContinuationSegment(DrawingAreaToMeshPos(endPos), drawThickness);
            }
            else
            {
                // this is a first input in series, meaning that we should
                // start new segment in builder
                _builder.AddSegment(DrawingAreaToMeshPos(startPos), DrawingAreaToMeshPos(endPos), drawThickness);
                _shouldStartSegment = false;
            }
        }

        /// <summary>
        /// Transform position from screen coordinate space to drawing area coordinate space
        /// </summary>
        /// <param name="pos">Position in pixels</param>
        /// <returns>Position in drawing-area rect factor (0f - 1f)</returns>
        private Vector2 ScreenToDrawingAreaPos(Vector2 pos)
        {
            if (!_drawingAreaRect.HasValue)
            {
                Debug.Assert(false, "_drawingAreaRect not set!");
                return Vector2.zero;
            }
            
            var screenPos = pos / new Vector2(Screen.width, Screen.height);
            var drawingAreaPos = new Vector2(
                Mathf.InverseLerp(_drawingAreaRect.Value.xMin, _drawingAreaRect.Value.xMax, screenPos.x),
                Mathf.InverseLerp(_drawingAreaRect.Value.yMin, _drawingAreaRect.Value.yMax, screenPos.y)
            );

            return drawingAreaPos;
        }

        /// <summary>
        /// Transform position from drawing area coordinate space to mesh coordinate space
        /// </summary>
        /// <param name="pos">Position in drawing area rect factor (0f - 1f)</param>
        /// <returns>Position in mesh coordinates (-0.5f - 0.5f)</returns>
        private Vector2 DrawingAreaToMeshPos(Vector2 pos)
        {
            return pos - new Vector2(0.5f, 0.5f);
        }
    }
}