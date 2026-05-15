using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKillable {
    public event EventHandler OnDeath;
    public event EventHandler OnApoptosis;
    public void Die();
    public void Apoptosis();
}