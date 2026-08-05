using UnityEngine;
using UnityEngine.InputSystem;

namespace Platformer
{
    public class PlayerInputReader : MonoBehaviour
    {
        private bool _isJumpRequested;

        public float MovementDirection { get; private set; }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;

            if (keyboard == null)
            {
                MovementDirection = 0f;
                return;
            }

            float direction = 0f;

            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                direction -= 1f;

            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                direction += 1f;

            MovementDirection = Mathf.Clamp(direction, -1f, 1f);

            if (keyboard.spaceKey.wasPressedThisFrame ||
                keyboard.wKey.wasPressedThisFrame ||
                keyboard.upArrowKey.wasPressedThisFrame)
            {
                _isJumpRequested = true;
            }
        }

        public bool ConsumeJumpRequest()
        {
            bool isRequested = _isJumpRequested;
            _isJumpRequested = false;
            return isRequested;
        }
    }
}
