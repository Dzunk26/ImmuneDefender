using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class MapConfigSO : ScriptableObject {
    public string mapName;
    public List<WaveConfigSO> waveConfigs;
}