using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlayer : MonoBehaviour, IAttackerStat {
    private const string INTERACTABLE_LAYER = "Interactable";

    public int Damage => lazeDamage;
    public int Accuracy => accuracy;

    [SerializeField] private GameInput gameInput;

    private float moveSpeed = 7f;
    private int lazeDamage = 1;
    private int accuracy = 100;
    private LayerMask interactableLayer;


    private void Start() {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        interactableLayer = LayerMask.GetMask(INTERACTABLE_LAYER);
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, interactableLayer)) {
            if (hit.collider.TryGetComponent(out IDamageable damageable)) {
                damageable.Damage(this);
            }
        }
    }
}
