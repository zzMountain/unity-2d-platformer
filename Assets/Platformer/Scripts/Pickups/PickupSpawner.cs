using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class PickupSpawner : MonoBehaviour
    {
        private readonly Dictionary<Pickup, Transform> _activePickups = new Dictionary<Pickup, Transform>();
        private readonly HashSet<Transform> _occupiedPoints = new HashSet<Transform>();
        private readonly List<float> _respawnTimers = new List<float>();

        [SerializeField] private Pickup _pickupPrefab;
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private int _maximumActivePickups = 3;
        [SerializeField] private float _respawnDelay = 3f;

        private void Start()
        {
            int initialPickupCount = Mathf.Min(_maximumActivePickups, _spawnPoints.Length);

            for (int i = 0; i < initialPickupCount; i++)
                SpawnPickup();
        }

        private void Update()
        {
            for (int i = _respawnTimers.Count - 1; i >= 0; i--)
            {
                _respawnTimers[i] -= Time.deltaTime;

                if (_respawnTimers[i] > 0f)
                    continue;

                _respawnTimers.RemoveAt(i);
                SpawnPickup();
            }
        }

        private void OnDestroy()
        {
            foreach (Pickup pickup in _activePickups.Keys)
                pickup.Collected -= HandlePickupCollected;
        }

        private void SpawnPickup()
        {
            if (_activePickups.Count >= _maximumActivePickups)
                return;

            Transform spawnPoint = GetAvailableSpawnPoint();

            if (spawnPoint == null)
                return;

            Pickup pickup = Instantiate(_pickupPrefab, spawnPoint.position, Quaternion.identity, transform);
            pickup.Collected += HandlePickupCollected;
            _activePickups.Add(pickup, spawnPoint);
            _occupiedPoints.Add(spawnPoint);
        }

        private Transform GetAvailableSpawnPoint()
        {
            if (_occupiedPoints.Count >= _spawnPoints.Length)
                return null;

            int startIndex = Random.Range(0, _spawnPoints.Length);

            for (int offset = 0; offset < _spawnPoints.Length; offset++)
            {
                int index = (startIndex + offset) % _spawnPoints.Length;
                Transform spawnPoint = _spawnPoints[index];

                if (_occupiedPoints.Contains(spawnPoint) == false)
                    return spawnPoint;
            }

            return null;
        }

        private void HandlePickupCollected(Pickup pickup)
        {
            if (_activePickups.Remove(pickup, out Transform spawnPoint) == false)
                return;

            pickup.Collected -= HandlePickupCollected;
            _occupiedPoints.Remove(spawnPoint);
            _respawnTimers.Add(_respawnDelay);
            Destroy(pickup.gameObject);
        }
    }
}
