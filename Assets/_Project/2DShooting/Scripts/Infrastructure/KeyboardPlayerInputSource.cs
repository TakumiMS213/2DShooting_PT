using TwoDShooting.Proto.Application;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TwoDShooting.Proto.Infrastructure
{
    public sealed class KeyboardPlayerInputSource : MonoBehaviour, IPlayerInputSource
    {
        private Keyboard _keyboard;

        public Vector2 Move { get; private set; }
        public bool FirePressed { get; private set; }

        private void OnEnable()
        {
            _keyboard = Keyboard.current;
        }

        private void Update()
        {
            if (_keyboard == null)
            {
                Move = Vector2.zero;
                FirePressed = false;
                return;
            }

            var x = ReadAxis(_keyboard.leftArrowKey.isPressed || _keyboard.aKey.isPressed, _keyboard.rightArrowKey.isPressed || _keyboard.dKey.isPressed);
            var y = ReadAxis(_keyboard.downArrowKey.isPressed || _keyboard.sKey.isPressed, _keyboard.upArrowKey.isPressed || _keyboard.wKey.isPressed);

            Move = new Vector2(x, y).normalized;
            FirePressed = _keyboard.spaceKey.isPressed || _keyboard.zKey.isPressed;
        }

        private static float ReadAxis(bool negative, bool positive)
        {
            if (negative == positive)
            {
                return 0f;
            }

            return positive ? 1f : -1f;
        }
    }
}
