using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDodgeable {
    public bool TryDodge(int attackerAccuracy);

    public void OnDodgeSuccess();
}