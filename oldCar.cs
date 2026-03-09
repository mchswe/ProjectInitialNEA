// using System;
// using UnityEngine;
//
// [Serializable]
// public class WheelProperties
// {
//     public int wheelState = 1; // 1=steerable, 0 =free
//     [HideInInspector] public float biDirectional = 0;
//     public Vector3 localPosition;
//     public float turnAngle = 30f; // max steering angle for the wheel
//
//     [HideInInspector] public float lastSuspensionLength = 0.0f;
//     [HideInInspector] public Vector3 localSlipDirection;
//     [HideInInspector] public Vector3 worldSlipDirection;
//     [HideInInspector] public Vector3 suspensionForceDirection;
//     [HideInInspector] public Vector3 wheelWorldPosition;
//     [HideInInspector] public float wheelCircumference;
//     [HideInInspector] public float torque = 0.0f;
//     [HideInInspector] public Rigidbody parentRigidbody;
//     [HideInInspector] public GameObject wheelObject;
//     [HideInInspector] public float hitPointForce;
//     [HideInInspector] public Vector3 localVelocity;
// }
// public class Car : MonoBehaviour
// {
//     [Header("Wheel Setup")]
//     public GameObject wheelPrefab;
//     public WheelProperties[] wheels; // array of wheels
//     public float wheelSize = 0.53f; // wheel radius
//     public float maxTorque = 450f; // max engine torque
//     public float wheelGrip = 12f; // strength of resistance to sideways slip
//     public float maxGrip = 12f; // strength of resistance to sideways slip
//     public float frictionCoWheel = 0.022f; // rolling friction
//
//     [Header("Suspension")] 
//     public float suspensionForce = 90f; // spring constant
//     public float dampAmount = 2.5f; // damping constant
//     public float suspensionForceClamp = 200f; // cap on total suspension force
//
//     [Header("Car Mass")] 
//     public float massInKg = 500f;
//     // updated each frame
//     [HideInInspector] public Vector2 input = Vector2.zero; // hori=steering vert=throttle/brake
//     [HideInInspector] public bool forwards = false;
//     
//     private Rigidbody _rb;
//     
//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {
//         _rb=GetComponent<Rigidbody>(); // gets or creates a rigidbody
//         if (!_rb) _rb=gameObject.AddComponent<Rigidbody>();
//
//         _rb.inertiaTensor = 1.0f * _rb.inertiaTensor; // tweaks to inertia if desired, changes how much inertia the car has
//         // turns on interpolation at start and disables autoCoM
//         if (_rb != null)
//         {
//             _rb.interpolation = RigidbodyInterpolation.Interpolate;
//             _rb.automaticCenterOfMass = false;
//             _rb.centerOfMass = new Vector3(0, -0.15f, 0);
//         } 
//
//         if (wheels != null)
//         {
//             for (int i = 0; i < wheels.Length; i++)
//             {
//                 WheelProperties w = wheels[i];
//                 
//                 //Convert localPosition Consistently
//                 Vector3 parentRelativePosition = transform.InverseTransformPoint(transform.TransformPoint(w.localPosition));
//                 w.localPosition = parentRelativePosition;
//                 
//                 // Instantiate the visual wheel
//                 w.wheelObject = Instantiate(wheelPrefab, transform);
//                 w.wheelObject.transform.localPosition = w.localPosition;
//                 w.wheelObject.transform.localRotation = Quaternion.identity;
//                 w.wheelObject.transform.eulerAngles = transform.eulerAngles;
//                 w.wheelObject.transform.localScale = 2f * new Vector3(wheelSize, wheelSize, wheelSize);
//
//                 // May be in wrong place. Flips Wheels on Left Side of Car to not be inverted
//
//                 if (w.wheelObject.transform.position.z < 0)
//                 {
//                     Vector3 currentRot = transform.eulerAngles;
//                     transform.eulerAngles = new Vector3(currentRot.x, 180f,currentRot.z);
//                 }
//                 
//                 // Calculate wheel circumference for rotation logic
//                 w.wheelCircumference = 2f * Mathf.PI * wheelSize;
//
//                 w.parentRigidbody = _rb;
//             }
//         }
//     }
//
//     // Update is called once per frame
//     void Update()
//     {
//         input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
//     }
//
//     void FixedUpdate()
//     {
//         if (wheels == null || wheels.Length == 0) return;
//
//         foreach (var wheel in wheels)
//         {
//             if (!wheel.wheelObject) continue;
//             
//             Transform wheelObj = wheel.wheelObject.transform;
//             Transform wheelVisual = wheelObj.GetChild(0);
//             
//             // calc steer angle if wheelState == 1
//             if (wheel.wheelState == 1)
//             {
//                 float targetAngle = wheel.turnAngle * input.x; // left/right
//                 Quaternion targetRot = Quaternion.Euler(0, targetAngle, 0);
//                 //
//                 wheelObj.localRotation = Quaternion.Lerp(wheelObj.localRotation, targetRot, Time.fixedDeltaTime * 100f);
//             }
//             else if (wheel.wheelState == 0 && _rb.linearVelocity.magnitude > 0.04f)
//             {
//                 //free wheels, optionally align them in direction of motion
//                 RaycastHit tmpHit;
//                 if (Physics.Raycast(transform.TransformPoint(wheel.localPosition),
//                         -transform.up,
//                         out tmpHit,
//                         wheelSize * 2f))
//                 {
//                     Quaternion aim = Quaternion.LookRotation(_rb.GetPointVelocity(tmpHit.point), transform.up);
//                     wheelObj.rotation = Quaternion.Lerp(wheelObj.rotation, aim, Time.fixedDeltaTime * 100f);
//                 }
//             }
//             
//             // determine world position and velocity of this wheel at that point
//             wheel.wheelWorldPosition = transform.TransformPoint(wheel.localPosition);
//             Vector3 velocityAtWheel = _rb.GetPointVelocity(wheel.wheelWorldPosition);
//             
//             // so we do not have to manually rotate by turnAngle again
//             wheel.localVelocity = wheelObj.InverseTransformDirection(velocityAtWheel);
//             
//             // ENGINE
//             wheel.torque = Mathf.Clamp(input.y, -1f, 1f) * maxTorque / massInKg;
//             
//             // rolling friction
//             float rollingFrictionForce = -frictionCoWheel * wheel.localVelocity.z;
//             
//             // lateral friction, tries to cancel sideways slip
//             float lateralFriction = -wheelGrip * wheel.localVelocity.x;
//             lateralFriction = Mathf.Clamp(lateralFriction, -maxGrip, maxGrip);
//             
//             // engine force (force = torque / radius
//             float engineForce = wheel.torque / wheelSize;
//             
//             Vector3 totalLocalForce = new Vector3(
//                 lateralFriction,
//                 0f,
//                 rollingFrictionForce + engineForce
//                 );
//             
//             wheel.localSlipDirection = totalLocalForce;
//             
//             Vector3 totalWorldForce = wheelObj.TransformDirection(totalLocalForce);
//             wheel.worldSlipDirection = totalWorldForce;
//             
//             // checks if wheel is moving forward in its own local frame
//             // Forwards = (wheel.localVelocity.z > 0f);
//             
//             // SUSPENSION
//             RaycastHit hit;
//             if (Physics.Raycast(wheel.wheelWorldPosition, -transform.up, out hit, wheelSize * 2f))
//             {
//                 // how much spring is compressed
//                 float rayLen = wheelSize * 2f;
//                 float compression = rayLen - hit.distance;
//                 // damping is diff from last frame
//                 float damping = (wheel.lastSuspensionLength - hit.distance) * dampAmount;
//                 float springForce = (compression + damping) * suspensionForce;
//                 
//                 springForce = Mathf.Clamp(springForce, 0f, suspensionForceClamp);
//                 
//                 // direction is the surface normal
//                 Vector3 springDir = hit.normal * springForce;
//                 wheel.suspensionForceDirection = springDir;
//                 
//                 // apply total forces at contact
//                 _rb.AddForceAtPosition(springDir + totalWorldForce, hit.point);
//                 
//                 // move wheel visuals to the contact point + offset
//                 wheelObj.position = hit.point + transform.up * wheelSize;
//                 
//                 // store for damping next frame
//                 wheel.lastSuspensionLength = hit.distance;
//             }
//
//             else
//             {
//                 // if not hitting anything, just position the wheel under the local anchor
//                 wheelObj.position = wheel.wheelWorldPosition - transform.up * wheelSize;
//             }
//             
//             Vector3 forwardInWheelSpace = wheelObj.InverseTransformDirection(_rb.GetPointVelocity(wheel.wheelWorldPosition));
//
//             float wheelRotationSpeed = forwardInWheelSpace.z * 360f / wheel.wheelCircumference;
//
//             wheelVisual.Rotate(Vector3.right, wheelRotationSpeed * Time.fixedDeltaTime, Space.Self);
//         }
//     }
//     
//     
// // Not important but may be useful for debugging
// void OnDrawGizmos()
// {
//     if (wheels == null) return;
//
//     foreach (var wheel in wheels)
//     {
//         // marks wheel center
//         Gizmos.color = Color.green;
//         Gizmos.DrawSphere(wheel.wheelWorldPosition, 0.08f);
//         
//         // suspension force
//         if (wheel.suspensionForceDirection != Vector3.zero)
//         {
//             Gizmos.color = Color.blue;
//             Gizmos.DrawLine(
//                 wheel.wheelWorldPosition,
//                 wheel.wheelWorldPosition + wheel.suspensionForceDirection * 0.01f
//                 );
//         }
//         
//         // slip/friction force
//         if (wheel.wheelWorldPosition != Vector3.zero)
//         {
//             Gizmos.color = Color.red;
//             Gizmos.DrawLine(
//                 wheel.wheelWorldPosition,
//                 wheel.wheelWorldPosition + wheel.worldSlipDirection * 0.01f);
//         }
//     }
//     }
// }