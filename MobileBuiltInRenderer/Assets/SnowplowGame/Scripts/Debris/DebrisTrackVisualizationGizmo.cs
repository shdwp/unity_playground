using UnityEngine;

namespace SnowplowGame.Scripts.Debris
{
    /// <summary>
    /// Small behaviour visualizing race tracks so they could be easily put together in the editor.
    /// </summary>
    public class DebrisTrackVisualizationGizmo : MonoBehaviour
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
        }
    }
}