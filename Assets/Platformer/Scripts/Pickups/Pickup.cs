using System;
using UnityEngine;

namespace Platformer
{
    [RequireComponent(typeof(CircleCollider2D))]
    public abstract class Pickup : MonoBehaviour
    {
        private bool _isCollected;

        public event Action<Pickup> Collected;

        protected virtual void OnEnable()
        {
            _isCollected = false;
        }

        protected bool TryCollectValue(int configuredValue, out int collectedValue)
        {
            collectedValue = 0;

            if (_isCollected || configuredValue <= 0)
                return false;

            _isCollected = true;
            collectedValue = configuredValue;
            Collected?.Invoke(this);
            return true;
        }
    }
}
