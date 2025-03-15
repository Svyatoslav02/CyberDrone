using UnityEngine;

namespace Core
{
    public class DroneInputHandler : MonoBehaviour
    {
        [Header("Input Settings")]
        [SerializeField] private float inputSmoothingFactor = 0.1f;
        
        // Публичные свойства
        public float ThrustInput { get; private set; }
        public float PitchInput { get; private set; }
        public float YawInput { get; private set; }
        public float RollInput { get; private set; }
        
        // Целевые значения для сглаживания
        private float targetThrust;
        private float targetPitch;
        private float targetYaw;
        private float targetRoll;
        
        
        private void FixedUpdate()
        {
            // Получаем сырой ввод
            GetRawInput();
            
            // Сглаживаем ввод
            SmoothInput();
            
            // Отладка
            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log($"Raw Input - Thrust: {targetThrust}, Pitch: {targetPitch}, Roll: {targetRoll}, Yaw: {targetYaw}");
                Debug.Log($"Smoothed Input - Thrust: {ThrustInput}, Pitch: {PitchInput}, Roll: {RollInput}, Yaw: {YawInput}");
            }
        }
        
        private void GetRawInput()
        {
            // Тяга вверх/вниз
            targetThrust = 0f;
            if (Input.GetKey(KeyCode.UpArrow)) targetThrust += 1.0f;
            if (Input.GetKey(KeyCode.DownArrow)) targetThrust -= 1.0f;
            if (Input.GetKey(KeyCode.LeftShift)) targetThrust += 2.0f;
            
            // Наклон вперед/назад
            targetPitch = 0f;
            if (Input.GetKey(KeyCode.W)) targetPitch += 1.0f;
            if (Input.GetKey(KeyCode.S)) targetPitch -= 1.0f;
            
            // Наклон влево/вправо
            targetRoll = 0f;
            if (Input.GetKey(KeyCode.D)) targetRoll += 1.0f;
            if (Input.GetKey(KeyCode.A)) targetRoll -= 1.0f;
            
            // Поворот влево/вправо
            targetYaw = 0f;
            if (Input.GetKey(KeyCode.RightArrow)) targetYaw += 1.0f;
            if (Input.GetKey(KeyCode.LeftArrow)) targetYaw -= 1.0f;
        }
        
        private void SmoothInput()
        {
            // Сглаживаем ввод для более плавного управления
            ThrustInput = Mathf.Lerp(ThrustInput, targetThrust, inputSmoothingFactor);
            PitchInput = Mathf.Lerp(PitchInput, targetPitch, inputSmoothingFactor);
            RollInput = Mathf.Lerp(RollInput, targetRoll, inputSmoothingFactor);
            YawInput = Mathf.Lerp(YawInput, targetYaw, inputSmoothingFactor);
        }
    }
}