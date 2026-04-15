using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//dai thuc bao
public class Macrophage : MonoBehaviour {
    [SerializeField] private MacrophageSight macrophageSight;
    [SerializeField] private Rigidbody rb;

    private float moveSpeed = 0.5f;

    private Transform currentTarget;

    private void Update() {
        currentTarget = FindTarget();
        HandleMovement();
    }

    private void HandleMovement() {
        Vector3 direction = new Vector3 (0, 0, 0);
        if (currentTarget != null) {
            direction = currentTarget.position - transform.position;
        }
        else {

        }
        rb.velocity = direction * moveSpeed;
    }

    private Transform FindTarget() {
        List<BaseBacteria> listBacteriaInRange = macrophageSight.GetListBacteriaInRange();

        Transform target = null;
        float minDistance = Mathf.Infinity;

        foreach (BaseBacteria bacteria in listBacteriaInRange) {
            Vector3 bacteriaPosion = bacteria.transform.position;

            if (Vector3.Distance(transform.position, bacteriaPosion) < minDistance) {
                minDistance = Vector3.Distance(transform.position, bacteriaPosion);
                target = bacteria.transform;
            }
        }

        return target;
    }


}