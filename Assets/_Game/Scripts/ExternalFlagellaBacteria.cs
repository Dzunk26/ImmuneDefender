using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//vi khuan co the di chuyen nhanh de ne he mien dich tan cong
public class ExternalFlagellaBacteria : BaseBacteria, IDodgeable {
    private int dodgeChance = 100;
    private float slideSpeed = 10f;
    private float slideTimerMax = 1f;

    private bool isSliding = false;
    private float slideTimer;
    private Vector3 slideDirection;

    private void Update() {
        HanleMultiplication();

        if (isSliding) {
            HandleSlide();
        }
        else {
            HandleMovevement();
        }
    }

    public override void Damage(IAttackerStat attackerStat) {
        if (TryDodge(attackerStat.Accuracy)) {
            OnDodgeSuccess();
        }
        else {
            base.Damage(attackerStat);
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
        slideDirection = transform.forward;
        Debug.Log("Dodge");
    }

    private void HandleSlide() {
        slideTimer += Time.deltaTime;

        // Tự move theo slideDirection thay vì dùng velocity
        float speedThisFrame = Mathf.Lerp(slideSpeed, 0f, slideTimer / slideTimerMax); // giảm dần
        transform.position += slideDirection * speedThisFrame * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z); // lock Y

        if (slideTimer >= slideTimerMax) {
            isSliding = false;
        }
    }
}