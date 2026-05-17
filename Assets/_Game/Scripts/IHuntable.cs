using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHuntable {
    public event EventHandler OnStartHunting;
    public event EventHandler OnStopHunting;
    public bool IsHunting { get; }
}