using UnityEngine;

namespace Core
{
    public class DroneController : MonoBehaviour
    {
        [Header("Drone Properties")]
        [SerializeField] private float thrust = 15f;
        [SerializeField] private float pitchSpeed = 3f;
        [SerializeField] private float yawSpeed = 1f;
        [SerializeField] private float rollSpeed = 0.5f;
        
        // Ссылки на компоненты
        private Rigidbody rb;
        private DroneInputHandler inputHandler;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();            
            inputHandler = GetComponent<DroneInputHandler>();
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
            
            // Применяем физические силы
            ApplyThrust();
            ApplyTorque();
            DroneMove();
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

            if (transform.position.y < 1.5f)
            {
                Vector3 pos = transform.position;
                pos.y = 1.5f;
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

        private void DroneMove()
        {
            // Отримуємо поточний нахил дрона по осях Pitch (вперед/назад) і Roll (вліво/вправо)
            float pitchAngle = Vector3.Dot(transform.forward, Vector3.down);
            float rollAngle = Vector3.Dot(transform.right, Vector3.down);

            // Переводимо в градуси для порівняння
            float pitchDegrees = Mathf.Asin(pitchAngle) * Mathf.Rad2Deg;
            float rollDegrees = Mathf.Asin(rollAngle) * Mathf.Rad2Deg;

            // Перевірка чи нахил більше порогового значення (7.5 градусів)
            if (Mathf.Abs(pitchDegrees) > 7.5f || Mathf.Abs(rollDegrees) > 7.5f)
            {
                // Розраховуємо рух вперед та вбік на основі нахилу
                Vector3 forwardMovement = transform.forward * pitchAngle;
                Vector3 rightMovement = transform.right * rollAngle;

                // Задаємо загальний напрямок руху
                Vector3 movementDirection = forwardMovement + rightMovement;
                movementDirection.Normalize();

                // Додаємо силу для руху
                rb.AddForce(movementDirection * thrust, ForceMode.Force);
            }
            else
            {
                // Поступово зменшуємо швидкість до нуля
                rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.fixedDeltaTime * 2f);
            }
        }

    }
}