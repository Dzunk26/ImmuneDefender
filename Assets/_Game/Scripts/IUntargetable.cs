using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUntargetable {
    public event EventHandler OnBecameUntargetable; 
    public event EventHandler OnBecameTargetable; 
}