using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    [SerializeField] private PrefabGenerator[] enemiesGenerators;
    [SerializeField] private Transform initialArrivalTarget;
    [SerializeField] private int enemiesLimit = 100;

    private void Awake() {
        foreach (var generator in enemiesGenerators) {
            generator.OnGenerate += OnNewEnemy;
        }
    }

    private IEnumerator Start() {
        while (true) {
            yield return new WaitForSeconds(1f);
            bool canGenerateMoreEnemy = EnemyState.EnabledEnemies < enemiesLimit;
            Debug.Log("Enabled enemies: " + EnemyState.EnabledEnemies + " can generate more?: " + canGenerateMoreEnemy);
            foreach (var generator in enemiesGenerators) {
                generator.enabled = canGenerateMoreEnemy;
            }
        }        
    }

    private void OnNewEnemy(GameObject enemyGO) {
        var enemy = enemyGO.GetComponent<MeleEnemyBehaviour>();
        enemy.SetPathTarget(initialArrivalTarget);
    }

}
