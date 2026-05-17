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

    protected override void HandleUpdate() {
        HandleMultiplication(poolTag);
        HandleUpdateActivityLevel();
        SelfDestruct();

        if (isSliding) {
            HandleSlide();
        }
        else {
            HandleMovevement();
        }
    }

    public override void TakeDamage(IAttackerStat attackerStat) {
        if (!isSliding && TryDodge(attackerStat.Accuracy)) {
            OnDodgeSuccess();
        }
        else {
            base.TakeDamage(attackerStat);
        }
    }

    public bool TryDodge(int attackerAccuracy) {
        if (bacteriaState == BacteriaState.Hunt) return false;

        float hitChance = (float)attackerAccuracy / (attackerAccuracy + dodgeChance); //calculate hitChance: hitChance is ability attack successfully
        hitChance = Mathf.Clamp01(hitChance); //limit hitChance in value range 0-1

        return UnityEngine.Random.value > hitChance;
    }

    public void OnDodgeSuccess() {
        isSliding = true;
        slideTimer = 0;
        slideDirection = transform.forward;
    }

    private void HandleSlide() {
        slideTimer += Time.deltaTime;

        float speedThisFrame = Mathf.Lerp(slideSpeed, 0f, slideTimer / slideTimerMax); 
        Vector3 movement = slideDirection * speedThisFrame * Time.deltaTime;

        if (Physics.Raycast(transform.position, slideDirection, out RaycastHit hit, movement.magnitude)) {
            if (hit.collider.TryGetComponent(out AttackArea attackArea)) {
                Debug.Log("enter attack area");
                Debug.Log(attackArea.CheckAttackState());
                if (attackArea.CheckAttackState()) {
                    attackArea.Phagocytosis(this);
                    isSliding = false;
                    return;
                }
            }
        }

        transform.position += slideDirection * speedThisFrame * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z); // lock Y

        if (slideTimer >= slideTimerMax) {
            isSliding = false;
        }
    }
}