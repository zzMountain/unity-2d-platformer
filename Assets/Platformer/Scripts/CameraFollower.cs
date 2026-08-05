using UnityEngine;

namespace Platformer
{
    public class CameraFollower : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Vector2 _offset = new Vector2(0f, 1f);
        [SerializeField] private float _smoothness = 7f;

        private void LateUpdate()
        {
            Vector3 targetPosition = new Vector3(
                _target.position.x + _offset.x,
                _target.position.y + _offset.y,
                transform.position.z);
            float interpolation = 1f - Mathf.Exp(-_smoothness * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, targetPosition, interpolation);
        }
    }
}
