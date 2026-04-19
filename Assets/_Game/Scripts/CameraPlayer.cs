using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlayer : MonoBehaviour, IAttackerStat {
    private const string BACTERIA_LAYER = "Bacteria";

    public int Damage => lazeDamage;
    public int Accuracy => accuracy;

    [SerializeField] private GameInput gameInput;

    private float moveSpeed = 7f;
    private int lazeDamage = 1;
    private int accuracy = 100;
    private LayerMask bacteriaLayer;


    private void Start() {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        bacteriaLayer = LayerMask.GetMask(BACTERIA_LAYER);
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e) {
        HandleInteract();
    }

    private void Update() {
        HandleMovement();
    }

    private void HandleMovement() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    private void HandleInteract() {
        Debug.Log("Interact");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, bacteriaLayer)) {
            Debug.Log("Start raycast");
            if (hit.collider.TryGetComponent(out BaseBacteria bacteria)) {
                Debug.Log("bacteria take damage");  
                bacteria.TakeDamage(this);
            }
        }
    }
}
