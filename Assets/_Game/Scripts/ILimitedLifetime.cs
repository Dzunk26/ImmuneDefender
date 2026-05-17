using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILimitedLifetime {
    public int PhagocytosisCount { get; }
    public int PhagocytosisLimit { get; }
    public float LifeTimer { get; }
    public float LifeTimeLimit { get; }

    public void OnPhagocytosis();
    public void OnAlive();
}