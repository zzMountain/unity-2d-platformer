using UnityEngine;

namespace Platformer
{
    [RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    [RequireComponent(typeof(Health), typeof(MeleeAttacker))]
    public class CombatAnimator : MonoBehaviour
    {
        private const string AttackParameterName = "Attack";
        private const string DeathParameterName = "Death";

        private static readonly int s_attackHash = Animator.StringToHash(AttackParameterName);
        private static readonly int s_deathHash = Animator.StringToHash(DeathParameterName);

        [SerializeField] private SpriteRenderer _attackEffect;

        private Animator _animator;
        private Health _health;
        private MeleeAttacker _attacker;
        private SpriteRenderer _renderer;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _health = GetComponent<Health>();
            _attacker = GetComponent<MeleeAttacker>();
            _renderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            _attacker.AttackPerformed += HandleAttackPerformed;
            _health.Died += HandleDied;
        }

        private void OnDisable()
        {
            _attacker.AttackPerformed -= HandleAttackPerformed;
            _health.Died -= HandleDied;
        }

        private void HandleAttackPerformed()
        {
            _attackEffect.flipX = _renderer.flipX;
            _animator.SetTrigger(s_attackHash);
        }

        private void HandleDied()
        {
            _animator.SetTrigger(s_deathHash);
        }
    }
}
