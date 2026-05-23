using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAntibioticResistable {
    public float ResistanceLevel { get; }
    void IncreaseResistance(float resistance);
}