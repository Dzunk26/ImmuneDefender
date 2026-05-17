using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//te bao limpho T
public class KillerT : MonoBehaviour {
    [SerializeField] private string poolTag;

    [Header("Movement")]
    [SerializeField] private KillerTSight killerTSight;
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float chaseSpeed = 1.2f;
    [SerializeField] private float turnSpeed = 3f;
    [SerializeField] private float attackDistance = 0.5f;

    [Header("Wander")]
    [SerializeField] private float waypointRadius = 2f;
    [SerializeField] private float waypointAngle = 120f;
    [SerializeField] private float waypointDistance = 0.1f;
    [SerializeField] private float waypointTimerMax = 3f;
    
    [SerializeField] private float updateTargetTimerMax = 1f;
    [SerializeField] private float aliveTimerMaxDefault = 18f;
    

    private enum State { Wander, Chase }
    private State state = State.Wander;

    private Macrophage currentTarget;
    private Vector3 currentWaypoint;
    private float waypointTimer;
    private float updateTargetTimer;
    private float aliveTimerMax;
    private float aliveTimer;

    private void OnEnable() {
        OnInit();
    }

    private void Start() {
        killerTSight.OnTargetChange += KillerTSight_OnTargetChange;
    }

    private void Update() {
        UpdateTarget();
        SelfDestruct();

        switch (state) {
            case State.Wander: 
                HandleWander(); 
                break;
            case State.Chase: 
                HandleChase(); 
                break;
        }
    }

    private void OnInit() {
        aliveTimerMax = UnityEngine.Random.Range(aliveTimerMaxDefault - 1, aliveTimerMaxDefault + 1);

        currentWaypoint = GetRandomWaypoint();
        currentTarget = null;
        updateTargetTimer = 0f;
        waypointTimer = 0f;
        aliveTimer = 0f;
    }

    private void UpdateTarget() {
        updateTargetTimer += Time.deltaTime;
        if (updateTargetTimer > updateTargetTimerMax) {
            updateTargetTimer = 0;
            RefreshTarget();
        }
    }

    private void KillerTSight_OnTargetChange(object sender, EventArgs e) {
        RefreshTarget();
    }

    private void RefreshTarget() {
        currentTarget = killerTSight.GetCachedClosestTarget();
        state = currentTarget != null ? State.Chase : State.Wander;
    }

    private void Die() {
        Destroy(gameObject);
        ObjectPooler.Instance.ReturnToPool(poolTag, gameObject);
    }

    private void SelfDestruct() {
        aliveTimer += Time.deltaTime;
        if (aliveTimer > aliveTimerMax) {
            Die();
        }
    }

    private void HandleWander() {
        waypointTimer += Time.deltaTime;
        bool arrived = (transform.position - currentWaypoint).sqrMagnitude < waypointDistance * waypointDistance;
        bool expired = waypointTimer >= waypointTimerMax;

        if (arrived || expired) {
            currentWaypoint = GetRandomWaypoint();
            waypointTimer = 0f;
        }

        MoveToward(currentWaypoint, moveSpeed);
    }

    private void HandleChase() {
        if (currentTarget == null) { state = State.Wander; return; }

        MoveToward(currentTarget.transform.position, chaseSpeed);

        float sqrDistance = (transform.position - currentTarget.transform.position).sqrMagnitude;
        if (sqrDistance < attackDistance * attackDistance) {
            Attack(currentTarget);
        }
    }

    private void Attack(Macrophage target) {
        target.Apoptosis();
    }

    private void MoveToward(Vector3 targetPosition, float speed) {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0f;

        if (direction != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        transform.position += transform.forward * speed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
    }

    private Vector3 GetRandomWaypoint() {
        float randomAngle = UnityEngine.Random.Range(-waypointAngle * 0.5f, waypointAngle * 0.5f);
        Quaternion rotation = Quaternion.Euler(0f, transform.eulerAngles.y + randomAngle, 0f);
        Vector3 randomDirection = rotation * Vector3.forward;
        float randomRadius = UnityEngine.Random.Range(waypointRadius * 0.5f, waypointRadius);
        return transform.position + randomDirection * randomRadius;
    }
}
