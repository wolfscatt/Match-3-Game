using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Match3Game.Infrastructure.Input
{
    /// <summary>
    /// Unity Input System ile oyuncu dokunuşunu/tıklamasını yakalar.
    /// Presentation katmanına event fırlatır -- domain'i bilmez.
    /// </summary>
    public class InputHandler : MonoBehaviour
    {
        public event Action<Vector2> OnTilePressed;
        public event Action<Vector2> OnTileReleased;

        private bool _isEnabled = true;
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        public void Enable() => _isEnabled = true;
        public void Disable() => _isEnabled = false;

        private void Update()
        {
            if(!_isEnabled) return;

            // Mouse
            if(Mouse.current != null)
            {
                if(Mouse.current.leftButton.wasPressedThisFrame)
                    OnTilePressed?.Invoke(GetWorldPosition(Mouse.current.position.ReadValue()));

                if(Mouse.current.leftButton.wasReleasedThisFrame)
                    OnTileReleased?.Invoke(GetWorldPosition(Mouse.current.position.ReadValue()));
            }

            // Touch
            if(Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                var touch = Touchscreen.current.primaryTouch;

                if(touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
                    OnTilePressed?.Invoke(GetWorldPosition(touch.position.ReadValue()));

                if(touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Ended)
                    OnTileReleased?.Invoke(GetWorldPosition(touch.position.ReadValue()));
            }
        }

        private Vector2 GetWorldPosition(Vector2 screenPos)
        {
            var world = _mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
            return new Vector2(world.x, world.y);
        }
    }

}
