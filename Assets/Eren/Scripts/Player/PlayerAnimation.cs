using System;
using UnityEngine;

namespace WallPunch.FinalCharacterController
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        private PlayerLocomotionInput _playerLocomotionInput;

        private static int inputXHash = Animator.StringToHash("inputX");
        private static int inputYHash = Animator.StringToHash("inputY");

        private void Awake()
        {
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
        }

        private void Update()
        {
            UpdateAnimationState();
        }

        private void UpdateAnimationState()
        {
            Vector2 inputTarget = _playerLocomotionInput.MovementInput;

            _animator.SetFloat(inputXHash, inputTarget.x ,0.1f, Time.deltaTime);
            _animator.SetFloat(inputYHash, inputTarget.y, 0.1f, Time.deltaTime);
        }
    }
}

