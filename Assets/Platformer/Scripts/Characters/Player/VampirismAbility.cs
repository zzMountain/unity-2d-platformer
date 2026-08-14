using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    [RequireComponent(typeof(PlayerInputReader), typeof(Health))]
    public class VampirismAbility : MonoBehaviour
    {
        [SerializeField] private float _abilityDuration = 6f;
        [SerializeField] private float _cooldownDuration = 4f;
        [SerializeField] private float _radius = 2.5f;
        [SerializeField] private int _damagePerTick = 2;
        [SerializeField] private int _healPerTick = 2;
        [SerializeField] private float _drainTickInterval = 0.1f;
        [SerializeField] private int _initialTargetCapacity = 16;
        [SerializeField] private LayerMask _targetLayers = ~0;

        private List<Collider2D> _overlapResults;
        private ContactFilter2D _targetFilter;
        private Health _health;
        private PlayerInputReader _inputReader;
        private Health _currentTarget;
        private Coroutine _abilityCoroutine;
        private VampirismAbilityState _state = VampirismAbilityState.Ready;
        private float _progress = 1f;

        public event Action<VampirismAbilityState> StateChanged;
        public event Action<float> ProgressChanged;
        public event Action<Health> TargetChanged;

        public VampirismAbilityState State => _state;
        public Health CurrentTarget => _currentTarget;
        public float Range => _radius;
        public bool IsReady => _state == VampirismAbilityState.Ready;
        public float Progress => _progress;

        private void Awake()
        {
            _health = GetComponent<Health>();
            _inputReader = GetComponent<PlayerInputReader>();
            _overlapResults = new List<Collider2D>(Mathf.Max(_initialTargetCapacity, 1));
            _targetFilter = new ContactFilter2D();
            _targetFilter.SetLayerMask(_targetLayers);
            _targetFilter.useTriggers = true;
        }

        private void OnEnable()
        {
            _health.Died += HandleOwnerDied;
            _inputReader.DrainRequested += HandleDrainRequested;
        }

        private void OnDisable()
        {
            _health.Died -= HandleOwnerDied;
            _inputReader.DrainRequested -= HandleDrainRequested;
            StopRoutine();
            ChangeState(VampirismAbilityState.Ready, 1f);
        }

        private void HandleDrainRequested()
        {
            if (_health.IsAlive == false || IsReady == false)
                return;

            Activate();
        }

        private void Activate()
        {
            if (_abilityDuration <= 0f || _cooldownDuration < 0f)
                return;

            if (_abilityCoroutine != null)
                return;

            _abilityCoroutine = StartCoroutine(RunLifecycle());
        }

        private IEnumerator RunLifecycle()
        {
            yield return RunActivePhase();
            yield return RunCooldownPhase();
            ChangeState(VampirismAbilityState.Ready, 1f);
            _abilityCoroutine = null;
        }

        private IEnumerator RunActivePhase()
        {
            float tickInterval = Mathf.Max(_drainTickInterval, 0.02f);
            float startedAt = Time.time;
            float endsAt = startedAt + _abilityDuration;
            float nextDrainAt = startedAt;

            ChangeTarget(null);
            ChangeState(VampirismAbilityState.Active, 1f);

            while (Time.time < endsAt && _health.IsAlive)
            {
                if (Time.time >= nextDrainAt)
                {
                    ApplyDrainTick();
                    nextDrainAt = Time.time + tickInterval;
                }

                float normalized = Mathf.Clamp01((endsAt - Time.time) / _abilityDuration);
                ChangeProgress(normalized);
                yield return null;
            }

            ChangeProgress(0f);
            ChangeTarget(null);
        }

        private IEnumerator RunCooldownPhase()
        {
            float startedAt = Time.time;
            float endsAt = startedAt + _cooldownDuration;

            ChangeState(VampirismAbilityState.Cooldown, 0f);

            while (Time.time < endsAt && _health.IsAlive)
            {
                float normalized = _cooldownDuration == 0f
                    ? 1f
                    : Mathf.Clamp01((Time.time - startedAt) / _cooldownDuration);
                ChangeProgress(normalized);
                yield return null;
            }
        }

        private void ApplyDrainTick()
        {
            Health target = FindNearestTargetInRange();
            ChangeTarget(target);

            if (target == null || target.IsAlive == false)
                return;

            if (target.TakeDamage(_damagePerTick, out int drainedHealth) == false)
                return;

            int restoredHealth = Mathf.Min(_healPerTick, drainedHealth);
            _health.Restore(restoredHealth);
        }

        private Health FindNearestTargetInRange()
        {
            _overlapResults.Clear();
            Physics2D.OverlapCircle(transform.position, _radius, _targetFilter, _overlapResults);

            float closestSqrDistance = float.MaxValue;
            Health nearest = null;

            for (int i = 0; i < _overlapResults.Count; i++)
            {
                Collider2D collider = _overlapResults[i];

                if (collider == null)
                    continue;

                if (collider.TryGetComponent(out Enemy enemy) == false)
                    continue;

                if (enemy.TryGetComponent(out Health candidate) == false ||
                    candidate == _health ||
                    candidate.IsAlive == false)
                    continue;

                float sqrDistance = (candidate.transform.position - transform.position).sqrMagnitude;

                if (sqrDistance >= closestSqrDistance)
                    continue;

                closestSqrDistance = sqrDistance;
                nearest = candidate;
            }

            return nearest;
        }

        private void ChangeTarget(Health target)
        {
            if (_currentTarget == target)
                return;

            _currentTarget = target;
            TargetChanged?.Invoke(_currentTarget);
        }

        private void ChangeState(VampirismAbilityState state, float normalizedProgress)
        {
            bool hasStateChanged = _state != state;
            _state = state;

            if (_state != VampirismAbilityState.Active)
                ChangeTarget(null);

            if (hasStateChanged)
                StateChanged?.Invoke(_state);

            ChangeProgress(normalizedProgress);
        }

        private void ChangeProgress(float normalizedProgress)
        {
            _progress = Mathf.Clamp01(normalizedProgress);
            ProgressChanged?.Invoke(_progress);
        }

        private void HandleOwnerDied()
        {
            StopRoutine();
            ChangeState(VampirismAbilityState.Ready, 1f);
        }

        private void StopRoutine()
        {
            if (_abilityCoroutine == null)
                return;

            StopCoroutine(_abilityCoroutine);
            _abilityCoroutine = null;
            ChangeTarget(null);
        }
    }
}
