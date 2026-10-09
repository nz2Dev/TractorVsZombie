
using System;

using Cinemachine;

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

public class LevelView {

    private readonly CameraManager cameraManager;
    private readonly UIDocument rootDocument;
    public bool CutsceneFinished;

    public LevelView(CameraManager cameraManager, UIDocument rootDocument) {
        this.cameraManager = cameraManager;
        this.rootDocument = rootDocument;
    }

    public void ShowEnteringCutscene(PlayableDirector cutsceneDirector, Vector3 startPosition) {
        cameraManager.BindToDirector(cutsceneDirector);
        cameraManager.UpdateTopDownFollowPosition(startPosition);
        cutsceneDirector.stopped += OnStopped;
        cutsceneDirector.Play();
    }

    public void ClearEvents() {
        CutsceneFinished = false;
    }

    private void OnStopped(PlayableDirector director) {
        CutsceneFinished = true;
    }

    internal void ShowPlayerUI() {
        var playerSection = rootDocument.rootVisualElement.Q("platformList");
        playerSection.style.display = DisplayStyle.Flex;
        var gameOverSection = rootDocument.rootVisualElement.Q("gameover-container");
        gameOverSection.style.display = DisplayStyle.None;
    }

    internal void ShowGameOverUI() {
        var playerSection = rootDocument.rootVisualElement.Q("platformList");
        playerSection.style.display = DisplayStyle.None;
        var gameOverSection = rootDocument.rootVisualElement.Q("gameover-container");
        gameOverSection.style.display = DisplayStyle.Flex;
    }

}