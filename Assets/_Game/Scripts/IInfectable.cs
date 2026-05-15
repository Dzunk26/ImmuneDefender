using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInfectable {
    public bool IsInfected { get; }
    public event EventHandler OnInfected;
}