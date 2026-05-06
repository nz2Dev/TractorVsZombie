using System;
using System.Numerics;

using UnityEngine.Playables;

public class LevelView {

    public bool CutsceneFinished;

    public void ShowEnteringCutscene(PlayableDirector cutsceneDirector) {
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