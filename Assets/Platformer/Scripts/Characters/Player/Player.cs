using UnityEngine;

namespace Platformer
{
    [RequireComponent(typeof(PlayerInputReader), typeof(GroundDetector), typeof(PlayerMover))]
    [RequireComponent(typeof(PlayerAnimator), typeof(Health), typeof(MeleeAttacker))]
    [RequireComponent(typeof(CombatAnimator))]
    public class Player : MonoBehaviour
    {
        private PlayerAnimator _animator;
        private GroundDetector _groundDetector;
        private Health _health;
        private PlayerInputReader _inputReader;
        private MeleeAttacker _attacker;
        private PlayerMover _mover;

        private void Awake()
        {
            _animator = GetComponent<PlayerAnimator>();
            _groundDetector = GetComponent<GroundDetector>();
            _health = GetComponent<Health>();
            _inputReader = GetComponent<PlayerInputReader>();
            _attacker = GetComponent<MeleeAttacker>();
            _mover = GetComponent<PlayerMover>();
        }

        private void FixedUpdate()
        {
            if (_health.IsAlive == false)
            {
                _mover.Move(0f);
                _animator.UpdateMovement(0f);
                return;
            }

            float direction = _inputReader.MovementDirection;
            bool isGrounded = _groundDetector.IsGrounded();

            _mover.Move(direction);

            if (_inputReader.ConsumeJumpRequest() && isGrounded)
                _mover.Jump();

            if (_inputReader.ConsumeAttackRequest())
                _attacker.TryAttack();

            _animator.UpdateMovement(direction);
        }
    }
}
