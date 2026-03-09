using UnityEngine;

public class ChaseCam : MonoBehaviour
{
    public Transform targetAnchor;

    public Transform carTransform;

    void LateUpdate()
    {
        // basic logic for camera 
        Vector3 curAnchorPosition = carTransform.position;
        Vector3 desiredAnchor = targetAnchor.position;
        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredAnchor,0.1f);
        transform.position = smoothedPos;
        // transform.LookAt(carTransform.position, Vector3.up);
        
        // clamps (distance)
        float maxCamCarDist = 7.5f;
        float cameraCarDist = Vector3.Distance(transform.position, carTransform.position);
        if (cameraCarDist > maxCamCarDist)
        {
            Vector3 direction = (transform.position - carTransform.position).normalized;
            transform.position = carTransform.position + (direction * maxCamCarDist);
        }
        transform.LookAt(carTransform.position, Vector3.up);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {
//         
//     }
//
//     // Update is called once per frame
//     void Update()
//     {
//         
//     }
}
