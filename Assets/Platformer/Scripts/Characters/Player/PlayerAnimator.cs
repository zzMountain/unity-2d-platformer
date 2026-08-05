using UnityEngine;

namespace Platformer
{
    [RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public class PlayerAnimator : MonoBehaviour
    {
        public const string SpeedParameterName = "Speed";

        private static readonly int s_speedHash = Animator.StringToHash(SpeedParameterName);

        private Animator _animator;
        private SpriteRenderer _renderer;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _renderer = GetComponent<SpriteRenderer>();
        }

        public void UpdateMovement(float direction)
        {
            float speed = Mathf.Abs(direction);
            _animator.SetFloat(s_speedHash, speed);

            if (Mathf.Approximately(direction, 0f) == false)
                _renderer.flipX = direction < 0f;
        }
    }
}
