using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

public class TestRunnerHotkey {
    [MenuItem("Tests/Run All Tests %#t")] // Ctrl/Cmd + Shift + T
    public static void RunAllTests()
    {
        var api = ScriptableObject.CreateInstance<TestRunnerApi>();
        var filter = new Filter { testMode = TestMode.EditMode }; // or TestMode.PlayMode
        api.Execute(new ExecutionSettings(filter));
        Debug.Log("Running All Tests...");
    }
}
