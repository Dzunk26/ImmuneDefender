using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntibioticHandler : MonoBehaviour {
    public static AntibioticHandler Instance { get; private set; }

    [SerializeField] private AntibioticConfigSO antibioticConfig;

    private List<BaseBacteria> allActiveBacterias = new List<BaseBacteria>();
    private int useCount = 0;

    private void Awake() {
        Instance = this;
    }

    private float CalculateKillChance(float resistance) {
        float killChance = antibioticConfig.baseKillChance - resistance;
        return Mathf.Max(killChance, antibioticConfig.minKillChance);
    }

    private void HandleActiveAntibiotic() {
        allActiveBacterias = ObjectManager.Instance.GetAllActiveBacterias();
        
        foreach(BaseBacteria bacteria in allActiveBacterias) {
            bacteria.IncreaseResistance(antibioticConfig.resistanceStep);
            float killChance = CalculateKillChance(bacteria.ResistanceLevel);
            if (Random.value < killChance) {
                bacteria.Die();
            }

        }
    }

    public void UseAntibiotic() {
        HandleActiveAntibiotic();

        useCount++;
    }
}