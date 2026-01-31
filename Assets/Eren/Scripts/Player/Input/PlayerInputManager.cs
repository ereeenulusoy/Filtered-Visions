using UnityEngine;

namespace WallPunch.FinalCharacterController
{
    // Bu script diðerlerinden önce çalýþsýn (-3)
    [DefaultExecutionOrder(-3)]
    public class PlayerInputManager : MonoBehaviour
    {
        // Singleton Yapýsý (Tek Patron)
        public static PlayerInputManager Instance;
        public PlayerControls PlayerControls { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            PlayerControls = new PlayerControls();
            PlayerControls.Enable();
        }

        private void OnDisable()
        {
            PlayerControls.Disable();
        }
    }
}