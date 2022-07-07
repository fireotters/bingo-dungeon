using UnityEngine;

namespace GameLogic
{
    public class Movement : MonoBehaviour
    {
        private void Update()
        {
            transform.Rotate(Vector3.forward, 90 * Time.deltaTime);
        }
    }
}
