using Core;
using UnityEngine;

namespace Core
{
    public class DroneController : MonoBehaviour
    {
        [Header("Drone Properties")]
        [SerializeField] private float thrust = 25f;
        [SerializeField] private float pitchSpeed = 3f;
        [SerializeField] private float yawSpeed = 0.0625f;
        [SerializeField] private float rollSpeed = 0.225f;
        [SerializeField] private float speedCof = 0.2f;

        private Rigidbody rb;
        private DroneInputHandler inputHandler;
        [SerializeField] private Transform droneBottom;
        [SerializeField] private Transform cameraTransform;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            inputHandler = GetComponent<DroneInputHandler>();
        }

        private void FixedUpdate()
        {
            ApplyThrust();
            ApplyTorque();
            ApplyDrag();
        }

        private void ApplyThrust()
        {
            Vector3 thrustDirection = (droneBottom.position - transform.position).normalized;
            float thrustForce = inputHandler.ThrustInput * thrust;
            rb.AddForce(thrustDirection * thrustForce, ForceMode.Force);
        }

        private void ApplyTorque()
        {
            Vector3 torque = new Vector3(
                inputHandler.PitchInput * pitchSpeed,
                inputHandler.YawInput * yawSpeed,
                -inputHandler.RollInput * rollSpeed
            );
            rb.AddRelativeTorque(torque, ForceMode.Force);
        }

        private void ApplyDrag()
        {
            // Створюємо опір на базі швидкості
            Vector3 dragForce = -rb.linearVelocity * speedCof;
            rb.AddForce(dragForce, ForceMode.Force);

            // Додавання опору на обертання
            Vector3 angularDrag = -rb.angularVelocity * 0.01f; // Можна налаштувати значення 0.1f для налаштування рівня опору
            rb.AddTorque(angularDrag, ForceMode.Force);
        }
    }
}