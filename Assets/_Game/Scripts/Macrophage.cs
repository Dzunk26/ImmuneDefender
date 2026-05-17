using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//dai thuc bao
public class Macrophage : MonoBehaviour, IDamageable, IInfectable, IKillable, ILimitedLifetime, IAttackerStat, IBuffable {
    public event EventHandler OnDeath;
    public event EventHandler OnApoptosis;
    public event EventHandler OnInfected;

    [Header("Movement")]
    [SerializeField] private MacrophageSight macrophageSight;
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float chaseSpeed = 1f;
    [SerializeField] private float turnSpeed = 3f;

    [Header("Wander")]
    [SerializeField] private float waypointRadius = 2f;
    [SerializeField] private float waypointAngle = 120f;
    [SerializeField] private float waypointDistance = 0.1f;
    [SerializeField] private float waypointTimerMax = 3f;

    [SerializeField] private int damage = 3;
    [SerializeField] private int accuracy = 200;
    [SerializeField] private float attackCoolDown = 0.5f;
    [SerializeField] private float attackDistance = 2f;
    [SerializeField] private float aliveTimerMaxDefault = 1f;
    [SerializeField] private float updateTargetTimerMax = 1f;
    [SerializeField] private int phagocytosisLimitDefault = 8;
    [SerializeField] private string poolTag;

    private enum State { Wander, Chase }
    private State state = State.Wander;

    private BaseBacteria currentTarget;
    private Vector3 currentWaypoint;
    private float waypointTimer;
    private float updateTargetTimer;
    private float aliveTimerMax;
    private float attackTimer;
    private int phagocytosisLimit;

    public int PhagocytosisCount { get; private set; }
    public int PhagocytosisLimit => phagocytosisLimit;
    public float LifeTimer { get; private set; }
    public float LifeTimeLimit => aliveTimerMax;

    public bool IsInfected { get; private set; }
    public int Damage => damage;
    public int Accuracy => accuracy;

    private void OnEnable() {
        OnInit();
    }


    private void Start() {
        macrophageSight.OnBacteriaListChange += MacrophageSight_OnBacteriaListChange;

        currentWaypoint = GetRandomWaypoint();
    }

    private void Update() {
        UpdateTarget();
        OnAlive();

        switch (state) {
            case 
                State.Wander: HandleWander(); 
                break;
            case 
                State.Chase: HandleChase(); 
                break;
        }
    }

    private void OnDisable() {
        OnDespawn();
    }

    private void OnInit() {
        aliveTimerMax = UnityEngine.Random.Range(aliveTimerMaxDefault - 1, aliveTimerMaxDefault + 1);

        currentWaypoint = GetRandomWaypoint();
        currentTarget = null;
        updateTargetTimer = 0f;
        waypointTimer = 0f;
        LifeTimer = 0f;
        PhagocytosisCount = 0;
        phagocytosisLimit = phagocytosisLimitDefault;
        HelperTHandler.Instance.Register(this);
    }

    private void OnDespawn() {
        HelperTHandler.Instance.Unregister(this);
    }

    private void UpdateTarget() {
        updateTargetTimer += Time.deltaTime;
        if (updateTargetTimer > updateTargetTimerMax) {
            updateTargetTimer = 0;
            RefreshTarget();
        }
    }

    private void MacrophageSight_OnBacteriaListChange(object sender, System.EventArgs e) {
        RefreshTarget();
    }

    private void RefreshTarget() {
        currentTarget = macrophageSight.GetClosestBacteria(transform.position);
        state = currentTarget != null ? State.Chase : State.Wander;
    }

    private void HandleWander() {
        waypointTimer += Time.deltaTime;

        bool arrived = Vector3.Distance(transform.position, currentWaypoint) < waypointDistance;
        bool expired = waypointTimer >= waypointTimerMax;

        if (arrived || expired) {
            currentWaypoint = GetRandomWaypoint();
            waypointTimer = 0f;
        }

        MoveToward(currentWaypoint, moveSpeed);
    }

    private void HandleChase() {
        if (currentTarget == null) {
            state = State.Wander;
            return;
        }

        MoveToward(currentTarget.transform.position, chaseSpeed);
        HandleAttack(currentTarget);
    }

    private void HandleAttack(BaseBacteria target) {
        float sqrDistance = (transform.position - target.transform.position).sqrMagnitude;
        if (sqrDistance > attackDistance * attackDistance) return;
        if (attackTimer <= attackCoolDown) {
            attackTimer += Time.deltaTime;
            return;
        }

        attackTimer = 0f;
        OnPhagocytosis();
        target.TakeDamage(this);
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
        float halfAngle = waypointAngle * 0.5f;
        float randomAngle = UnityEngine.Random.Range(-halfAngle, halfAngle);

        Quaternion rotation = Quaternion.Euler(0f, randomAngle, 0f);
        Vector3 randomDirection = rotation * transform.forward;

        float randomRadius = UnityEngine.Random.Range(waypointRadius * 0.5f, waypointRadius);

        return transform.position + randomDirection * randomRadius;
    }

    public void TakeDamage(IAttackerStat attacker) {
        if (!IsInfected) return;

        Die();
    }

    public void Die() {
        OnDeath?.Invoke(this, EventArgs.Empty);
        Destroy(gameObject);
        //ObjectPooler.Instance.ReturnToPool(poolTag, this.gameObject);
    }

    public void Apoptosis() {
        OnApoptosis?.Invoke(this, EventArgs.Empty);
        Destroy(gameObject);
        //ObjectPooler.Instance.ReturnToPool(poolTag, this.gameObject);
    }

    public void SetInfected(bool infected) {
        if (IsInfected) return;
        IsInfected = infected;
        if (infected) OnInfected?.Invoke(this, EventArgs.Empty);
    }

    public void OnPhagocytosis() {
        PhagocytosisCount++;
        if (PhagocytosisCount >= phagocytosisLimit) {
            Die();
        }
    }

    public void OnAlive() {
        LifeTimer += Time.deltaTime;
        if (LifeTimer >= LifeTimeLimit) {
            Die();
        }
    }

    public void ApplyBuff(HelperTConfigSO config) {
        aliveTimerMax += config.lifeTimeBonus;
        phagocytosisLimit += config.phacytosisLimitBonus;
    }
}