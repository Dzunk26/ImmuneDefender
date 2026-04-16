using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BacteriaSight : MonoBehaviour {
    public event EventHandler OnDangerDetected; //phat hien nguy hiem
    public event EventHandler OnPreyDetected; //phat hien con moi

    [SerializeField] private BaseBacteria owner;

    private List<BaseBacteria> listEdibleBacteriaInRange = new List<BaseBacteria>(); //danh sach vi khuan co the an duoc trong tam nhin
    private List<BaseBacteria> listInedibleBacteriaInRange = new List<BaseBacteria>(); // danh sach vi khuan khong the an duoc trong tam nhin
    private List<Macrophage> listMacrophageInRange = new List<Macrophage>(); //danh sach dai thuc bao trong tam nhin

    private bool isInDanger = false;
    private bool hasPrey = false;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.TryGetComponent(out BaseBacteria bacteria) && bacteria != owner) {
            if (CheckEdible(bacteria)) {
                listEdibleBacteriaInRange.Add(bacteria);
                bacteria.OnDeath += Bacteria_OnDeath;
                UpdatePreyState();
            }
            else if (CheckInedible(bacteria)) {
                listInedibleBacteriaInRange.Add(bacteria);
                OnDangerDetected?.Invoke(this, EventArgs.Empty);
                bacteria.OnDeath += Bacteria_OnDeath;
                UpdateDangerState();
            }
        }

        if (other.gameObject.TryGetComponent(out Macrophage macrophage)) {
            listMacrophageInRange.Add(macrophage);
            UpdateDangerState();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.TryGetComponent(out BaseBacteria bacteria)) {
            bacteria.OnDeath -= Bacteria_OnDeath;

            listEdibleBacteriaInRange.Remove(bacteria);
            listInedibleBacteriaInRange.Remove(bacteria);
            UpdatePreyState();
            UpdateDangerState();
        }

        if (other.gameObject.TryGetComponent(out Macrophage macrophage)) {
            listMacrophageInRange.Remove(macrophage);
            UpdateDangerState();
        }
    }

    private void Bacteria_OnDeath(object sender, EventArgs e) {
        BaseBacteria bacteria = sender as BaseBacteria;
        bacteria.OnDeath -= Bacteria_OnDeath;

        listEdibleBacteriaInRange.Remove(bacteria);
        listInedibleBacteriaInRange.Remove(bacteria);

        UpdateDangerState();
        UpdatePreyState();
    }

    private void UpdateDangerState() {
        bool danger = listMacrophageInRange.Count > 0 || listInedibleBacteriaInRange.Count > 0;

        if (!isInDanger && danger) {
            isInDanger = true;
            OnDangerDetected?.Invoke(this, EventArgs.Empty);
        }
        else {
            isInDanger = false;
        }
    }

    private void UpdatePreyState() {
        bool prey = listEdibleBacteriaInRange.Count > 0; //check con moi trong tam nhin

        if (prey && !hasPrey) {
            hasPrey = true;
            OnPreyDetected?.Invoke(this, EventArgs.Empty);
        }
        else {
            hasPrey = false;
        }
    }

    private bool CheckEdible(BaseBacteria other) {
        return owner.GetTrophicLevel() > other.GetTrophicLevel();
    }

    private bool CheckInedible(BaseBacteria other) {
        return owner.GetTrophicLevel() < other.GetTrophicLevel();
    }

    public BaseBacteria GetClosestPrey(Vector3 fromPosition) {
        BaseBacteria closest = null;
        float minDistance = Mathf.Infinity;

        foreach (BaseBacteria bacteria in listEdibleBacteriaInRange) {
            float distance = Vector3.Distance(fromPosition, bacteria.transform.position);

            if (distance < minDistance) {
                closest = bacteria;
                minDistance = distance;
            }
        }

        return closest;
    }
}
