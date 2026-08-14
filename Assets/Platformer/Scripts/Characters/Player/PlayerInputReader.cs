using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Platformer
{
    public class PlayerInputReader : MonoBehaviour
    {
        private bool _isJumpRequested;
        private bool _isAttackRequested;

        public event Action DrainRequested;

        public float MovementDirection { get; private set; }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            float direction = 0f;

            if (keyboard != null)
            {
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                    direction -= 1f;

                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                    direction += 1f;

                if (keyboard.spaceKey.wasPressedThisFrame ||
                    keyboard.wKey.wasPressedThisFrame ||
                    keyboard.upArrowKey.wasPressedThisFrame)
                {
                    _isJumpRequested = true;
                }

                if (keyboard.fKey.wasPressedThisFrame)
                    _isAttackRequested = true;

                if (keyboard.qKey.wasPressedThisFrame)
                    DrainRequested?.Invoke();
            }

            MovementDirection = Mathf.Clamp(direction, -1f, 1f);

            Mouse mouse = Mouse.current;

            if (mouse != null && mouse.leftButton.wasPressedThisFrame)
                _isAttackRequested = true;
        }

        public bool ConsumeAttackRequest()
        {
            bool isRequested = _isAttackRequested;
            _isAttackRequested = false;
            return isRequested;
        }

        public bool ConsumeJumpRequest()
        {
            bool isRequested = _isJumpRequested;
            _isJumpRequested = false;
            return isRequested;
        }
    }
}
