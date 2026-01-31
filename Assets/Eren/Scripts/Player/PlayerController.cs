using UnityEngine;

namespace WallPunch.FinalCharacterController
{
    [DefaultExecutionOrder(-1)]
    public class PlayerController : MonoBehaviour
    {
        #region Class Variables
        [Header("Components")]
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Camera _playerCamera;

        public float RotationMismatch { get; private set; } = 0f;
        public bool IsRotatingToTarget { get; private set; } = false;

        [Header("Base Movement")]
        public float runAcceleration = 35f;
        public float runSpeed = 4f;
        public float sprintAcceleration = 50f;
        public float sprintSpeed = 7f;
        public float inAirAcceleration = 25f; // Havada hareket kontrolü
        public float drag = 20f;
        public float gravity = 25f;
        public float jumpSpeed = 1.0f;
        public float movingThreshold = 0.01f;

        [Header("Animation")]
        public float playerModelRotationSpeed = 10f;
        public float rotateToTargetTime = 0.67f;

        [Header("Camera Settings")]
        public float lookSenseH = 0.1f;
        public float lookSenseV = 0.1f;
        public float lookLimitV = 89f;

        [Header("Environment Details")]
        [SerializeField] private LayerMask _groundLayers; // Yer tabakasýný seçmelisin (Default, Ground vb.)

        private PlayerLocomotionInput _playerLocomotionInput;
        private PlayerState _playerState;

        private Vector2 _cameraRotation = Vector2.zero;
        private Vector2 _playerTargetRotation = Vector2.zero;

        private bool _jumpedLastFrame = false;
        private bool _isRotatingClockwise = false;
        private float _rotatingToTargetTimer = 0f;
        private float _verticalVelocity = 0f;
        private float _antiBump; // Yokuþ aþaðý inerken zýplamayý önleyen güç
        private float _stepOffset; // Basamak çýkma yüksekliði

        private PlayerMovementState _lastMovementState = PlayerMovementState.Falling;
        #endregion

        #region Startup
        private void Awake()
        {
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            _playerState = GetComponent<PlayerState>();

            _antiBump = sprintSpeed; // Anti-bump'ý koþu hýzýna eþitlemek genelde iyi sonuç verir
            _stepOffset = _characterController.stepOffset;
        }
        #endregion

        #region Update Logic
        private void Update()
        {
            UpdateMovementState();
            HandleVerticalMovement();
            HandleLateralMovement();
        }

        private void UpdateMovementState()
        {
            _lastMovementState = _playerState.CurrentPlayerMovementState;

            bool isMovementInput = _playerLocomotionInput.MovementInput != Vector2.zero;
            bool isMovingLaterally = IsMovingLaterally();
            bool isSprinting = _playerLocomotionInput.SprintToggledOn && isMovingLaterally;
            bool isGrounded = IsGrounded();

            // Yürüme (Walking) kaldýrýldý. Varsayýlan Running.
            PlayerMovementState lateralState = isSprinting ? PlayerMovementState.Sprinting :
                                               isMovingLaterally || isMovementInput ? PlayerMovementState.Running : PlayerMovementState.Idling;

            _playerState.SetPlayerMovementState(lateralState);

            // Havada olma durumlarý
            // Eðer zýpladýysak veya yerle temasýmýz kesildiyse:
            if ((!isGrounded || _jumpedLastFrame) && _characterController.velocity.y > 0f)
            {
                _playerState.SetPlayerMovementState(PlayerMovementState.Jumping);
                _jumpedLastFrame = false;
                _characterController.stepOffset = 0f; // Havada basamak çýkmayý engelle
            }
            else if ((!isGrounded || _jumpedLastFrame) && _characterController.velocity.y <= 0f)
            {
                _playerState.SetPlayerMovementState(PlayerMovementState.Falling);
                _jumpedLastFrame = false;
                _characterController.stepOffset = 0f;
            }
            else
            {
                // Yerdeysek step offset'i geri yükle
                _characterController.stepOffset = _stepOffset;
            }
        }

        private void HandleVerticalMovement()
        {
            bool isGrounded = _playerState.InGroundedState();

            _verticalVelocity -= gravity * Time.deltaTime;

            // Yerdeysek ve aþaðý doðru çekiliyorsak, anti-bump uygula (yere yapýþtýr)
            if (isGrounded && _verticalVelocity < 0)
                _verticalVelocity = -_antiBump;

            if (_playerLocomotionInput.JumpPressed && isGrounded)
            {
                // Zýplama formülü
                _verticalVelocity += Mathf.Sqrt(jumpSpeed * 3 * gravity);
                _jumpedLastFrame = true;
            }

            // Eðer bir önceki karede yerdeysek ama þimdi havadaysak (örn: yokuþ aþaðý koþarken),
            // karakteri aþaðý iterek yere yapýþtýr.
            if (_playerState.IsStateGroundedState(_lastMovementState) && !isGrounded)
            {
                _verticalVelocity += _antiBump;
            }
        }

        private void HandleLateralMovement()
        {
            bool isSprinting = _playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;
            bool isGrounded = _playerState.InGroundedState();

            // Havadaysak farklý, yerdeysek farklý ivme kullan
            float lateralAcceleration = !isGrounded ? inAirAcceleration :
                                        isSprinting ? sprintAcceleration : runAcceleration;

            float clampLateralMagnitude = !isGrounded ? sprintSpeed :
                                          isSprinting ? sprintSpeed : runSpeed;

            Vector3 cameraForwardXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
            Vector3 cameraRightXZ = new Vector3(_playerCamera.transform.right.x, 0f, _playerCamera.transform.right.z).normalized;
            Vector3 movementDirection = cameraRightXZ * _playerLocomotionInput.MovementInput.x + cameraForwardXZ * _playerLocomotionInput.MovementInput.y;

            Vector3 movementDelta = movementDirection * lateralAcceleration * Time.deltaTime;
            Vector3 newVelocity = _characterController.velocity + movementDelta;

            // Sürtünme (Drag)
            Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
            newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
            newVelocity = Vector3.ClampMagnitude(new Vector3(newVelocity.x, 0f, newVelocity.z), clampLateralMagnitude);

            // Dikey hýzý ekle
            newVelocity.y += _verticalVelocity;

            // VIDEO 6: Eðimli duvarlardan kayma mantýðý
            newVelocity = !isGrounded ? HandleSteepWalls(newVelocity) : newVelocity;

            _characterController.Move(newVelocity * Time.deltaTime);
        }

        // VIDEO 6: Dik duvarlarda karakterin týrmanmasýný engeller, yana/aþaðý kaydýrýr.
        private Vector3 HandleSteepWalls(Vector3 velocity)
        {
            Vector3 normal = CharacterControllerUtils.GetNormalWithSphereCast(_characterController, _groundLayers);
            float angle = Vector3.Angle(normal, Vector3.up);
            bool validAngle = angle <= _characterController.slopeLimit;

            // Eðer zemin açýsý limitten fazlaysa (dikse) ve düþüyorsak:
            if (!validAngle && _verticalVelocity < 0f)
                velocity = Vector3.ProjectOnPlane(velocity, normal);

            return velocity;
        }
        #endregion

        #region Late Update (Kamera Dönüþü - Deðiþmedi)
        private void LateUpdate()
        {
            UpdateCameraRotation();
        }

        private void UpdateCameraRotation()
        {
            _cameraRotation.x += lookSenseH * _playerLocomotionInput.LookInput.x;
            _cameraRotation.y = Mathf.Clamp(_cameraRotation.y - lookSenseV * _playerLocomotionInput.LookInput.y, -lookLimitV, lookLimitV);

            _playerTargetRotation.x += transform.eulerAngles.x + lookSenseH * _playerLocomotionInput.LookInput.x;

            float rotationTolerance = 90f;
            bool isIdling = _playerState.CurrentPlayerMovementState == PlayerMovementState.Idling;
            IsRotatingToTarget = _rotatingToTargetTimer > 0;

            if (!isIdling)
            {
                RotatePlayerToTarget();
            }
            else if (Mathf.Abs(RotationMismatch) > rotationTolerance || IsRotatingToTarget)
            {
                UpdateIdleRotation(rotationTolerance);
            }

            _playerCamera.transform.rotation = Quaternion.Euler(_cameraRotation.y, _cameraRotation.x, 0f);

            Vector3 camForwardProjectedXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
            Vector3 crossProduct = Vector3.Cross(transform.forward, camForwardProjectedXZ);
            float sign = Mathf.Sign(Vector3.Dot(crossProduct, transform.up));
            RotationMismatch = sign * Vector3.Angle(transform.forward, camForwardProjectedXZ);
        }

        private void UpdateIdleRotation(float rotationTolerance)
        {
            if (Mathf.Abs(RotationMismatch) > rotationTolerance)
            {
                _rotatingToTargetTimer = rotateToTargetTime;
                _isRotatingClockwise = RotationMismatch > rotationTolerance;
            }
            _rotatingToTargetTimer -= Time.deltaTime;

            if (_isRotatingClockwise && RotationMismatch > 0f ||
                !_isRotatingClockwise && RotationMismatch < 0f)
            {
                RotatePlayerToTarget();
            }
        }

        private void RotatePlayerToTarget()
        {
            Quaternion targetRotationX = Quaternion.Euler(0f, _playerTargetRotation.x, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotationX, playerModelRotationSpeed * Time.deltaTime);
        }
        #endregion

        #region State Checks
        private bool IsMovingLaterally()
        {
            Vector3 lateralVelocity = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.z);
            return lateralVelocity.magnitude > movingThreshold;
        }

        private bool IsGrounded()
        {
            // Ýki kontrol yapýyoruz: 1. Fiziksel temas var mý? 2. Temas varsa bile o zemin düz mü (yürünebilir mi)?
            bool grounded = _playerState.InGroundedState() ? IsGroundedWhileGrounded() : IsGroundedWhileAirborne();
            return grounded;
        }

        private bool IsGroundedWhileGrounded()
        {
            
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _characterController.radius, transform.position.z);

            float tolerance = 0.05f;

            bool grounded = Physics.CheckSphere(spherePosition, _characterController.radius + tolerance, _groundLayers, QueryTriggerInteraction.Ignore);

            return grounded;
        }

        // VIDEO 6: Havadaysak, yere inmeye çalýþýrken eðimi kontrol et
        private bool IsGroundedWhileAirborne()
        {
            Vector3 normal = CharacterControllerUtils.GetNormalWithSphereCast(_characterController, _groundLayers);
            float angle = Vector3.Angle(normal, Vector3.up);
            bool validAngle = angle <= _characterController.slopeLimit;

            // CharacterController yere deðiyor diyorsa VE zemin açýsý yürünebilir ise "Grounded" say.
            return _characterController.isGrounded && validAngle;
        }
        #endregion
    }
}