using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInfectableImmuneCell : IKillable {
    public event EventHandler OnInfected;
    public bool IsInfected { get; }
    public void SetInfected(bool infected);
}