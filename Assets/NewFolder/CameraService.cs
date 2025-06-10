using System;
using System.Linq;

using Cinemachine;

using UnityEngine;
using UnityEngine.Assertions;

public class CameraService {

    private Transform followTransform;
    private CinemachineVirtualCamera topDownCamera;

    public CameraService(Camera unityCamera) {
        if (unityCamera.GetComponent<CinemachineBrain>() == null) {
            unityCamera.gameObject.AddComponent<CinemachineBrain>();
        }
    }

    public Vector3 CameraPosition => topDownCamera.transform.position;
    public Vector3 CameraForward => topDownCamera.transform.forward;

    public void InitTopDownFollowTarget(Vector3 initPosition, float distance) {
        Assert.IsNull(followTransform);
        var followGameObject = new GameObject("Camera Follow Target (New)");
        followTransform = followGameObject.transform;
        followTransform.position = initPosition;

        var topDownCameraPrefab = Resources.Load<CinemachineVirtualCamera>("Top Down Camera");
        topDownCamera = UnityEngine.Object.Instantiate(topDownCameraPrefab);
        topDownCamera.m_Follow = followTransform;
        topDownCamera.m_LookAt = followTransform;

        var framingTransposer = topDownCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        framingTransposer.m_CameraDistance = distance;

        CinemachineBrain.SoloCamera = topDownCamera;
    }

    public void UpdateTopDownFollowPosition(Vector3 vector3) {
        followTransform.position = vector3;
    }
}