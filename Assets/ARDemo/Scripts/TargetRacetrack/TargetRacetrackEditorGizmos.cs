using UnityEngine;

namespace ARDemo.Scripts.TargetTrack
{
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

            if (previousPoint.HasValue && firstPoint.HasValue)
            {
                Gizmos.DrawLine(previousPoint.Value, firstPoint.Value);
            }
        }
    }
}