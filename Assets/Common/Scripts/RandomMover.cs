using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMover : MonoBehaviour
{
    [SerializeField]
    private float _speed = 1f;
    private float _rotateSpeed = 10f;
    [SerializeField]
    private float _minX = 0, _minY = 0, _minZ = 0;
    [SerializeField]
    bool _lockX = false, _lockY = true, _lockZ = false;
    [SerializeField]
    private float _maxX = 0, _maxY = 0, _maxZ = 0;
    private Vector3 _targetPosition;
    private Vector3 _startPosition;
    private bool _isTargetPositionChoosed = false;
    private float _targetMovingDeltaTime;
    private void Update()
    {
        if (!_isTargetPositionChoosed)
        {
            _targetPosition.x = _lockX ? transform.position.x : Random.Range(_minX, _maxX);
            _targetPosition.y = _lockY ? transform.position.y : Random.Range(_minY, _maxY);
            _targetPosition.z = _lockZ ? transform.position.z : Random.Range(_minZ, _maxZ);
            _targetMovingDeltaTime = 0;
            _startPosition = transform.position;
            _isTargetPositionChoosed = true;
        }
        else if (_targetMovingDeltaTime < 1 && _targetPosition != transform.position)
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
}
