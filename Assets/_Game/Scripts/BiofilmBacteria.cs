using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiofilmBacteria : BaseBacteria, IBiofilmable
{
    public event EventHandler OnBreakShield;
    public event EventHandler OnRecoverShield;

    private bool isbreak = false;
    private float recoverShieldTimer;
    private float recoverShieldTimerMax = 4f;

    public override void OnUpdate() {
        if (isbreak) {
            recoverShieldTimer += Time.deltaTime;
            if (recoverShieldTimer >= recoverShieldTimerMax) {
                isbreak = false;
                recoverShieldTimer = 0f;

                OnRecoverShield?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public override void TakeDamage(int damage) {
        if (!isbreak) {
            BreakShield();
        }
        else {
            base.TakeDamage(damage);
        }
    }

    public void BreakShield() {
        isbreak = true;

        OnBreakShield?.Invoke(this, EventArgs.Empty);
    }
}
