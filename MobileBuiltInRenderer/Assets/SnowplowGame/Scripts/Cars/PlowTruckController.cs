using UnityEngine;

namespace SnowplowGame.Scripts.Cars
{
    public class PlowTruckController : MonoBehaviour
    {
        public GameObject truck;
        public ScanlineMaskTexController maskController;
        public float maxTurnRate = 10f;

        private Vector3 _steeringTarget;

        private void Update()
        {
            // @TODO: stop endless float; could use a curve
            var zRatio = Mathf.Clamp(truck.transform.position.z - _steeringTarget.z, -maxTurnRate, maxTurnRate);

            var pos = truck.transform.position;
            pos.z = Mathf.Clamp(truck.transform.position.z - zRatio * Time.deltaTime, -18f, 18f);

            // @TODO: add turn ratio with curve
            var rotAngles = truck.transform.rotation.eulerAngles;
            rotAngles.y = Mathf.Lerp(-40f, 40f, Mathf.InverseLerp(-maxTurnRate, maxTurnRate, zRatio));
            
            truck.transform.position = pos;
            truck.transform.rotation = Quaternion.Euler(rotAngles);
            maskController.ContinueMaskedArea(pos, 5f);
        }

        public void SteerTowards(float z)
        {
            _steeringTarget = new Vector3(0f, 0f, z);
        }
    }
}