using UnityEngine;

namespace Platformer
{
    [RequireComponent(typeof(Coin))]
    public class CoinView : MonoBehaviour
    {
        [SerializeField] private Transform _visual;
        [SerializeField] private float _bobHeight = 0.15f;
        [SerializeField] private float _bobSpeed = 2.5f;
        [SerializeField] private float _rotationSpeed = 120f;

        private float _phase;
        private Vector3 _startLocalPosition;

        private void Awake()
        {
            _startLocalPosition = _visual.localPosition;
        }

        private void OnEnable()
        {
            _phase = Random.Range(0f, Mathf.PI * 2f);
            _visual.localPosition = _startLocalPosition;
        }

        private void Update()
        {
            float verticalOffset = Mathf.Sin(Time.time * _bobSpeed + _phase) * _bobHeight;
            _visual.localPosition = _startLocalPosition + Vector3.up * verticalOffset;
            _visual.Rotate(0f, _rotationSpeed * Time.deltaTime, 0f, Space.Self);
        }
    }
}
