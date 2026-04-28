using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class ObjectPooler : MonoBehaviour {

    [Serializable]
    public class Pool {
        public string tag;
        public GameObject prefab;
        public int size;
        public Transform parent;
    }

    public static ObjectPooler Instance { get; private set; }
    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        OnInitializePool();
    }


    private void OnInitializePool() {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool  in pools) {
            Queue<GameObject> objectsPool = new Queue<GameObject>();

            for(int i = 0; i < pool.size; i++) {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectsPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectsPool);
        }
    }

    public GameObject GetFromPool(string tag) {
        if (!poolDictionary.ContainsKey(tag)) {
            Debug.LogError("Pool doesn't contain tag " + tag);
            return null;
        }

        GameObject obj = poolDictionary[tag].Dequeue();
        obj.SetActive(true);
        
        return obj;
    }

    public void ReturnToPool(string tag, GameObject obj) {
        if (!poolDictionary.ContainsKey(tag)) {
            Debug.LogError("Pool doesn't contain tag " + tag);
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        poolDictionary[tag].Enqueue(obj);
    }
}