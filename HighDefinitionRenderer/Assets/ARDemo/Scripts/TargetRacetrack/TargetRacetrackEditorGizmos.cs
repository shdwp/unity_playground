using UnityEngine;

namespace ARDemo.Scripts.TargetRacetrack
{
    /// <summary>
    /// Small behaviour visualizing race tracks so they could be easily put together in the editor.
    /// You might also want to enable _EditorBackdrop object so that there will be a marker reference at 0;0;0.
    /// </summary>
    public class TargetRacetrackEditorGizmos : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Vector3? previousPoint = null;
            Vector3? firstPoint = null;
            
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                Gizmos.DrawSphere(child.position, 0.01f);
                
                if (previousPoint.HasValue)
                {
                    Gizmos.DrawLine(previousPoint.Value, child.position);
                }

                if (!firstPoint.HasValue)
                {
                    firstPoint = child.position;
                }
                
                previousPoint = child.position;
            }

            // draw line connecting last and first points
            if (previousPoint.HasValue && firstPoint.HasValue)
            {
                Gizmos.DrawLine(previousPoint.Value, firstPoint.Value);
            }
        }
    }
}