using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    [SerializeField] private EnemyGenerator[] enemiesGenerators;
    [SerializeField] private Transform initialArrivalTarget;
    [SerializeField] private int enemiesLimit = 100;

    private void Start() {
        foreach (var generator in enemiesGenerators) {
            generator.StartGeneration(400, 30f);
        }
    }
}
