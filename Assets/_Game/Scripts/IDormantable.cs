using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDormantable : IUntargetable {
    public float DormancyTimerMax { get; }
    public float DormancyChance { get; }
    public bool IsDormant {  get; }
}
