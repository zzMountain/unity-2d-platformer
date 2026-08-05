using System;
using UnityEngine;

namespace Platformer
{
    public class Coin : MonoBehaviour
    {
        [SerializeField] private int _value = 1;
        [SerializeField] private float _bobHeight = 0.15f;
        [SerializeField] private float _bobSpeed = 2.5f;
        [SerializeField] private float _rotationSpeed = 120f;

        private bool _isCollected;
        private float _phase;
        private Vector3 _startPosition;

        public event Action<Coin> Collected;

        private void OnEnable()
        {
            _isCollected = false;
            _phase = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            _startPosition = transform.position;
        }

        private void Update()
        {
            float verticalOffset = Mathf.Sin(Time.time * _bobSpeed + _phase) * _bobHeight;
            transform.position = _startPosition + Vector3.up * verticalOffset;
            transform.Rotate(0f, _rotationSpeed * Time.deltaTime, 0f, Space.Self);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isCollected)
                return;

            if (other.TryGetComponent(out CoinWallet wallet) == false)
                return;

            _isCollected = true;
            wallet.Add(_value);
            Collected?.Invoke(this);
        }
    }
}
