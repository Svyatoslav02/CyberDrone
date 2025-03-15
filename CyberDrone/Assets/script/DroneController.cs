using UnityEngine;

namespace Core
{
    public class DroneController : MonoBehaviour
    {
        [Header("Drone Properties")]
        [SerializeField] private float thrust = 15f;
        [SerializeField] private float pitchSpeed = 3f;
        [SerializeField] private float yawSpeed = 1f;
        [SerializeField] private float rollSpeed = 0.1f;
        [SerializeField] private float speedCof = 1.8f;

        // Ссылки на компоненты
        private Rigidbody rb;
        private DroneInputHandler inputHandler;
        [SerializeField] private Transform droneBottom;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            inputHandler = GetComponent<DroneInputHandler>();
        }

        private void FixedUpdate()
        {
            // Проверка компонентов
            if (rb == null || inputHandler == null || droneBottom == null) return;

            ApplyThrust();
            ApplyTorque();
            if (Input.GetKey(KeyCode.Space))rb.linearVelocity *= (1f - speedCof * Time.deltaTime);;
        }

        private void ApplyThrust()
        {
            Vector3 thrustDirection = (droneBottom.position - transform.position).normalized;
            float thrustForce = inputHandler.ThrustInput * thrust;
            rb.AddForce(thrustDirection * thrustForce, ForceMode.Force);

        }

        private void ApplyTorque()
        {
            // Применяем вращающие силы
            Vector3 torque = new Vector3(
                inputHandler.PitchInput * pitchSpeed,
                inputHandler.YawInput * yawSpeed,
                -inputHandler.RollInput * rollSpeed
            );
            rb.AddRelativeTorque(torque, ForceMode.Force);
        }
    }
}
