using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

public class EnemyGenerator : MonoBehaviour {

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform enemyContainer;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform initialTarget;
    [SerializeField] private float spawnInterval = 0.5f;
    [SerializeField] private int enemiesPerSpawn = 10;
    [SerializeField] private int maxPoolSize = 30;
    [SerializeField] private int maxEnemies = 200;

    public int innactiveCount = 0;
    public int totalActive = 0;

    private IObjectPool<GameObject> _pool;
    private HashSet<GameObject> _active;

    private int ActiveCount => _active.Count;

    private void Awake() {
        _pool = new ObjectPool<GameObject>(OnCreateEnemy, OnTakeFromPool, OnReleaseToPool, OnDestroyEnemy, true, 50, maxPoolSize);
        _active = new HashSet<GameObject>(maxEnemies);
    }

    private void Update() {
        innactiveCount = _pool.CountInactive;
        totalActive = _active.Count;
    }

    private GameObject OnCreateEnemy() {
        var enemyObject = Instantiate(enemyPrefab, enemyContainer);
        var enemy = enemyObject.GetComponent<MeleZombieOperator>();
        enemy.OnDeath += () => _pool.Release(enemyObject);
        
        _active.Add(enemyObject);
        return enemyObject;
    }

    private void OnTakeFromPool(GameObject enemyObject) {
        enemyObject.SetActive(true);
        _active.Add(enemyObject);
        var health = enemyObject.GetComponent<Health>();
        health.Full();
        var behaviour = enemyObject.GetComponent<MeleZombieOperator>();
        behaviour.SetPathTarget(initialTarget);
        behaviour.Patrol();
    }

    private void OnReleaseToPool(GameObject enemyObject) {
        enemyObject.SetActive(false);
        _active.Remove(enemyObject);
    }

    private void OnDestroyEnemy(GameObject enemyObject) {
        Destroy(enemyObject);
        _active.Remove(enemyObject);
    }

    public void StartGeneration(int amount, float time) {
        StartCoroutine(GenerationRoutine(amount, time));
    }

    private IEnumerator GenerationRoutine(int amount, float time) {
        var generated = 0;
        var timeStarted = Time.time;
        var perFrameAllowed = 5;

        while(generated < amount) {
            var timePassed = Mathf.Min(Time.time - timeStarted, time);
            var amountProgress = (int) (timePassed / time * amount);    
            var needGenerateAmount = Mathf.Max(amountProgress - generated, 1);
            var thisFrameAmount = Mathf.Min(needGenerateAmount, perFrameAllowed);
            var canGeneratedAmount = Mathf.Max(maxEnemies - ActiveCount, 0);
            var generateAmount = Mathf.Min(thisFrameAmount, canGeneratedAmount);

            GenerateNext(generateAmount);             
            generated += generateAmount;

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void GenerateNext(int amount) {
        for (int i = 0; i < amount; i++) {
            var enemy = _pool.Get();
            enemy.transform.position = spawnPoint.position;
        }
    }

}
