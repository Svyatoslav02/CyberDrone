using Core;
using UnityEngine;

public class Drone : MonoBehaviour
{
  [Header("Drone Settings")]
    [SerializeField] private float thrust = 25f;
    [SerializeField] private float pitchPower = 5f;
    [SerializeField] private float rollPower = 5f;
    [SerializeField] private float yawPower = 3f;
    [SerializeField] private float boostMultiplier = 2.0f;
    [SerializeField] private float hoverForce = 0.9f;
    
    [Header("Model")]
    [SerializeField] private Transform[] rotors;
    [SerializeField] private float rotorSpeed = 800f;
    [SerializeField] private Transform droneBody;

    [Header("Camera Settings")]
    [SerializeField] private Vector3 thirdPersonOffset = new Vector3(0, 2, -5);
    [SerializeField] private Vector3 firstPersonOffset = new Vector3(0, 0.5f, 1.5f); 
    [SerializeField] private float cameraSmoothing = 2f; 
    [SerializeField] private float camRotationSmoothing = 1f; 
    
    [Header("Energy System")]
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float energyConsumptionRate = 4f;
    [SerializeField] private float energyRechargeRate = 5f;
    [SerializeField] private float boostEnergyConsumption = 8f;
    [SerializeField] private float criticalEnergyLevel = 20f;
    
    // Components
    private Rigidbody rb;
    private Camera mainCamera;
    
    // Camera variables
    private Vector3 cameraVelocity = Vector3.zero;
    private Quaternion targetCamRotation = Quaternion.identity;
    private Transform cameraTarget;
    
    // Runtime values
    private float currentEnergy;
    private bool isBoostActive;
    private bool isCriticalEnergy;
    private float maxHeight = 0f;
    
    // Input values
    private float thrustInput;
    private float pitchInput;
    private float rollInput;
    private float yawInput;
    
    // Camera modes
    private enum CameraMode { FirstPerson, ThirdPerson, TopDown }
    private CameraMode currentCameraMode = CameraMode.ThirdPerson;
    private CameraMode previousCameraMode;
    
    private void Start()
    {
        // Get or add Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        // Настройка физики для лучшего полета
        rb.mass = 0.8f;
        rb.linearDamping = 0.2f;
        rb.angularDamping = 0.5f;
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        
        // Get main camera
        mainCamera = Camera.main;
        
        // Create camera target for smooth following
        if (cameraTarget == null)
        {
            GameObject targetObj = new GameObject("Drone Camera Target");
            cameraTarget = targetObj.transform;
            cameraTarget.position = transform.position;
            targetObj.hideFlags = HideFlags.HideInHierarchy;
        }
        
        // Initialize energy
        currentEnergy = maxEnergy;
        
        // Set drone body if not assigned
        if (droneBody == null)
            droneBody = transform;
        
        // Set initial position above ground
        transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
        
        // Initial camera setup
        previousCameraMode = currentCameraMode;
        UpdateCameraPosition();
        
        Debug.Log("Enhanced drone initialized! Use WASD to move, QE to rotate, Shift/Ctrl for up/down");
        Debug.Log("Press SPACE for boost, 1/3/5 to switch camera modes");
    }
    
    private void Update()
    {
        // Get input
        GetInput();
        
        // Handle boost
        isBoostActive = Input.GetKey(KeyCode.Space) && currentEnergy > 0;
        
        // Handle camera switching
        HandleCameraInput();
        
        // Update energy
        UpdateEnergy();
        
        // Rotate rotors
        RotateRotors();
        
        // Apply visual tilt to drone body for feedback
        ApplyVisualTilt();
        
        // Обновляем максимальную высоту для отслеживания
        if (transform.position.y > maxHeight)
            maxHeight = transform.position.y;
    }
    
    private void LateUpdate()
    {
        // Update camera in LateUpdate for smoother movement
        UpdateCameraPosition();
    }
    
    private void FixedUpdate()
    {
        // Apply physics forces (only if we have energy)
        if (currentEnergy > 0)
        {
            // Apply thrust with boost if active
            float actualThrust = thrust;
            if (isBoostActive)
                actualThrust *= boostMultiplier;
                
            // Thrust (up/down) - увеличенная сила для лучшего полета
            rb.AddForce(Vector3.up * thrustInput * actualThrust, ForceMode.Force);
            
            // Counter gravity when hovering to keep stable
            if (Mathf.Abs(thrustInput) < 0.1f)
            {
                rb.AddForce(Vector3.up * Physics.gravity.magnitude * hoverForce * rb.mass, ForceMode.Force);
            }
            
            // Rotation (pitch, roll, yaw)
            rb.AddRelativeTorque(
                pitchInput * pitchPower,
                yawInput * yawPower,
                -rollInput * rollPower,
                ForceMode.Force
            );
        }
        
        // Prevent going below ground level
        if (transform.position.y < 0.5f)
        {
            Vector3 pos = transform.position;
            pos.y = 0.5f;
            transform.position = pos;
            
            // Stop downward velocity
            if (rb.linearVelocity.y < 0)
            {
                Vector3 vel = rb.linearVelocity;
                vel.y = 0;
                rb.linearVelocity = vel;
            }
        }
        
        // Smoothly move camera target to follow drone
        if (cameraTarget != null)
        {
            cameraTarget.position = Vector3.SmoothDamp(
                cameraTarget.position, 
                transform.position, 
                ref cameraVelocity, 
                0.2f
            );
        }
    }
    
    private void GetInput()
    {
        thrustInput = 0;
        if (Input.GetKey(KeyCode.LeftShift)) thrustInput += 1f;
        if (Input.GetKey(KeyCode.LeftControl)) thrustInput -= 1f;
        
        pitchInput = 0;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) pitchInput += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) pitchInput -= 1f;
        
        rollInput = 0;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) rollInput += 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) rollInput -= 1f;
        
        yawInput = 0;
        if (Input.GetKey(KeyCode.E)) yawInput += 1f;
        if (Input.GetKey(KeyCode.Q)) yawInput -= 1f;
    }
    
    private void UpdateEnergy()
    {
        // Calculate energy consumption
        float consumption = 0f;
        
        // Basic consumption when moving
        if (thrustInput != 0 || pitchInput != 0 || rollInput != 0 || yawInput != 0)
            consumption += energyConsumptionRate * Time.deltaTime;
            
        // Extra consumption when boosting
        if (isBoostActive)
            consumption += boostEnergyConsumption * Time.deltaTime;
            
        // Apply consumption
        currentEnergy = Mathf.Max(0, currentEnergy - consumption);
        
        // Recharge when not moving much
        if (consumption < 0.1f)
            currentEnergy = Mathf.Min(maxEnergy, currentEnergy + energyRechargeRate * Time.deltaTime);
            
        // Check for critical energy
        isCriticalEnergy = currentEnergy < criticalEnergyLevel;
    }
    
    private void RotateRotors()
    {
        if (rotors == null || rotors.Length == 0)
            return;
            
        // Faster rotation when boosting
        float rotationModifier = isBoostActive ? 1.5f : 1f;
        
        // Slower rotation when low on energy
        if (currentEnergy <= 0)
            rotationModifier = 0.2f;
            
        foreach (Transform rotor in rotors)
        {
            if (rotor != null)
            {
                rotor.Rotate(Vector3.up, rotorSpeed * rotationModifier * Time.deltaTime);
            }
        }
    }
    
    private void ApplyVisualTilt()
    {
        if (droneBody == null) return;
        
        // Create visual tilt for feedback
        Quaternion targetRotation = Quaternion.Euler(
            pitchInput * 15f,
            0f,
            -rollInput * 15f
        );
        
        // Apply rotation to body
        droneBody.localRotation = Quaternion.Slerp(
            droneBody.localRotation,
            targetRotation,
            Time.deltaTime * 5f
        );
    }
    
    private void HandleCameraInput()
    {
        // Save previous mode
        previousCameraMode = currentCameraMode;
        
        // Change mode based on input
        if (Input.GetKeyDown(KeyCode.Alpha1))
            currentCameraMode = CameraMode.FirstPerson;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            currentCameraMode = CameraMode.ThirdPerson;
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            currentCameraMode = CameraMode.TopDown;
    }
    
    private void UpdateCameraPosition()
    {
        if (mainCamera == null || cameraTarget == null) return;
        
        // If camera mode changed, set target rotation
        if (previousCameraMode != currentCameraMode)
        {
            previousCameraMode = currentCameraMode;
            
            // Reset camera velocity when changing modes
            cameraVelocity = Vector3.zero;
        }
        
        Vector3 targetPosition = Vector3.zero;
        
        switch (currentCameraMode)
        {
            case CameraMode.ThirdPerson:
                // Third person view
                targetPosition = cameraTarget.position + transform.rotation * thirdPersonOffset;
                targetCamRotation = Quaternion.LookRotation(cameraTarget.position - targetPosition);
                break;
                
            case CameraMode.FirstPerson:
                // First person view
                targetPosition = transform.position + transform.rotation * firstPersonOffset;
                targetCamRotation = transform.rotation;
                break;
                
            case CameraMode.TopDown:
                // Top down view
                targetPosition = new Vector3(cameraTarget.position.x, cameraTarget.position.y + 10f, cameraTarget.position.z);
                targetCamRotation = Quaternion.Euler(90f, 0f, 0f);
                break;
        }
        
        // Smooth camera movement using SmoothDamp instead of Lerp for better damping
        mainCamera.transform.position = Vector3.SmoothDamp(
            mainCamera.transform.position,
            targetPosition,
            ref cameraVelocity,
            cameraSmoothing
        );
        
        // Smooth camera rotation
        mainCamera.transform.rotation = Quaternion.Slerp(
            mainCamera.transform.rotation,
            targetCamRotation,
            Time.deltaTime * (1.0f / camRotationSmoothing)
        );
    }
    
    private void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 16;
        
        // Energy display
        style.normal.textColor = isCriticalEnergy ? Color.red : Color.green;
        GUI.Label(new Rect(20, 20, 200, 30), $"Energy: {currentEnergy:F1}/{maxEnergy}", style);
        
        // Boost indicator
        if (isBoostActive)
        {
            style.normal.textColor = Color.yellow;
            GUI.Label(new Rect(20, 50, 200, 30), "BOOST ACTIVE", style);
        }
        
        // Flight data
        style.normal.textColor = Color.white;
        GUI.Label(new Rect(20, 80, 200, 30), $"Altitude: {transform.position.y:F1}m", style);
        GUI.Label(new Rect(20, 110, 200, 30), $"Max Altitude: {maxHeight:F1}m", style);
        GUI.Label(new Rect(20, 140, 200, 30), $"Speed: {rb.linearVelocity.magnitude:F1}m/s", style);
        GUI.Label(new Rect(20, 170, 200, 30), $"Vertical Speed: {rb.linearVelocity.y:F1}m/s", style);
        
        // Camera mode
        GUI.Label(new Rect(20, 200, 200, 30), $"Camera: {currentCameraMode}", style);
        
        // Controls
        style.fontSize = 14;
        style.normal.textColor = Color.yellow;
        GUI.Label(new Rect(20, Screen.height - 100, 400, 30), "WASD/Arrows = Move, Q/E = Rotate", style);
        GUI.Label(new Rect(20, Screen.height - 80, 400, 30), "Shift = Up, Ctrl = Down, SPACE = Boost", style);
        GUI.Label(new Rect(20, Screen.height - 60, 400, 30), "1 = First Person, 3 = Third Person, 5 = Top Down", style);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        // Handle collision effects
        if (collision.relativeVelocity.magnitude > 5f)
        {
            Debug.Log($"Drone crash! Velocity: {collision.relativeVelocity.magnitude:F1} m/s");
            
            // Reduce energy on crash
            currentEnergy = Mathf.Max(0, currentEnergy - collision.relativeVelocity.magnitude);
        }
    }
}