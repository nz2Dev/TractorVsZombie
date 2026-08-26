
using Cinemachine;

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class LevelView {

    private readonly CameraManager cameraManager;
    public bool CutsceneFinished;

    public LevelView(CameraManager cameraManager) {
        this.cameraManager = cameraManager;
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
}