using UnityEngine;

namespace Platformer
{
    [RequireComponent(typeof(PlayerInputReader), typeof(GroundDetector), typeof(PlayerMover))]
    [RequireComponent(typeof(PlayerAnimator))]
    public class Player : MonoBehaviour
    {
        private PlayerAnimator _animator;
        private GroundDetector _groundDetector;
        private PlayerInputReader _inputReader;
        private PlayerMover _mover;

        private void Awake()
        {
            _animator = GetComponent<PlayerAnimator>();
            _groundDetector = GetComponent<GroundDetector>();
            _inputReader = GetComponent<PlayerInputReader>();
            _mover = GetComponent<PlayerMover>();
        }

        private void FixedUpdate()
        {
            float direction = _inputReader.MovementDirection;
            bool isGrounded = _groundDetector.IsGrounded();

            _mover.Move(direction);

            if (_inputReader.ConsumeJumpRequest() && isGrounded)
                _mover.Jump();

            _animator.UpdateMovement(direction);
        }
    }
}
