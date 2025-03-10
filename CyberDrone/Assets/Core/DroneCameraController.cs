using UnityEngine;

namespace Core
{
    public class DroneCameraController : MonoBehaviour
    {
        [Header("Camera References")]
        public Camera mainCamera;
        public Transform droneTransform;
        public Transform fpvCameraPosition;

        [Header("Camera Settings")]
        [SerializeField] private float followSpeed = 5f;
        [SerializeField] private float fpvDistance = 0.5f;
        [SerializeField] private float thirdPersonDistance = 5f;
        [SerializeField] private float thirdPersonHeight = 2f;
        [SerializeField] private float topDownHeight = 10f;

        // Enum для режимов камеры
        public enum CameraMode
        {
            FirstPerson,
            ThirdPerson,
            TopDown,
            Orbit
        }

        [SerializeField] private CameraMode currentMode = CameraMode.ThirdPerson;
        
        private Transform cameraTarget;
        private Vector3 velocity = Vector3.zero;

        private void Start()
        {
            // Если камера не назначена, берем главную
            if (mainCamera == null)
                mainCamera = Camera.main;
                
            // Создаем объект для следования
            if (cameraTarget == null)
            {
                GameObject targetObj = new GameObject("Camera Target");
                cameraTarget = targetObj.transform;
                
                if (droneTransform != null)
                    cameraTarget.position = droneTransform.position;
            }
            
            // Сразу устанавливаем режим камеры
            UpdateCameraPosition(1.0f);
        }

        private void LateUpdate()
        {
            if (droneTransform == null || mainCamera == null)
                return;
                
            // Обновление позиции
            UpdateCameraPosition(Time.deltaTime);
        }
        
        private void UpdateCameraPosition(float deltaTime)
        {
            if (droneTransform == null) return;
            
            // Обновляем позицию цели камеры
            cameraTarget.position = Vector3.SmoothDamp(
                cameraTarget.position,
                droneTransform.position,
                ref velocity,
                0.2f
            );
            
            switch (currentMode)
            {
                case CameraMode.FirstPerson:
                    FirstPersonView();
                    break;
                case CameraMode.ThirdPerson:
                    ThirdPersonView(deltaTime);
                    break;
                case CameraMode.TopDown:
                    TopDownView(deltaTime);
                    break;
                case CameraMode.Orbit:
                    OrbitView(deltaTime);
                    break;
            }
        }
        
        private void FirstPersonView()
        {
            if (fpvCameraPosition != null)
            {
                // Используем позицию от первого лица напрямую
                mainCamera.transform.position = fpvCameraPosition.position;
                mainCamera.transform.rotation = fpvCameraPosition.rotation;
            }
            else
            {
                // Позиция непосредственно на дроне
                mainCamera.transform.position = droneTransform.position;
                mainCamera.transform.rotation = droneTransform.rotation;
            }
        }
        
        private void ThirdPersonView(float deltaTime)
        {
            // Позиция сзади и чуть выше дрона
            Vector3 offset = -droneTransform.forward * thirdPersonDistance + Vector3.up * thirdPersonHeight;
            Vector3 targetPosition = cameraTarget.position + offset;
            
            // Плавное движение камеры
            mainCamera.transform.position = Vector3.Lerp(
                mainCamera.transform.position,
                targetPosition,
                followSpeed * deltaTime
            );
            
            // Направление взгляда на дрон
            Vector3 dirToDrone = (droneTransform.position - mainCamera.transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(dirToDrone);
            
            // Применяем вращение
            mainCamera.transform.rotation = Quaternion.Slerp(
                mainCamera.transform.rotation,
                targetRotation,
                followSpeed * deltaTime
            );
        }
        
        private void TopDownView(float deltaTime)
        {
            // Позиция прямо над дроном
            Vector3 targetPosition = new Vector3(
                cameraTarget.position.x,
                cameraTarget.position.y + topDownHeight,
                cameraTarget.position.z
            );
            
            // Плавное движение
            mainCamera.transform.position = Vector3.Lerp(
                mainCamera.transform.position,
                targetPosition,
                followSpeed * deltaTime
            );
            
            // Смотрим прямо вниз
            Quaternion targetRotation = Quaternion.Euler(90f, 0f, 0f);
            mainCamera.transform.rotation = Quaternion.Slerp(
                mainCamera.transform.rotation,
                targetRotation,
                followSpeed * deltaTime
            );
        }
        
        private void OrbitView(float deltaTime)
        {
            // Орбитальное движение
            float angle = Time.time * 0.3f;
            Vector3 offset = new Vector3(
                Mathf.Sin(angle) * thirdPersonDistance,
                thirdPersonHeight,
                Mathf.Cos(angle) * thirdPersonDistance
            );
            
            // Позиция
            Vector3 targetPosition = cameraTarget.position + offset;
            mainCamera.transform.position = Vector3.Lerp(
                mainCamera.transform.position,
                targetPosition,
                followSpeed * deltaTime
            );
            
            // Смотрим на дрон
            mainCamera.transform.LookAt(cameraTarget.position);
        }

        public void SwitchCameraMode(CameraMode newMode)
        {
            currentMode = newMode;
            
            // Сразу обновляем позицию
            UpdateCameraPosition(1.0f);
            
            Debug.Log("Camera mode switched to: " + newMode);
        }
    }
}