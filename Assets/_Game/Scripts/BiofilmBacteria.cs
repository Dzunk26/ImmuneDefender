using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//vi khuan co the tao lop mang bao ve
public class BiofilmBacteria : BaseBacteria, IBiofilmable
{
    public event EventHandler OnBreakShield;
    public event EventHandler OnRecoverShield;

    private int shieldAmount = 1;
    private int curentShieldAmount = 1;
    private float recoverShieldTimer;
    private float recoverShieldTimerMax = 4f;
    private bool isBreak = false;

    protected override void HandleUpdate() {
        HandleRecoverShield();
        base.HandleUpdate();
    }

    private void HandleRecoverShield() {
        if (isBreak) {
            recoverShieldTimer += Time.deltaTime;
            if (recoverShieldTimer >= recoverShieldTimerMax) {
                isBreak = false;
                curentShieldAmount = shieldAmount;
                recoverShieldTimer = 0f;

                OnRecoverShield?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public override void Damage(IAttackerStat attackerStat) {
        int damage = attackerStat.Damage;

        if (curentShieldAmount > 0) {
            int damageToShield = Mathf.Min(curentShieldAmount, damage);
            curentShieldAmount -= damageToShield;
            damage -= damageToShield;

            if (curentShieldAmount == 0) {
                BreakShield();
            }
        }

        if (damage > 0) {
            base.Damage(attackerStat);
        }
    }

    public void BreakShield() {
        isBreak = true;
        OnBreakShield?.Invoke(this, EventArgs.Empty);
    }
}
