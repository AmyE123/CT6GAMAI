namespace CT6GAMAI
{
    using UnityEngine;
    using UnityEngine.UI;
    using static CT6GAMAI.Constants;

    public class UnitStatsManager : MonoBehaviour
    {        
        [SerializeField] private UnitManager _unitManager;

        [SerializeField] private int _healthPoints;
        [SerializeField] private Image _healthBarFill;

        private UnitData _unitBaseData;

        public int HealthPoints => _healthPoints;

        public void Start()
        {
            _unitBaseData = _unitManager.UnitData;
            _healthPoints = _unitBaseData.HealthPointsBaseValue;
        }

        public void Update() 
        {
            _healthBarFill.fillAmount = CalculateHealthPercentage(_healthPoints, _unitBaseData.HealthPointsBaseValue);
        }

        public int AdjustHealthPoints(int value)
        {
            // Add the value to current health points and clamp it within the valid range
            _healthPoints = Mathf.Clamp(_healthPoints + value, 0, _unitBaseData.HealthPointsBaseValue);
            CheckHealthState();

            return _healthPoints;
        }

        public UnitHealthState CheckHealthState()
        {
            if (_healthPoints <= 0)
            {
                Debug.Log("[BATTLE]: Unit death! HP at 0");
                return UnitHealthState.Dead;
            }
            else
            {
                return UnitHealthState.Alive;
            }
        }

        private float CalculateHealthPercentage(int currentHealth, int maxHealth)
        {
            return (float)currentHealth / maxHealth;
        }
    }
}