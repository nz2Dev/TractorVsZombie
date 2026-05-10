using System;

using Cinemachine;

using UnityEngine;
using UnityEngine.Playables;

// Is presentation layer subsystem that is the main authority over what camera rig is currently used
// and how camera rig behave, and what camera it operate
// It's isolaed from gameplay and its controllers via Engine/CameraProvider.cs
// ***
// The name: Camera, indicates that this relates to Presentation/Application, not to a gameplay feature
// I keep it in separate Folder, just to break folder coupling with GameBootstrapper
// **
// It doesn't fit inside Local/ "packages" semantic as it's not a library in its sense, it's tight to the application we develop
public class CameraManager : MonoBehaviour {

    private CinemachineSource cinemachineSource;
    private TopDownCameraRig topDownCameraRig;

    internal void SetCinemachineSource(CinemachineSource cinemachineSource) {
        this.cinemachineSource = cinemachineSource;
    }

    public Camera GetActiveCamera() {
        return cinemachineSource.cameraRef;
    }

    public void BindToDirector(PlayableDirector director) {
        foreach (var output in director.playableAsset.outputs) {
            if (output.sourceObject is CinemachineTrack) {
                director.SetGenericBinding(output.sourceObject, cinemachineSource.cinemachineBrain);
            }
        }
    }

    public void SetTopDownCameraRig(TopDownCameraRig topDownCameraRig) {
        this.topDownCameraRig = topDownCameraRig;
    }

    public void UpdateTopDownFollowPosition(Vector3 position) {
        topDownCameraRig.UpdateFollowPosition(position);
    }

}