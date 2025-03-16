using UnityEngine;

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
        // Визначаємо позицію камери, яка буде слідувати за дроном
        Vector3 targetPosition = droneTransform.position;

        // Зміщуємо камеру за допомогою фіксованого відстані і висоти
        Vector3 offset = -droneTransform.forward * thirdPersonDistance + Vector3.up * thirdPersonHeight;

        // Камера слідує за дроном з додаванням зміщення
        cameraTarget.position = Vector3.SmoothDamp(cameraTarget.position, targetPosition, ref velocity, 0.025f);

        ThirdPersonView(deltaTime, offset);
    }

    private void ThirdPersonView(float deltaTime, Vector3 offset)
    {
        // Камера повинна бути під певним кутом відносно дрону
        Vector3 targetPosition = cameraTarget.position + offset;

        // Плавне переміщення камери
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, followSpeed * deltaTime);

        // Камера завжди дивиться на дрон
        mainCamera.transform.LookAt(droneTransform.position);
    }
}