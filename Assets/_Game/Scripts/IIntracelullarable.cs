using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IIntracelullarable : IUntargetable {
    public bool IsParasiting { get; }
}