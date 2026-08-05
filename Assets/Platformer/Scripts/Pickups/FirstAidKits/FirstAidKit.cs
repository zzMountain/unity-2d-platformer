using System;
using UnityEngine;

namespace Platformer
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class FirstAidKit : MonoBehaviour
    {
        [SerializeField] private int _healing = 30;

        private bool _isCollected;

        public event Action<FirstAidKit> Collected;

        private void OnEnable()
        {
            _isCollected = false;
        }

        public bool TryCollect(out int healing)
        {
            healing = 0;

            if (_isCollected || _healing <= 0)
                return false;

            _isCollected = true;
            healing = _healing;
            Collected?.Invoke(this);
            return true;
        }
    }
}
