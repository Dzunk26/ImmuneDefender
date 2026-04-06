using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiofilmBacteriaVisual : MonoBehaviour {
    [SerializeField] private BiofilmBacteria biofilmBacteria;
    [SerializeField] private Outline outline;

    private void Start() {
        biofilmBacteria.OnBreakShield += BiofilmBacteria_OnBreakShield;
        biofilmBacteria.OnRecoverShield += BiofilmBacteria_OnRecoverShield;
        Show();
    }

    private void BiofilmBacteria_OnRecoverShield(object sender, System.EventArgs e) {
        Show();
    }

    private void BiofilmBacteria_OnBreakShield(object sender, System.EventArgs e) {
        Hide();
    }

    private void Show() {
        outline.OutlineMode = Outline.Mode.OutlineAll;
    }

    private void Hide() {
        outline.OutlineMode = Outline.Mode.OutlineHidden;
    }
}