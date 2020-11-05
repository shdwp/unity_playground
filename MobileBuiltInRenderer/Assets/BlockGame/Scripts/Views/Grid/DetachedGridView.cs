using BlockGame.Scripts.Model;
using BlockGame.Scripts.Model.Interfaces;
using UnityEngine;

namespace BlockGame.Scripts.Views.Grid
{
    public class DetachedGridView: GridView
    {
        public float moveSpeed;
        public float pushSpeed;
        public float fallSpeed;
        
        public override GridType GridType => GridType.Detached;
        
        private void Update()
        {
            transform.position += Vector3.down * (Time.deltaTime * fallSpeed);
            positionUpdate.Dispatch(transform.position);
        }

        public void Move(Vector3 direction)
        {
            transform.position += direction * (moveSpeed * Time.deltaTime);
        }

        public void Push()
        {
            transform.position += Vector3.down * (pushSpeed * Time.deltaTime);
        }
    }
}