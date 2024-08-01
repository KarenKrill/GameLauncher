using UnityEngine;

namespace Assets.Scenes.GoblinClickerScene
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField]
        private float _speed = 1f;
        private float _rotateSpeed = 10f;
        [SerializeField]
        private float _minX, _minY;
        [SerializeField]
        private float _maxX, _maxY;
        [SerializeField]
        private Vector3 _targetPosition;
        private Vector3 _startPosition;
        [SerializeField]
        private bool _isTargetPositionChoosed = false;
        private float _targetMovingDeltaTime;
        private void Update()
        {
            if (!_isTargetPositionChoosed)
            {
                _targetPosition.x = Random.Range(_minX, _maxX);
                _targetPosition.z = Random.Range(_minY, _maxY);
                _targetPosition.y = transform.position.y;
                _targetMovingDeltaTime = 0;
                _startPosition = transform.position;
                _isTargetPositionChoosed = true;
            }
            else if (_targetMovingDeltaTime < 1 && (_targetPosition.x != transform.position.x || _targetPosition.z != transform.position.z))
            {
                _targetMovingDeltaTime += Time.deltaTime * _speed;
                var singleRotateStep = Time.deltaTime * _rotateSpeed;
                Vector3 targetDirection = _targetPosition - transform.position;
                var newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleRotateStep, 0.0f);
                transform.SetPositionAndRotation(Vector3.Lerp(_startPosition, _targetPosition, _targetMovingDeltaTime), Quaternion.LookRotation(newDirection));
            }
            else if (Random.Range(0, 25) == 0)
            {
                _isTargetPositionChoosed = false;
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            
        }
    }
}
