using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AntibioticConfigSO : ScriptableObject {
    public float baseKillChance = 0.8f;
    public float resistanceStep = 0.2f;
    public float minKillChance = 0.1f;
}