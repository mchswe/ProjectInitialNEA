using System;
using NUnit.Framework.Internal.Commands;
using UnityEngine;
using Unity.Cinemachine;
public class CameraManager : MonoBehaviour
{
    // public Camera[] cameras;
    // Array which holds the different cameras used
    public CinemachineCamera[] cameras;
    private int _currentCameraIndex;

    // increments the currentCameraIndex to change the next camera in the array. Modulo allows the array to wrap around.
    public void CycleCamera()
    {
        _currentCameraIndex = (_currentCameraIndex + 1) % cameras.Length;
    }

    // This handles the priority to enable and disable the camera in use. The camera in use has its priority set to 999 which will make Cinemachine select it as the Live camera. Every other camera is set to priority of 0.
    public void UpdateCameraState()
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras == null)
            {
                break;
            }
            // if the current loop index matches the index of the current camera, the priority is changed to 999
            if (i == _currentCameraIndex)
            {
                cameras[i].Priority = 999;
            }
            // every other camera's priority is set to 0
            else
            {
                cameras[i].Priority = 0;
            }
        }

        // checks if the current camera contains the word "Orbit" in its name. if it contains "Orbit", the cursor is locked to the center of the screen and hidden. This is for the orbit camera only
        if (cameras[_currentCameraIndex].name.Contains("Orbit"))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        // if the camera is not the orbit camera (so the Chase, Bonnet camera), the cursor isn't locked and is visible 
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
    // if the player presses the "C" key, it will cycle to the next camera in the array (CycleCamera()) and change the priority of the cameras (UpdateCameraState())
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            CycleCamera();
            UpdateCameraState();
        }
        
        // Debug.Log(Input.GetKey(KeyCode.C));
    }
}

