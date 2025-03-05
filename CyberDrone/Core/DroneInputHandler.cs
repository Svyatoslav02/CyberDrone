using UnityEngine;
using UnityEngine.InputSystem;

namespace DroneGame.Core
{
    /// <summary>
    /// Handles user input for the drone controller.
    /// This class uses the new Unity Input System.
    /// </summary>
    public class DroneInputHandler : MonoBehaviour
    {
        [Header("Input Properties")]
        [SerializeField] private float inputSmoothingFactor = 0.1f;

        // Input properties
        public float ThrustInput { get; private set; }
        public float PitchInput { get; private set; }
        public float YawInput { get; private set; }
        public float RollInput { get; private set; }

        // Target values for smooth input
        private float targetThrust;
        private float targetPitch;
        private float targetYaw;
        private float targetRoll;

        // Input Action references
        private InputAction thrustAction;
        private InputAction pitchYawAction;
        private InputAction rollAction;

        private PlayerInput playerInput;

        private void Awake()
        {
            // Get the PlayerInput component
            playerInput = GetComponent<PlayerInput>();

            // Get input actions
            if (playerInput != null)
            {
                thrustAction = playerInput.actions["Thrust"];
                pitchYawAction = playerInput.actions["PitchYaw"];
                rollAction = playerInput.actions["Roll"];
            }
            else
            {
                Debug.LogError("PlayerInput component not found on DroneInputHandler");
            }
        }

        private void Update()
        {
            // Read input values
            ReadInputValues();

            // Apply smoothing to inputs
            SmoothInputValues();
        }

        private void ReadInputValues()
        {
            if (thrustAction != null && pitchYawAction != null && rollAction != null)
            {
                // Read thrust
                targetThrust = thrustAction.ReadValue<float>();

                // Read pitch and yaw (usually from joystick or WASD)
                Vector2 pitchYawValue = pitchYawAction.ReadValue<Vector2>();
                targetPitch = pitchYawValue.y;
                targetYaw = pitchYawValue.x;

                // Read roll (usually from Q/E keys or triggers)
                targetRoll = rollAction.ReadValue<float>();
            }
            else
            {
                // Fallback to legacy input system
                targetThrust = Input.GetAxis("Vertical");
                targetPitch = Input.GetAxis("Vertical");
                targetYaw = Input.GetAxis("Horizontal");
                targetRoll = Input.GetAxis("Roll"); // Custom axis would need to be defined
            }
        }

        private void SmoothInputValues()
        {
            // Apply smoothing for more natural drone control
            ThrustInput = Mathf.Lerp(ThrustInput, targetThrust, inputSmoothingFactor);
            PitchInput = Mathf.Lerp(PitchInput, targetPitch, inputSmoothingFactor);
            YawInput = Mathf.Lerp(YawInput, targetYaw, inputSmoothingFactor);
            RollInput = Mathf.Lerp(RollInput, targetRoll, inputSmoothingFactor);
        }
    }
}