using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBacteria : MonoBehaviour, IDamageable, IAttackerStat {
    public event EventHandler OnDeath;

    public enum ActivityLevel { 
        Full, 
        Reduced, 
        Dormant 
    }

    public enum BacteriaState {
        Wander,
        Hunt
    }

    public int Damage => damage;
    public int Accuracy => accuracy;

    [SerializeField] private float mapRadius = 500f; // ban kinh map
    [SerializeField] private float eatDistance = 1f; 

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 0.5f;

    [Header("Smooth Turning")]
    [SerializeField] private float turnSpeed = 0.2f;
    [SerializeField] private float waypointDistance = 0.1f;
    [SerializeField] private float waypointRadius = 2f;
    [SerializeField] private float waypointAngle = 120f;
    [SerializeField] private float waypointTimerMax = 3f;

    [Header("Activity Level")]
    [SerializeField] private float fullActivityRange = 50f;
    [SerializeField] private float reducedActivityRange = 100f;
    [SerializeField] protected BacteriaSight bacteriaSight;
    [SerializeField] protected Collider bodyCollider;
    [SerializeField] protected GameObject bacteriaVisual;
    [SerializeField] private float updateActivityLevelTimerMax = 0.2f;

    [SerializeField] private float aliveTimerMaxDefault = 20;
    [SerializeField] private float multiplicationTimerMaxDefault = 7f;
    [SerializeField] private float hungerTimerMaxDefault = 15f;

    [SerializeField] protected string poolTag;
    [SerializeField] protected int speciesId = 0; // ma loai vi khuan


    private Transform cameraTranform;          
    protected BacteriaState bacteriaState;
    private ActivityLevel currentActivityLevel = ActivityLevel.Full;
    private float updateActivityLevelTimer;
    private float aliveTimer;
    private float aliveTimerMax;
    private float multiplicationTimer;
    private float multiplicationTimerMax;
    private float hungerTimer;
    private float hungerTimerMax;
    
    private int hp = 1;
    private int damage = 2;
    private int accuracy = 100;

    private Vector3 currentWaypoint;
    private float waypointTimer;

    private BaseBacteria currentPrey;

    private void OnEnable() {
        OnInit();
    }
    private void Start() {
        cameraTranform = Camera.main.transform;
        bacteriaSight.OnPreyChange += BacteriaSight_OnPreyChange;
        currentWaypoint = GetRandomWaypoint();
    }

    public virtual void OnInit() {
        bacteriaState = BacteriaState.Wander;

        aliveTimerMax = UnityEngine.Random.Range(aliveTimerMaxDefault - 1, aliveTimerMaxDefault + 1);
        multiplicationTimerMax = UnityEngine.Random.Range(multiplicationTimerMaxDefault - 1, multiplicationTimerMaxDefault + 1);
        hungerTimerMax = UnityEngine.Random.Range(hungerTimerMaxDefault - 1, hungerTimerMaxDefault + 1);

        aliveTimer = 0;
        multiplicationTimer = UnityEngine.Random.Range(0, multiplicationTimerMaxDefault * 0.3f);
        hungerTimer = UnityEngine.Random.Range(0, hungerTimerMaxDefault * 0.3f);
    }

    public virtual void TakeDamage(IAttackerStat attacker) {
        hp -= attacker.Damage;
        if (hp <= 0) {
            Die();
        }
    }

    public virtual bool IsHostile() => true; // danh dau vi khuan co hai hay khong

    public virtual void Die() {
        OnDeath?.Invoke(this, EventArgs.Empty);
        // thu vao pool
        ObjectPooler.Instance.ReturnToPool(poolTag, this.gameObject);
    }

    public virtual void Eat(BaseBacteria prey) {
        prey.TakeDamage(this);
        hungerTimer = 0f;
        EnterWanderState();
    }

    public virtual void Eaten() { }


    private void BacteriaSight_OnPreyChange(object sender, EventArgs e) {
        currentPrey = bacteriaSight.GetClosestPrey();
    }

    private void BacteriaSight_OnPreyDetected(object sender, EventArgs e) {
        TryEnterHuntState();
    }

    public void ManualUpdate() {
        switch (currentActivityLevel) {
            case ActivityLevel.Full:
                HandleUpdate();
                break;
            case ActivityLevel.Reduced:
                HandleMultiplication(poolTag);
                HandleUpdateActivityLevel();
                SelfDestruct();
                break;
            case ActivityLevel.Dormant:
                HandleUpdateActivityLevel();
                break; 
        }
    }

    protected virtual void HandleUpdate() {
        HandleState();
        HandleMultiplication(poolTag);
        HandleUpdateActivityLevel();
        SelfDestruct();
    }

    protected void HandleMovevement() {
        waypointTimer += Time.deltaTime;

        // Random waypoint mới nếu đến nơi hoặc hết thời gian
        bool arrivedAtWaypoint = (transform.position - currentWaypoint).sqrMagnitude < waypointDistance * waypointDistance;
        bool waypointExpired = waypointTimer >= waypointTimerMax;

        if (arrivedAtWaypoint || waypointExpired) {
            currentWaypoint = GetRandomWaypoint();
            waypointTimer = 0f;
        }

        MoveToTarget(currentWaypoint);
    }

    protected void HandleMultiplication(string poolTag) {
        multiplicationTimer += Time.deltaTime;

        if (multiplicationTimer > multiplicationTimerMax) {
            GameObject newBacteria = ObjectPooler.Instance.GetFromPool(poolTag);
            if (newBacteria == null) return;

            //float randomAngle = UnityEngine.Random.Range(120f, 240f);
            //float childAngle = transform.eulerAngles.y + randomAngle;

            newBacteria.transform.position = transform.position;
            newBacteria.transform.rotation = transform.rotation;
            
            if (newBacteria.TryGetComponent(out BaseBacteria bacteria)) {
                bacteria.SetWaypoint(GetRandomWaypoint(180f));
            }

            multiplicationTimer = 0f; // reset timer
        }
    }

    protected void SelfDestruct() {
        aliveTimer += Time.deltaTime;
        if (aliveTimer > aliveTimerMax) {
            Die();
        }
    }

    protected void HandleUpdateActivityLevel() {
        updateActivityLevelTimer += Time.deltaTime;
        if (updateActivityLevelTimer > updateActivityLevelTimerMax) {
            UpdateActivityLevel();
            updateActivityLevelTimer = 0f;
        }
    }

    private void HandleState() {
        switch (bacteriaState) {
            case BacteriaState.Wander:
                HandleWander();
                break;
            case BacteriaState.Hunt:
                HandleHunt();
                break;
        }
    }

    private void HandleWander() {
        HandleMovevement();

        hungerTimer += Time.deltaTime;
        TryEnterHuntState();
    }

    private void TryEnterHuntState() {
        if (hungerTimer > hungerTimerMax) {
            //currentPrey = bacteriaSight.GetClosestPrey(transform.position);
            if (currentPrey != null) {
                EnterHuntState();
            }
        }
    }

    private void EnterHuntState() {
        bacteriaState = BacteriaState.Hunt;
    }

    private void EnterWanderState() {
        bacteriaState = BacteriaState.Wander;
    }

    private void HandleHunt() {
        if (currentPrey == null) {
            EnterWanderState();
            return;
        }

        Vector3 preyPosition = currentPrey.transform.position;
        float sqrDistance = (transform.position - preyPosition).sqrMagnitude;
        MoveToTarget(preyPosition);

        // Đủ gần → ăn
        if (sqrDistance < eatDistance * eatDistance) {
            Eat(currentPrey);
        }
    }

    private void UpdateActivityLevel() {
        float sqrDistance = (transform.position - cameraTranform.position).sqrMagnitude;
        if (sqrDistance < fullActivityRange * fullActivityRange) {
            SetActivityLevel(ActivityLevel.Full);
        }
        else if (sqrDistance < reducedActivityRange * reducedActivityRange) {
            SetActivityLevel(ActivityLevel.Reduced);
        }
        else {
            SetActivityLevel(ActivityLevel.Dormant);
        }
    }

    private void SetActivityLevel(ActivityLevel activityLevel) {
        if (currentActivityLevel == activityLevel) return;

        currentActivityLevel = activityLevel;

        switch (activityLevel) {
            case ActivityLevel.Full:
                bacteriaVisual.SetActive(true);
                bodyCollider.enabled = true;
                bacteriaSight.gameObject.SetActive(true);
                break;
            case ActivityLevel.Reduced:
                bacteriaVisual.SetActive(false);
                bodyCollider.enabled = true;
                bacteriaSight.gameObject.SetActive(false);
                break;
            case ActivityLevel.Dormant:
                bacteriaVisual.SetActive(false);
                bodyCollider.enabled = false;
                bacteriaSight.gameObject.SetActive(false);
                break;

        }
    }

    private void MoveToTarget(Vector3 targetPosition) {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0f;

        // Xoay mượt về hướng waypoint
        if (direction != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        // Di chuyển thẳng về phía trước
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z); // lock Y
    }

    private Vector3 GetRandomWaypoint(float angleOffset = 0) {
        // Random góc trong phạm vi ±60 độ trước mặt (tổng 120 độ)
        float halfAngle = waypointAngle * 0.5f;
        float randomAngle = UnityEngine.Random.Range(-halfAngle, halfAngle) + angleOffset;

        // Xoay hướng hiện tại theo góc random
        Quaternion rotation = Quaternion.Euler(0f, transform.eulerAngles.y + randomAngle, 0f);
        Vector3 randomDirection = rotation * Vector3.forward;

        // Waypoint = vị trí hiện tại + hướng random * bán kính random
        float randomRadius = UnityEngine.Random.Range(waypointRadius * 0.5f, waypointRadius);
        Vector3 candidate = transform.position + randomDirection * randomRadius;

        // Nếu waypoint ra ngoài vùng → kéo về phía tâm map
        if (!IsInBounds(candidate)) {
            Vector3 toCenter = (Vector3.zero - transform.position).normalized;
            candidate = transform.position + toCenter * randomRadius;
        }

        return candidate;
    }

    private bool IsInBounds(Vector3 point) {
        return Vector2.Distance(new Vector2(point.x, point.z), Vector2.zero) < mapRadius;
    }

    public void SetWaypoint(Vector3 newWaypoint) {
        currentWaypoint = newWaypoint;
    }

    public int GetSpeciesId() {
        return speciesId;
    }
}
