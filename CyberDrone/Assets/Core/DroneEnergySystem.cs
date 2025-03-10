using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    public class DroneEnergySystem : MonoBehaviour
    {
        [Header("Energy Settings")]
        [SerializeField] private float maxEnergy = 100f;
        [SerializeField] private float currentEnergy;
        [SerializeField] private float consumptionRate = 5f;
        [SerializeField] private float rechargeRate = 2f;
        [SerializeField] private float criticalThreshold = 20f;
        
        // Свойства
        public float CurrentEnergy => currentEnergy;
        public float MaxEnergy => maxEnergy;
        public float EnergyPercentage => (currentEnergy / maxEnergy) * 100f;
        public bool HasEnergy => currentEnergy > 0;
        public bool IsCritical => currentEnergy < criticalThreshold;
        
        // События
        public UnityEvent OnEnergyDepleted;
        public UnityEvent OnCriticalEnergy;
        public UnityEvent OnEnergyRecharged;
        
        private bool wasInCriticalState;
        private bool wasDepleted;
        
        private void Awake()
        {
            // Начальная энергия
            currentEnergy = maxEnergy;
            wasInCriticalState = false;
            wasDepleted = false;
        }
        
        private void Update()
        {
            // Автоматическая подзарядка
            if (currentEnergy < maxEnergy)
            {
                currentEnergy = Mathf.Min(maxEnergy, currentEnergy + rechargeRate * Time.deltaTime);
            }
            
            // Проверка состояний
            CheckCriticalState();
            CheckDepletedState();
            CheckRechargedState();
        }
        
        public void ConsumeEnergy(float amount)
        {
            float consumption = amount * consumptionRate;
            currentEnergy = Mathf.Max(0, currentEnergy - consumption);
        }
        
        private void CheckCriticalState()
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
        
        private void CheckDepletedState()
        {
            if (!HasEnergy && !wasDepleted)
            {
                OnEnergyDepleted?.Invoke();
                wasDepleted = true;
            }
            else if (HasEnergy && wasDepleted)
            {
                wasDepleted = false;
            }
        }
        
        private void CheckRechargedState()
        {
            if (currentEnergy >= maxEnergy && (wasInCriticalState || wasDepleted))
            {
                OnEnergyRecharged?.Invoke();
            }
        }
    }
}