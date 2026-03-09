using System;
using NUnit.Framework.Internal.Commands;
using UnityEngine;
using Unity.Cinemachine;
public class CameraManager : MonoBehaviour
{
    // public Camera[] cameras;
    public CinemachineCamera[] cameras;
    private int _currentCameraIndex;

    public void CycleCamera()
    {
        _currentCameraIndex = (_currentCameraIndex + 1) % cameras.Length;
    }
    public void UpdateCameraState()
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras == null)
            {
                break;
            }
            if (i == _currentCameraIndex)
            {
                cameras[i].Priority = 999;
            }
            else
            {
                cameras[i].Priority = 0;
            }
        }

        if (cameras[_currentCameraIndex].name.Contains("Orbit"))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    void Start()
    {
        UpdateCameraState();
        // ShowChaseCam();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            CycleCamera();
            UpdateCameraState();
        }
        
        Debug.Log(Input.GetKey(KeyCode.C));
    }
}
