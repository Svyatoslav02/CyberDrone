using UnityEngine;

namespace Core
{
    public class DroneController : MonoBehaviour
    {
        [Header("Drone Properties")]
        [SerializeField] private float thrust = 15f;
        [SerializeField] private float pitchSpeed = 5f;
        [SerializeField] private float yawSpeed = 5f;
        [SerializeField] private float rollSpeed = 5f;
        
        // Ссылки на компоненты
        private Rigidbody rb;
        private DroneInputHandler inputHandler;
        private DroneEnergySystem energySystem;

        private void Awake()
        {
            // Получаем компоненты
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("Отсутствует Rigidbody на объекте дрона!");
                rb = gameObject.AddComponent<Rigidbody>();
                rb.mass = 1f;
                rb.linearDamping = 0.5f;
                rb.angularDamping = 0.5f;
            }
            
            inputHandler = GetComponent<DroneInputHandler>();
            energySystem = GetComponent<DroneEnergySystem>();
        }

        private void Update()
        {
            // Вывод информации о силах
            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log($"Thrust Force: {thrust * inputHandler.ThrustInput}");
                Debug.Log($"Torque: ({pitchSpeed * inputHandler.PitchInput}, {yawSpeed * inputHandler.YawInput}, {rollSpeed * inputHandler.RollInput})");
                Debug.Log($"Current Pos Y: {transform.position.y}");
            }
        }

        private void FixedUpdate()
        {
            // Проверка компонентов
            if (rb == null || inputHandler == null) return;
            
            // Проверка энергии
            if (energySystem != null && !energySystem.HasEnergy) return;
            
            // Применяем физические силы
            ApplyThrust();
            ApplyTorque();
            
            // Предотвращаем падение ниже нуля
            if (transform.position.y < 0.5f)
            {
                Vector3 pos = transform.position;
                pos.y = 0.5f;
                transform.position = pos;
                
                // Останавливаем падение
                if (rb.linearVelocity.y < 0)
                {
                    Vector3 vel = rb.linearVelocity;
                    vel.y = 0;
                    rb.linearVelocity = vel;
                }
            }
        }

        private void ApplyThrust()
        {
            // Применяем тягу вверх/вниз
            float thrustForce = inputHandler.ThrustInput * thrust;
            rb.AddForce(Vector3.up * thrustForce, ForceMode.Force);
            
            // Добавляем стабилизацию, чтобы дрон не падал слишком быстро
            if (inputHandler.ThrustInput < 0.1f)
            {
                // Компенсация гравитации
                rb.AddForce(Vector3.up * Physics.gravity.magnitude * 0.7f * rb.mass, ForceMode.Force);
            }
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