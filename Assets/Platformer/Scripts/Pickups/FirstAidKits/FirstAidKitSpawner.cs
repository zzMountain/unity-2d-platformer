using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class FirstAidKitSpawner : MonoBehaviour
    {
        private readonly Dictionary<FirstAidKit, Transform> _activeFirstAidKits =
            new Dictionary<FirstAidKit, Transform>();
        private readonly HashSet<Transform> _occupiedPoints = new HashSet<Transform>();
        private readonly List<float> _respawnTimers = new List<float>();

        [SerializeField] private FirstAidKit _firstAidKitPrefab;
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private int _maxActiveFirstAidKits = 3;
        [SerializeField] private float _respawnDelay = 8f;

        private void Start()
        {
            int initialFirstAidKitCount = Mathf.Min(_maxActiveFirstAidKits, _spawnPoints.Length);

            for (int i = 0; i < initialFirstAidKitCount; i++)
                SpawnFirstAidKit();
        }

        private void Update()
        {
            for (int i = _respawnTimers.Count - 1; i >= 0; i--)
            {
                _respawnTimers[i] -= Time.deltaTime;

                if (_respawnTimers[i] > 0f)
                    continue;

                _respawnTimers.RemoveAt(i);
                SpawnFirstAidKit();
            }
        }

        private void OnDestroy()
        {
            foreach (FirstAidKit firstAidKit in _activeFirstAidKits.Keys)
                firstAidKit.Collected -= OnFirstAidKitCollected;
        }

        private void SpawnFirstAidKit()
        {
            if (_activeFirstAidKits.Count >= _maxActiveFirstAidKits)
                return;

            Transform spawnPoint = GetAvailableSpawnPoint();

            if (spawnPoint == null)
                return;

            FirstAidKit firstAidKit = Instantiate(
                _firstAidKitPrefab,
                spawnPoint.position,
                Quaternion.identity,
                transform);
            firstAidKit.Collected += OnFirstAidKitCollected;
            _activeFirstAidKits.Add(firstAidKit, spawnPoint);
            _occupiedPoints.Add(spawnPoint);
        }

        private Transform GetAvailableSpawnPoint()
        {
            if (_occupiedPoints.Count >= _spawnPoints.Length)
                return null;

            int startIndex = UnityEngine.Random.Range(0, _spawnPoints.Length);

            for (int offset = 0; offset < _spawnPoints.Length; offset++)
            {
                int index = (startIndex + offset) % _spawnPoints.Length;
                Transform spawnPoint = _spawnPoints[index];

                if (_occupiedPoints.Contains(spawnPoint) == false)
                    return spawnPoint;
            }

            return null;
        }

        private void OnFirstAidKitCollected(FirstAidKit firstAidKit)
        {
            if (_activeFirstAidKits.Remove(firstAidKit, out Transform spawnPoint) == false)
                return;

            firstAidKit.Collected -= OnFirstAidKitCollected;
            _occupiedPoints.Remove(spawnPoint);
            _respawnTimers.Add(_respawnDelay);
            Destroy(firstAidKit.gameObject);
        }
    }
}
