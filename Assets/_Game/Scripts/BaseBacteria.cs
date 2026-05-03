using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBacteria : MonoBehaviour, IDamageable {
    public event EventHandler OnDeath;

    public enum ActivityLevel { 
        Full, 
        Reduced, 
        Dormant 
    }

    [SerializeField] private float mapRadius = 500f; // bán kính map

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 0.5f;              // Tốc độ cơ bản

    [Header("Smooth Turning")]
    [SerializeField] private float turnSpeed = 0.2f;            // Tốc độ xoay hướng (càng nhỏ càng chậm/mượt, thử 1.5-4.0)
    [SerializeField] private float waypointDistance = 0.1f;  // khoảng cách coi là "đến nơi"
    [SerializeField] private float waypointRadius = 2f;      // bán kính random waypoint
    [SerializeField] private float waypointAngle = 120f;     // góc trước mặt để random
    [SerializeField] private float waypointTimerMax = 3f;    // thời gian tối đa trước khi random lại

    [SerializeField] private float aliveTimerMaxDefault = 20;
    [SerializeField] private float multiplicationTimerMaxDefault = 7f;

    [SerializeField] protected string poolTag;
    [SerializeField] protected int trophicLevel = 0; // thu bac trong chuoi thuc an

    [Header("Activity Level")]
    [SerializeField] private float fullActivityRange = 50f;
    [SerializeField] private float reducedActivityRange = 100f;
    [SerializeField] protected BacteriaSight bacteriaSight;
    [SerializeField] protected Collider bodyCollider;
    [SerializeField] protected GameObject bacteriaVisual;
    [SerializeField] private float updateActivityLevelTimerMax = 0.2f;


    private ActivityLevel currentActivityLevel = ActivityLevel.Full;
    private float updateActivityLevelTimer;
    private float aliveTimer;
    private float aliveTimerMax;
    private float multiplicationTimer;
    private float multiplicationTimerMax;
    
    private int hp = 1;

    private Vector3 currentWaypoint;
    private float waypointTimer;

    public virtual void OnInit() {
        aliveTimerMax = UnityEngine.Random.Range(aliveTimerMaxDefault - 1, aliveTimerMaxDefault + 1);
        multiplicationTimerMax = UnityEngine.Random.Range(multiplicationTimerMaxDefault - 1, multiplicationTimerMaxDefault + 1);

        aliveTimer = 0;
        multiplicationTimer = UnityEngine.Random.Range(0, multiplicationTimer * 0.5f);
    }

    public virtual void Damage(IAttackerStat attacker) {
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

    public virtual void Eat() { }

    public virtual void Eaten() { }

    private void Start() {
        currentWaypoint = GetRandomWaypoint();
        OnInit();
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
        HandleMovevement();
        HandleMultiplication(poolTag);
        HandleUpdateActivityLevel();
        SelfDestruct();
    }

    protected void HandleMovevement() {
        waypointTimer += Time.deltaTime;

        // Random waypoint mới nếu đến nơi hoặc hết thời gian
        bool arrivedAtWaypoint = Vector3.Distance(transform.position, currentWaypoint) < waypointDistance;
        bool waypointExpired = waypointTimer >= waypointTimerMax;

        if (arrivedAtWaypoint || waypointExpired) {
            currentWaypoint = GetRandomWaypoint();
            waypointTimer = 0f;
        }

        MoveTowardWaypoint();
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

    private void UpdateActivityLevel() {
        float distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        if (distance < fullActivityRange) {
            SetActivityLevel(ActivityLevel.Full);
        }
        else if (distance < reducedActivityRange) {
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

    private void MoveTowardWaypoint() {
        Vector3 direction = (currentWaypoint - transform.position).normalized;
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

    public int GetTrophicLevel() {
        return trophicLevel;
    }
}
