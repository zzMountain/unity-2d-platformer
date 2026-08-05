using System;
using UnityEngine;

namespace Platformer
{
    [RequireComponent(typeof(CircleCollider2D))]
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

        public bool TryCollect(out int value)
        {
            value = 0;

            if (_isCollected)
                return false;

            _isCollected = true;
            value = _value;
            Collected?.Invoke(this);
            return true;
        }
    }
}
