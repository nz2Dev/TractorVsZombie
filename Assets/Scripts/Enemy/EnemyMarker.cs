using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public static class EnemyState {

    private static int enemyCount = 0;

    public static int EnabledEnemies => enemyCount;

    internal static void OnEnemyEnabled() {
        enemyCount++;
    }

    internal static void OnEnemyDisabled() {
        Assert.IsTrue(enemyCount >= 1);
        enemyCount--;
    }
}

public class EnemyMarker : MonoBehaviour {

    private void OnEnable() {
        EnemyState.OnEnemyEnabled();
    }

    private void OnDisable() {
        EnemyState.OnEnemyDisabled();
    }
}
