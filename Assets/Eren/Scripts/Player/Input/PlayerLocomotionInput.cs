using UnityEngine;
using UnityEngine.InputSystem;

namespace WallPunch.FinalCharacterController
{
    [DefaultExecutionOrder(-2)]
    public class PlayerLocomotionInput : MonoBehaviour, PlayerControls.IPlayerLocomotionMapActions
    {
        [Header("Settings")]
        [SerializeField] private bool holdToSprint = true;

        // Artýk PlayerControls'ü kendimiz oluþturmuyoruz, Manager'dan alacaðýz.
        public PlayerControls PlayerControls { get; private set; }
        public Vector2 MovementInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool SprintToggledOn { get; private set; }

        private void OnEnable()
        {
            // GÜNCELLEME BURADA: InputManager'ý bekliyoruz
            if (PlayerInputManager.Instance?.PlayerControls == null)
            {
                Debug.LogError("Player controls baþlatýlmadý - enable edilemiyor");
                return;
            }

            // Manager'daki kontrollere abone oluyoruz
            PlayerInputManager.Instance.PlayerControls.PlayerLocomotionMap.Enable();
            PlayerInputManager.Instance.PlayerControls.PlayerLocomotionMap.SetCallbacks(this);
        }

        private void OnDisable()
        {
            if (PlayerInputManager.Instance?.PlayerControls == null) return;

            PlayerInputManager.Instance.PlayerControls.PlayerLocomotionMap.Disable();
            PlayerInputManager.Instance.PlayerControls.PlayerLocomotionMap.RemoveCallbacks(this);
        }

        private void LateUpdate()
        {
            JumpPressed = false;
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            MovementInput = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }

        public void OnToggleSprint(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                SprintToggledOn = holdToSprint || !SprintToggledOn;
            }
            else if (context.canceled)
            {
                SprintToggledOn = !holdToSprint && SprintToggledOn;
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            JumpPressed = true;
        }

        public void OnToggleWalk(InputAction.CallbackContext context) { }
    }
}