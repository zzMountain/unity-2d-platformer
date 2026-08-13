using UnityEngine;

namespace Platformer
{
    public class FirstAidKit : Pickup
    {
        [SerializeField] private int _healing = 30;

        public bool TryCollect(out int healing)
        {
            return TryCollectValue(_healing, out healing);
        }
    }
}
