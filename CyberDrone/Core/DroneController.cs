using UnityEngine;

namespace DroneGame.Core
{
    /// <summary>
    /// Main controller for the drone. Handles physics, movement and core functionality.
    /// </summary>
    public class DroneController : MonoBehaviour
    {
        [Header("Drone Properties")]
        [SerializeField] private float thrust = 10f;
        [SerializeField] private float pitchSpeed = 5f;
        [SerializeField] private float yawSpeed = 5f;
        [SerializeField] private float rollSpeed = 5f;
        [SerializeField] private float stabilizationSpeed = 3f;

        [Header("Physics")]
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Transform droneModel;
        [SerializeField] private float gravityMultiplier = 1f;

        // Input values
        private float thrustInput;
        private float pitchInput;
        private float yawInput;
        private float rollInput;

        private DroneInputHandler inputHandler;
        private DroneEnergySystem energySystem;

        private void Awake()
        {
            // Get components
            rb = GetComponent<Rigidbody>();
            inputHandler = GetComponent<DroneInputHandler>();
            energySystem = GetComponent<DroneEnergySystem>();

            // If no model assigned, use this transform
            if (droneModel == null)
                droneModel = transform;
        }

        private void Update()
        {
            // Get input values from input handler
            if (inputHandler != null)
            {
                thrustInput = inputHandler.ThrustInput;
                pitchInput = inputHandler.PitchInput;
                yawInput = inputHandler.YawInput;
                rollInput = inputHandler.RollInput;
            }

            // Apply visual rotation to the model
            ApplyDroneRotation();
        }

        private void FixedUpdate()
        {
            // Apply physics forces
            ApplyThrust();
            ApplyTorque();
            ApplyGravity();
            ApplyStabilization();
        }

        private void ApplyThrust()
        {
            // Apply upward thrust based on input and available energy
            if (energySystem != null && !energySystem.HasEnergy)
                return;

            float currentThrust = thrust * thrustInput;
            rb.AddForce(transform.up * currentThrust, ForceMode.Force);

            // Consume energy if system exists
            if (energySystem != null)
                energySystem.ConsumeEnergy(thrustInput * Time.fixedDeltaTime);
        }

        private void ApplyTorque()
        {
            // Apply rotational forces for pitch, yaw, and roll
            Vector3 torque = new Vector3(
                pitchInput * pitchSpeed,
                yawInput * yawSpeed,
                -rollInput * rollSpeed
            );

            rb.AddRelativeTorque(torque, ForceMode.Force);
        }

        private void ApplyGravity()
        {
            // Apply custom gravity
            rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);
        }

        private void ApplyStabilization()
        {
            // Add stabilization when no input is given
            if (Mathf.Abs(pitchInput) < 0.1f && Mathf.Abs(rollInput) < 0.1f)
            {
                // Gradually level out the drone when no rotation input
                Quaternion targetRotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, stabilizationSpeed * Time.fixedDeltaTime);
            }
        }

        private void ApplyDroneRotation()
        {
            // Apply visual rotation to the drone model for feedback
            // This is separate from physics rotation
            droneModel.localRotation = Quaternion.Euler(
                pitchInput * 15f,
                0f,
                -rollInput * 15f
            );
        }
    }
}