using UnityEngine;

namespace Assets.Scenes.DoozyCatcherScene
{
    public class Movement : MonoBehaviour
    {
        public float MaximumSpeed = 3.5f, RotationDegreeSpeed = 360.0f, JumpHeight = 2.0f, JumpHorizontalSpeed = 3.0f, GravityMultiplier = 1.5f;
        public float JumpButtonGracePeriod = 0.2f;
        private CharacterController _characterController;
        private Animator _animator;
        private float _jumpDownSpeed;
        private bool _isJumping, _isGrounded, _isSliding;
        private Vector3 _slopeSlideVelocity;
        private float _characterControllerStepOffset;
        private float? _lastGroundedTime, _jumpButtonPressedTime;
        [SerializeField]
        private Transform _cameraTransform;
        public bool UseRootMotion = false;
        private void SetSlopeSlideVelocity()
        {
            if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out var hitInfo, 5.0f))
            {
                float slopeAngle = Vector3.Angle(hitInfo.normal, Vector3.up);
                if (slopeAngle >= _characterController.slopeLimit)
                {
                    _slopeSlideVelocity = Vector3.ProjectOnPlane(new Vector3(0, _jumpDownSpeed, 0), hitInfo.normal);
                    return;
                }
            }
            if (_isSliding)
            {
                _slopeSlideVelocity -= _slopeSlideVelocity * Time.deltaTime * 3;
                if (_slopeSlideVelocity.magnitude > 1)
                {
                    return;
                }
            }
            _slopeSlideVelocity = Vector3.zero;
        }
        // Start is called before the first frame update
        void Start()
        {
            _characterController = GetComponent<CharacterController>();
            _characterControllerStepOffset = _characterController.stepOffset;
            _animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            bool isGrounded = _characterController.isGrounded;

            Vector3 direction = new(horizontalInput, 0, verticalInput);
            direction = Quaternion.AngleAxis(_cameraTransform.rotation.eulerAngles.y, Vector3.up) * direction;
            float inputMagnitude = Mathf.Clamp(direction.magnitude, 0, 0.5f);
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                inputMagnitude *= 2;
            }
            direction.Normalize();

            _animator.SetFloat("Input Magnitude", inputMagnitude, 0.5f, Time.deltaTime);

            float gravity = Physics.gravity.y * GravityMultiplier;
            if (_isJumping && _jumpDownSpeed > 0 && !Input.GetButton("Jump")) // если короткое нажатие
            {
                gravity *= 2; // ускоряем прыжок
            }
            _jumpDownSpeed += gravity * Time.deltaTime;
            SetSlopeSlideVelocity();
            if (_slopeSlideVelocity == Vector3.zero)
            {
                _isSliding = false;
            }

            if (isGrounded)
            {
                _lastGroundedTime = Time.time;
            }
            else
            {
                _characterController.stepOffset = 0;
            }
            if (Input.GetButtonDown("Jump"))
            {
                _jumpButtonPressedTime = Time.time;
            }
            if (Time.time - _lastGroundedTime <= JumpButtonGracePeriod)
            {
                if (_slopeSlideVelocity != Vector3.zero)
                {
                    _isSliding = true;
                }
                _characterController.stepOffset = _characterControllerStepOffset;
                if (!_isSliding)
                {
                    _jumpDownSpeed = -0.5f;
                }
                _animator.SetBool("IsGrounded", true);
                _isGrounded = true;
                _animator.SetBool("IsJumping", false);
                _isJumping = false;
                _animator.SetBool("IsFalling", false);

                if (Time.time - _jumpButtonPressedTime <= JumpButtonGracePeriod && !_isSliding)
                {
                    _animator.SetBool("IsJumping", true);
                    _isJumping = true;
                    _jumpDownSpeed = Mathf.Sqrt(JumpHeight * -3 * gravity);
                    _jumpButtonPressedTime = null;
                    _lastGroundedTime = null;
                }
            }
            else
            {
                _animator.SetBool("IsGrounded", false);
                _isGrounded = false;
                if ((_isJumping && _jumpDownSpeed < 0) || _jumpDownSpeed < -2)
                {
                    _animator.SetBool("IsFalling", true);
                }
            }

            if (!UseRootMotion)
            {
                float speed = inputMagnitude * MaximumSpeed;
                Vector3 velocity = speed * direction;
                velocity.y = _jumpDownSpeed;
                _characterController.Move(velocity * Time.deltaTime);
            }
            if (!_isGrounded && !_isSliding)
            {
                float speed = inputMagnitude * JumpHorizontalSpeed;
                Vector3 velocity = speed * direction;
                velocity.y = _jumpDownSpeed;
                _characterController.Move(velocity * Time.deltaTime);
            }
            if (_isSliding)
            {
                Vector3 velocity = _slopeSlideVelocity;
                velocity.y = _jumpDownSpeed;
                _characterController.Move(velocity * Time.deltaTime);
            }

            if (direction != Vector3.zero)
            {
                Quaternion rotationQuaternion = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationQuaternion, RotationDegreeSpeed * Time.deltaTime);
                _animator.SetBool("IsMoving", true);
            }
            else
            {
                //Debug.Log("Direction is zero!");
                _animator.SetBool("IsMoving", false);
            }
        }
        private void OnAnimatorMove()
        {
            if (UseRootMotion && _isGrounded && !_isSliding)
            {
                Vector3 velocity = _animator.deltaPosition;
                velocity.y = _jumpDownSpeed * Time.deltaTime;
                _characterController.Move(velocity);
            }
        }
    }
}
