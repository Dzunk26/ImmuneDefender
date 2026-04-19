using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MacrophageSight : MonoBehaviour
{
    private List<BaseBacteria> listBacteriaInRange = new List<BaseBacteria>();
    private List<BaseBacteria> listTargetableBacteriaInRange = new List<BaseBacteria>();

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent<BaseBacteria>(out BaseBacteria bacteria)) {
            bacteria.OnDeath += Bacteria_OnDeath;

            if (bacteria is IUntargetable untargetable) {
                untargetable.OnBecameUntargetable += Untargetable_OnBecameUntargetable;
                untargetable.OnBecameTargetable += Untargetable_OnBecameTargetable;
            }

            listBacteriaInRange.Add(bacteria);
            listTargetableBacteriaInRange.Add(bacteria);
        }
    }


    private void OnTriggerExit(Collider other) {
        if (other.TryGetComponent<BaseBacteria>(out BaseBacteria bacteria)) {
            OnBacteriaInsightDisappear(bacteria);
        }
    }

    private void Untargetable_OnBecameTargetable(object sender, System.EventArgs e) {
        BaseBacteria bacteria = sender as BaseBacteria;

        if (listBacteriaInRange.Contains(bacteria)) {
            listTargetableBacteriaInRange.Add(bacteria);
        }
    }

    private void Untargetable_OnBecameUntargetable(object sender, System.EventArgs e) {
        BaseBacteria bacteria = sender as BaseBacteria;

        listTargetableBacteriaInRange.Remove(bacteria);
    }

    private void Bacteria_OnDeath(object sender, System.EventArgs e) {
        BaseBacteria bacteria = sender as BaseBacteria;

        OnBacteriaInsightDisappear(bacteria);
    }

    private void OnBacteriaInsightDisappear(BaseBacteria bacteria) {
        bacteria.OnDeath -= Bacteria_OnDeath;

        if (bacteria is IUntargetable untargetable) {
            untargetable.OnBecameUntargetable -= Untargetable_OnBecameTargetable;
            untargetable.OnBecameTargetable -= Untargetable_OnBecameUntargetable;
        }

        listBacteriaInRange.Remove(bacteria);
        listTargetableBacteriaInRange.Remove(bacteria);
    }

    public BaseBacteria GetClosestBacteria(Vector3 fromPosition) {
        BaseBacteria closest = null;
        float minDistance = Mathf.Infinity;

        foreach (BaseBacteria bacteria in listTargetableBacteriaInRange) {
            float distance = Vector3.Distance(fromPosition, bacteria.transform.position);

            if (distance < minDistance) {
                closest = bacteria;
                minDistance = distance;
            }
        }

        return closest;
    }
}
