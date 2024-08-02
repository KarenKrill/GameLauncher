using UnityEngine;

namespace Assets.Scenes.DoozyCatcherScene
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField]
        PlayerController _playerController;
        [SerializeField]
        GameObject _enemyPrefab;
        [SerializeField]
        private float _y;
        [SerializeField]
        private float _minX, _minZ;
        [SerializeField]
        private float _maxX, _maxZ;
        // Start is called before the first frame update
        void Start()
        {
            if (_playerController != null)
            {
                _playerController.EnemyCaught += OnEnemyCaught;
            }
        }

        private void OnEnemyCaught()
        {
            Vector3 newEnemyPosition = new()
            {
                x = Random.Range(_minX, _maxX),
                z = Random.Range(_minZ, _maxZ),
                y = _y
            };
            var newEnemy = Instantiate(_enemyPrefab, newEnemyPosition, Quaternion.identity);
        }
    }
}
