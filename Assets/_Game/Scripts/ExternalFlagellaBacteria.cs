using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//vi khuan co the di chuyen nhanh de ne he mien dich tan cong
public class ExternalFlagellaBacteria : BaseBacteria, IDodgeable {
    private int dodgeChance = 100;
    private float slideSpeed = 10f;
    private bool isSliding = false;
    private float slideTimer;
    private float slideTimerMax = 0.35f;

    private void Update() {
        HanleMultiplication();

        if (isSliding) {
            slideTimer += Time.deltaTime;
            if (slideTimer > slideTimerMax) {
                isSliding = false;
            }
        }
        else {
            HandleMovevement();
        }
    }

    public override void TakeDamage(IAttackerStat attackerStat) {
        if (TryDodge(attackerStat.Accuracy)) {
            OnDodgeSuccess();
        }
        else {
            base.TakeDamage(attackerStat);
        }
    }

    public bool TryDodge(int attackerAccuracy) {
        float hitChance = (float)attackerAccuracy / (attackerAccuracy + dodgeChance); //calculate hitChance: hitChance is ability attack successfully
        hitChance = Mathf.Clamp01(hitChance); //limit hitChance in value range 0-1

        return UnityEngine.Random.value > hitChance;
    }

    public void OnDodgeSuccess() {
        isSliding = true;
        slideTimer = 0;

        rb.velocity = transform.forward.normalized * slideSpeed;
        Debug.Log("Dodge");
    }
}