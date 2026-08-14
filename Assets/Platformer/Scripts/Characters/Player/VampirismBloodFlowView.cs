using System.Collections;
using UnityEngine;

namespace Platformer
{
    [RequireComponent(typeof(VampirismAbility))]
    public class VampirismBloodFlowView : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private ParticleSystem _particleSystem;
        [SerializeField] private Color _flowColor = new Color(0.95f, 0.11f, 0.11f, 0.92f);
        [SerializeField] private float _targetWidth = 0.03f;
        [SerializeField] private float _playerWidth = 0.1f;
        [SerializeField] private float _targetOffsetY = 0.25f;
        [SerializeField] private float _playerOffsetY = 0.35f;
        [SerializeField] private float _particlesPerSecond = 28f;
        [SerializeField] private float _particleLifetime = 0.45f;
        [SerializeField] private float _particleSize = 0.09f;

        private VampirismAbility _ability;
        private Coroutine _followCoroutine;
        private Health _target;
        private float _particleEmissionProgress;

        private void Awake()
        {
            _ability = GetComponent<VampirismAbility>();
            _lineRenderer.startColor = _flowColor;
            _lineRenderer.endColor = _flowColor;
            _lineRenderer.widthCurve = new AnimationCurve(
                new Keyframe(0f, _targetWidth),
                new Keyframe(1f, _playerWidth));
            _lineRenderer.enabled = false;
            _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        private void OnEnable()
        {
            _ability.StateChanged += HandleStateChanged;
            _ability.TargetChanged += HandleTargetChanged;
            _target = _ability.CurrentTarget;
            HandleStateChanged(_ability.State);
        }

        private void OnDisable()
        {
            _ability.StateChanged -= HandleStateChanged;
            _ability.TargetChanged -= HandleTargetChanged;
            StopFollowRoutine();
            _lineRenderer.enabled = false;
            _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        private void HandleTargetChanged(Health target)
        {
            _target = target;
        }

        private void HandleStateChanged(VampirismAbilityState state)
        {
            bool isActive = state == VampirismAbilityState.Active;

            if (isActive == false)
            {
                StopFollowRoutine();
                _lineRenderer.enabled = false;
                _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                return;
            }

            _particleEmissionProgress = 0f;
            _particleSystem.Play();

            if (_followCoroutine == null)
                _followCoroutine = StartCoroutine(FollowTarget());
        }

        private IEnumerator FollowTarget()
        {
            while (_ability.State == VampirismAbilityState.Active && enabled)
            {
                if (_target == null || _target.IsAlive == false)
                {
                    _lineRenderer.enabled = false;
                    yield return null;
                    continue;
                }

                if (_lineRenderer.enabled == false)
                    _lineRenderer.enabled = true;

                Vector3 start = _target.transform.position + Vector3.up * _targetOffsetY;
                Vector3 end = transform.position + Vector3.up * _playerOffsetY;

                _lineRenderer.SetPosition(0, start);
                _lineRenderer.SetPosition(1, end);
                EmitBloodParticles(start, end);
                yield return null;
            }

            _lineRenderer.enabled = false;
            _followCoroutine = null;
        }

        private void EmitBloodParticles(Vector3 start, Vector3 end)
        {
            float emissionRate = Mathf.Max(_particlesPerSecond, 0f);
            _particleEmissionProgress += emissionRate * Time.deltaTime;
            int particleCount = Mathf.FloorToInt(_particleEmissionProgress);

            if (particleCount <= 0)
                return;

            _particleEmissionProgress -= particleCount;

            float lifetime = Mathf.Max(_particleLifetime, 0.02f);
            Vector3 velocity = (end - start) / lifetime;
            ParticleSystem.EmitParams emitParameters = new ParticleSystem.EmitParams();
            emitParameters.position = start;
            emitParameters.velocity = velocity;
            emitParameters.startLifetime = lifetime;
            emitParameters.startSize = _particleSize;
            emitParameters.startColor = _flowColor;
            _particleSystem.Emit(emitParameters, particleCount);
        }

        private void StopFollowRoutine()
        {
            if (_followCoroutine == null)
                return;

            StopCoroutine(_followCoroutine);
            _followCoroutine = null;
        }
    }
}
