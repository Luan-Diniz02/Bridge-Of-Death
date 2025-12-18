using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera virtualCamera;
    [SerializeField] private CinemachineCamera deathCamera;
    [SerializeField] private Camera mainCamera;

    void Awake()
    {
        if(mainCamera == null) {
            mainCamera = Camera.main;
        }
    }
    
    /// <summary>
    /// Permite injetar referências de câmera externamente (usado pelo CameraManager)
    /// </summary>
    public void SetCameras(CinemachineCamera vcam, CinemachineCamera deathCam, Camera cam)
    {
        virtualCamera = vcam;
        deathCamera = deathCam;
        mainCamera = cam;
    }
    
    public void ActivateDeathCamera()
    {
        if (virtualCamera != null && deathCamera != null)
        {
            virtualCamera.Priority = 0;
            deathCamera.Priority = 1;
        }
    }

    public Vector3 GetCameraForwardDirection()
    {
        Vector3 cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();
        return cameraForward;
    }

    public Vector3 GetCameraRightDirection()
    {
        Vector3 cameraRight = mainCamera.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();
        return cameraRight;
    }

}
