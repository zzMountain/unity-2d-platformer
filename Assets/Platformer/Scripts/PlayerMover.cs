using UnityEngine;

namespace Platformer
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] private float _speed = 7f;
        [SerializeField] private float _jumpVelocity = 18f;

        private Rigidbody2D _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        public void Move(float direction)
        {
            _rigidbody.linearVelocityX = direction * _speed;
        }

        public void Jump()
        {
            _rigidbody.linearVelocityY = _jumpVelocity;
        }
    }
}
