using UnityEngine;
using Cinemachine;

public class ChangeVirtualCamera : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera[] vcams;
    private int nbOfVCam;
    private int currentCam;
    void Start()
    {
        if (vcams.Length == 0)
        {
            vcams = FindObjectsOfType<CinemachineVirtualCamera>();
            vcams[0].Priority = 10;
            nbOfVCam = vcams.Length;
            for (int i = 1; i < nbOfVCam; i++)
            {
                vcams[i].Priority = 0;
            }
        }

    }

    private void OnEnable()
    {
        ActionManager.onRotateLeft += TurnCameraLeft;
        ActionManager.onRotateRight += TurnCameraRight;
    }
    private void OnDisable()
    {
        ActionManager.onRotateLeft -= TurnCameraLeft;
        ActionManager.onRotateRight -= TurnCameraRight;
    }

    private void TurnCameraLeft()
    {
        vcams[currentCam].Priority = 0;
        currentCam = (currentCam - 1 + nbOfVCam) % nbOfVCam;
        vcams[currentCam].Priority = 10;
    }

    private void TurnCameraRight()
    {
        vcams[currentCam].Priority = 0;
        currentCam = (currentCam + 1) % nbOfVCam;
        vcams[currentCam].Priority = 10;
    }
}

