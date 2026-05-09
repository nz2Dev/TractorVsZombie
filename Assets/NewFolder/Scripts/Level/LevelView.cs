using Cinemachine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class LevelView {

    public bool CutsceneFinished;

    public void ShowEnteringCutscene(PlayableDirector cutsceneDirector, CinemachineBrain brain) {
        foreach (var output in cutsceneDirector.playableAsset.outputs) {
            if (output.sourceObject is CinemachineTrack) {
                cutsceneDirector.SetGenericBinding(output.sourceObject, brain);
            }
        }
        
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