using UnityEngine;

namespace DroneGame.Core
{
    /// <summary>
    /// Controls the drone's camera system.
    /// Handles switching between different camera views and follow behavior.
    /// </summary>
    public class DroneCameraController : MonoBehaviour
    {
        [Header("Camera References")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private Transform droneTransform;

        [Header("Camera Settings")]
        [SerializeField] private float followSpeed = 5f;
        [SerializeField] private float rotationSpeed = 3f;
        [SerializeField] private float defaultDistance = 5f;
        [SerializeField] private float minDistance = 2f;
        [SerializeField] private float maxDistance = 10f;
        [SerializeField] private Vector3 offset = new Vector3(0, 2, -5);

        [Header("Camera Modes")]
        [SerializeField] private CameraMode currentMode = CameraMode.ThirdPerson;
        [SerializeField] private Transform fpvCameraPosition;

        private float currentDistance;

        // Camera mode enum
        public enum CameraMode
        {
            FirstPerson,
            ThirdPerson,
            TopDown,
            Orbit
        }

        private void Awake()
        {
            // Set up references if not assigned
            if (mainCamera == null)
                mainCamera = Camera.main;

            if (cameraTarget == null)
                cameraTarget = new GameObject("Camera Target").transform;

            if (droneTransform == null)
                droneTransform = transform;

            // Initialize
            currentDistance = defaultDistance;
        }

        private void LateUpdate()
        {
            // Update camera based on current mode
            switch (currentMode)
            {
                case CameraMode.FirstPerson:
                    UpdateFirstPersonCamera();
                    break;
                case CameraMode.ThirdPerson:
                    UpdateThirdPersonCamera();
                    break;
                case CameraMode.TopDown:
                    UpdateTopDownCamera();
                    break;
                case CameraMode.Orbit:
                    UpdateOrbitCamera();
                    break;
            }
        }

        private void UpdateFirstPersonCamera()
        {
            if (fpvCameraPosition != null)
            {
                // Match FPV camera position and rotation
                mainCamera.transform.position = fpvCameraPosition.position;
                mainCamera.transform.rotation = fpvCameraPosition.rotation;
            }
            else
            {
                // Fallback to drone's forward view
                mainCamera.transform.position = droneTransform.position + droneTransform.forward * 0.5f;
                mainCamera.transform.rotation = droneTransform.rotation;
            }
        }

        private void UpdateThirdPersonCamera()
        {
            // Update target position
            cameraTarget.position = Vector3.Lerp(cameraTarget.position, droneTransform.position, followSpeed * Time.deltaTime);
            
            // Calculate camera position with offset
            Vector3 targetPosition = cameraTarget.position + droneTransform.TransformDirection(offset);
            
            // Smoothly move to target position
            mainCamera.transform.position = Vector3.Lerp(
                mainCamera.transform.position, 
                targetPosition, 
                followSpeed * Time.deltaTime
            );
            
            // Look at drone
            Vector3 lookDirection = (droneTransform.position - mainCamera.transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            mainCamera.transform.rotation = Quaternion.Slerp(
                mainCamera.transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        private void UpdateTopDownCamera()
        {
            // Position above the drone
            Vector3 targetPosition = droneTransform.position + Vector3.up * currentDistance;
            mainCamera.transform.position = Vector3.Lerp(
                mainCamera.transform.position,
                targetPosition,
                followSpeed * Time.deltaTime
            );
            
            // Look down at the drone
            mainCamera.transform.rotation = Quaternion.Lerp(
                mainCamera.transform.rotation,
                Quaternion.Euler(90f, 0f, 0f),
                rotationSpeed * Time.deltaTime
            );
        }

        private void UpdateOrbitCamera()
        {
            // Update target position
            cameraTarget.position = Vector3.Lerp(cameraTarget.position, droneTransform.position, followSpeed * Time.deltaTime);
            
            // Orbit around the drone
            float angle = Time.time * 20f * Mathf.Deg2Rad;
            Vector3 targetPosition = cameraTarget.position + new Vector3(
                Mathf.Sin(angle) * currentDistance,
                offset.y,
                Mathf.Cos(angle) * currentDistance
            );
            
            // Move to position
            mainCamera.transform.position = Vector3.Lerp(
                mainCamera.transform.position,
                targetPosition,
                followSpeed * Time.deltaTime
            );
            
            // Look at drone
            mainCamera.transform.LookAt(cameraTarget);
        }

        /// <summary>
        /// Switch to a different camera mode
        /// </summary>
        public void SwitchCameraMode(CameraMode newMode)
        {
            currentMode = newMode;
        }

        /// <summary>
        /// Adjust camera distance
        /// </summary>
        public void AdjustCameraDistance(float zoomAmount)
        {
            currentDistance = Mathf.Clamp(currentDistance + zoomAmount, minDistance, maxDistance);
        }
    }
}