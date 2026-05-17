using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BacteriaSight : MonoBehaviour {
    public event EventHandler OnDangerDetected; //phat hien nguy hiem
    public event EventHandler OnPreyChange;

    [SerializeField] private BaseBacteria owner;

    private List<BaseBacteria> listPreyInRange = new List<BaseBacteria>(); //danh sach vi khuan co the an duoc trong tam nhin
    private List<BaseBacteria> listTargetablePreyInRange = new List<BaseBacteria>(); //danh sach vi khuan co the target va an duoc trong tam nhin
    private List<BaseBacteria> listHuntingBacteriaInRange = new List<BaseBacteria>(); //danh sach vi khuan o trang thai hunt trong tam nhin
    private List<Macrophage> listMacrophageInRange = new List<Macrophage>(); //danh sach dai thuc bao trong tam nhin
    private BaseBacteria cachedClosestPrey;

    private bool isInDanger = false;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.TryGetComponent(out BaseBacteria bacteria) && bacteria != owner) {
            if (CheckEdible(bacteria)) {
                if (bacteria is IUntargetable untargetable) {
                    untargetable.OnBecameUntargetable += Untargetable_OnBecameUntargetable;
                    untargetable.OnBecameTargetable += Untargetable_OnBecameTargetable;
                }
                if(bacteria is IHuntable huntable) {
                    huntable.OnStartHunting += Huntable_OnStartHunting;
                    huntable.OnStopHunting += Huntable_OnStopHunting;
                    
                    if (huntable.IsHunting) {
                        listHuntingBacteriaInRange.Add(bacteria);
                        UpdateDangerState();
                    }
                }
                listPreyInRange.Add(bacteria);
                listTargetablePreyInRange.Add(bacteria);
                bacteria.OnDeath += Bacteria_OnDeath;
                UpdatePreyState();
                   
            }
        }

        if (other.gameObject.TryGetComponent(out Macrophage macrophage)) {
            listMacrophageInRange.Add(macrophage);
            UpdateDangerState();
        }
    }

    private void Huntable_OnStartHunting(object sender, EventArgs e) {
        BaseBacteria bacteria = sender as BaseBacteria;

        if (!listHuntingBacteriaInRange.Contains(bacteria)) {
            listHuntingBacteriaInRange.Add(bacteria);
            UpdateDangerState();
        }
    }

    private void Huntable_OnStopHunting(object sender, EventArgs e) {
        BaseBacteria bacteria = sender as BaseBacteria;

        listHuntingBacteriaInRange.Remove(bacteria);
        UpdateDangerState();
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.TryGetComponent(out BaseBacteria bacteria)) {
            OnBacteriaInsightDisappear(bacteria);
        }

        if (other.gameObject.TryGetComponent(out Macrophage macrophage)) {
            listMacrophageInRange.Remove(macrophage);
            UpdateDangerState();
        }
    }

    private void Untargetable_OnBecameTargetable(object sender, EventArgs e) {
        BaseBacteria bacteria = sender as BaseBacteria;

        listTargetablePreyInRange.Add(bacteria);
    }

    private void Untargetable_OnBecameUntargetable(object sender, EventArgs e) {
        BaseBacteria bacteria = sender as BaseBacteria;

        listTargetablePreyInRange.Remove(bacteria);
    }

    private void OnBacteriaInsightDisappear(BaseBacteria bacteria) {
        bacteria.OnDeath -= Bacteria_OnDeath;

        if (bacteria is IUntargetable untargetable) {
            untargetable.OnBecameUntargetable -= Untargetable_OnBecameUntargetable;
            untargetable.OnBecameTargetable -= Untargetable_OnBecameTargetable;
        }

        if (bacteria is IHuntable huntable) {
            huntable.OnStartHunting -= Huntable_OnStartHunting;
            huntable.OnStopHunting -= Huntable_OnStopHunting;
        }

        listPreyInRange.Remove(bacteria);
        listTargetablePreyInRange.Remove(bacteria);
        listHuntingBacteriaInRange.Remove(bacteria);

        UpdatePreyState();
        UpdateDangerState();
    }

    private void Bacteria_OnDeath(object sender, EventArgs e) {
        BaseBacteria bacteria = sender as BaseBacteria;
        OnBacteriaInsightDisappear(bacteria);
    }

    private void UpdateDangerState() {
        bool danger = listMacrophageInRange.Count > 0 || listHuntingBacteriaInRange.Count > 0;
        
        if (!isInDanger && danger) {
            isInDanger = true;
            OnDangerDetected?.Invoke(this, EventArgs.Empty);
        }
        else if (!danger && isInDanger){
            isInDanger = false;
        }
    }

    private void UpdatePreyState() {
        cachedClosestPrey = GetClosestPrey(owner.transform.position);
        OnPreyChange?.Invoke(this, EventArgs.Empty);
    }

    private bool CheckEdible(BaseBacteria other) {
        return owner.GetSpeciesId() != other.GetSpeciesId();
    }

    public BaseBacteria GetClosestPrey(Vector3 fromPosition) {
        BaseBacteria closest = null;
        float minDistance = Mathf.Infinity;

        foreach (BaseBacteria bacteria in listPreyInRange) {
            float distance = (fromPosition - bacteria.transform.position).sqrMagnitude;

            if (distance < minDistance) {
                closest = bacteria;
                minDistance = distance;
            }
        }

        return closest;
    }

    public BaseBacteria GetClosestPrey() {
        return cachedClosestPrey;
    }
}
