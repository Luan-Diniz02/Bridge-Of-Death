using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera virtualCamera;
    [SerializeField] private CinemachineCamera deathCamera;

    public void ActivateDeathCamera()
    {
        if (virtualCamera != null && deathCamera != null)
        {
            virtualCamera.Priority = 0;
            deathCamera.Priority = 1;
        }
    }

}
