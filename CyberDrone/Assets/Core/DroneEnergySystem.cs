using UnityEngine;
using UnityEngine.Events;

namespace DroneGame.Core
{
    /// <summary>
    /// Manages drone energy/battery system.
    /// Handles energy consumption, recharging, and related events.
    /// </summary>
    public class DroneEnergySystem : MonoBehaviour
    {
        [Header("Energy Properties")]
        [SerializeField] private float maxEnergy = 100f;
        [SerializeField] private float currentEnergy;
        [SerializeField] private float consumptionRate = 10f;
        [SerializeField] private float rechargeRate = 5f;
        [SerializeField] private float criticalEnergyThreshold = 20f;

        [Header("Recharge Settings")]
        [SerializeField] private bool autoRechargeWhenIdle = true;
        [SerializeField] private float idleThreshold = 0.1f;

        // Events
        public UnityEvent OnEnergyDepleted;
        public UnityEvent OnCriticalEnergy;
        public UnityEvent OnEnergyRecharged;

        // Properties
        public float CurrentEnergy => currentEnergy;
        public float MaxEnergy => maxEnergy;
        public float EnergyPercentage => (currentEnergy / maxEnergy) * 100f;
        public bool HasEnergy => currentEnergy > 0;
        public bool IsCritical => currentEnergy <= criticalEnergyThreshold;

        private DroneInputHandler inputHandler;
        private bool wasInCriticalState = false;

        private void Awake()
        {
            inputHandler = GetComponent<DroneInputHandler>();
            currentEnergy = maxEnergy;
        }

        private void Update()
        {
            // Handle critical energy state
            HandleCriticalState();

            // Auto recharge when idle if enabled
            if (autoRechargeWhenIdle && IsIdle())
            {
                RechargeEnergy(rechargeRate * Time.deltaTime);
            }
        }

        /// <summary>
        /// Consumes energy based on the specified amount
        /// </summary>
        public void ConsumeEnergy(float amount)
        {
            float consumption = amount * consumptionRate;
            currentEnergy = Mathf.Max(0, currentEnergy - consumption);

            // Trigger event if energy is depleted
            if (currentEnergy <= 0)
            {
                OnEnergyDepleted?.Invoke();
            }
        }

        /// <summary>
        /// Recharges energy by the specified amount
        /// </summary>
        public void RechargeEnergy(float amount)
        {
            bool wasNotFull = currentEnergy < maxEnergy;
            currentEnergy = Mathf.Min(maxEnergy, currentEnergy + amount);

            // Trigger event if energy was recharged to full
            if (wasNotFull && currentEnergy >= maxEnergy)
            {
                OnEnergyRecharged?.Invoke();
            }
        }

        /// <summary>
        /// Handles critical energy state and events
        /// </summary>
        private void HandleCriticalState()
        {
            if (IsCritical && !wasInCriticalState)
            {
                OnCriticalEnergy?.Invoke();
                wasInCriticalState = true;
            }
            else if (!IsCritical && wasInCriticalState)
            {
                wasInCriticalState = false;
            }
        }

        /// <summary>
        /// Checks if the drone is idle (not receiving significant input)
        /// </summary>
        private bool IsIdle()
        {
            if (inputHandler == null)
                return true;

            return Mathf.Abs(inputHandler.ThrustInput) < idleThreshold &&
                   Mathf.Abs(inputHandler.PitchInput) < idleThreshold &&
                   Mathf.Abs(inputHandler.YawInput) < idleThreshold &&
                   Mathf.Abs(inputHandler.RollInput) < idleThreshold;
        }
    }
}