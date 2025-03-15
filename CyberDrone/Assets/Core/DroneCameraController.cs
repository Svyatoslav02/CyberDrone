using UnityEngine;

namespace Core
{
    public class DroneCameraController : MonoBehaviour
    {
        [Header("Camera References")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Transform droneTransform;

        [Header("Camera Settings")]
        private float followSpeed = 50f;
        private float thirdPersonDistance = 7;
        private float thirdPersonHeight = 2.5f;


        private Transform cameraTarget;
        private Vector3 velocity = Vector3.zero;

        private void Start()
        {
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

        private void FixedUpdate()
        {
            UpdateCameraPosition(Time.deltaTime);
        }

        private void UpdateCameraPosition(float deltaTime)
        {
            // Обновляем позицию цели камеры
            cameraTarget.position = Vector3.SmoothDamp(
                cameraTarget.position,
                droneTransform.position,
                ref velocity,
                0.025f
            );
            ThirdPersonView(deltaTime);
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

    }
}