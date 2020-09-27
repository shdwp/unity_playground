using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityTemplateProjects
{
    public class SceneSphereController : MonoBehaviour
    {
        public string targetScene;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var center = Camera.main.WorldToScreenPoint(transform.position);
                var top = Camera.main.WorldToScreenPoint(transform.position + transform.up);
                var radius = Vector3.Distance(top, center) / 2.5f;

                if (Vector3.Distance(Input.mousePosition, center) < radius)
                {
                    SceneManager.LoadScene(targetScene);
                }
            }
        }
    }
}