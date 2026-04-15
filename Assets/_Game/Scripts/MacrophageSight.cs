using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MacrophageSight : MonoBehaviour
{
    private List<BaseBacteria> listBacteriaInRange = new List<BaseBacteria>();

    private void OnTriggerEnter(Collider other) {
        BaseBacteria bacteria =  other.GetComponent<BaseBacteria>();
        if (bacteria != null) {
            listBacteriaInRange.Add(bacteria);
        }
    }

    private void OnTriggerExit(Collider other) {
        BaseBacteria bacteria = other.GetComponent<BaseBacteria>();
        if (listBacteriaInRange.Contains(bacteria)) {
            listBacteriaInRange.Remove(bacteria);
        }
    }

    public List<BaseBacteria> GetListBacteriaInRange() {
        return listBacteriaInRange;
    }

    public void RemoveDeadBacteria(BaseBacteria bacteria) {
        if (!listBacteriaInRange.Contains(bacteria)) {
            return;
        }
        
        listBacteriaInRange.Remove(bacteria);
    }
}
