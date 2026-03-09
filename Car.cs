using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;


// [System.Serializable]
// public class Engine
// {
//     public float idleRpm = 2400f;
//     public float maxRpm = 7000f;
//     public float[] gearRatios = { 3.50f, 2.80f, 2.30f, 1.90f, 1.60f, 1.30f, 1.00f, 0.85f };
//     public float finalDriveRatio = 4.0f;
//     private int _currentGear = 0;
//     public bool automaticTransmission = true;
//     private bool _switchingGears = false;
//     private float _gearChangeTime = 0.18f;
//     private float _rpm = 0f;
//
//     public void SetRpm(float averageWheelAngularVelocity)
//     {
//         float averageWheelRpm = (averageWheelAngularVelocity * 60f) / (2f * Mathf.PI);
//         float totalRatio = Math.Abs(gearRatios[_currentGear] * finalDriveRatio);
//         float transmissionRpm = averageWheelRpm * totalRatio;
//         float targetRpm = Mathf.Max(idleRpm, transmissionRpm);
//         this._rpm = Mathf.Clamp(targetRpm, idleRpm, maxRpm);
//     }
//
//     public float GetCurrentPower(MonoBehaviour context)
//     {
//         if (_switchingGears) return 0.3f;
//         return Mathf.Clamp01(_rpm / maxRpm);
//     }
//     public float AngularVelocityToRpm(float angularVelocity)
//     {
//         return angularVelocity * 60f / (2f * Mathf.PI);
//     }
//     public void UpGear(MonoBehaviour context)
//     {
//         if (_currentGear < gearRatios.Length - 1 && !_switchingGears)
//         {
//             _currentGear++;
//             _switchingGears = true;
//             context.StartCoroutine(ResetSwitchingGearsCoroutine());
//         }
//     }
//     public void DownGear(MonoBehaviour context)
//     {
//         if (_currentGear > 0 && !_switchingGears)
//         {
//             _currentGear--;
//             _switchingGears = true;
//             context.StartCoroutine(ResetSwitchingGearsCoroutine());
//         }
//     }
//     private System.Collections.IEnumerator ResetSwitchingGearsCoroutine()
//     {
//         yield return new WaitForSeconds(_gearChangeTime);
//         _switchingGears = false;
//     }
//     public int GetCurrentGear()
//     {
//         return _currentGear + 1;
//     }
//     public void CheckGearSwitching(MonoBehaviour context)
//     {
//         if (_switchingGears) return;
//         
//         // something to check transmission
//
//         if (_rpm > maxRpm * 0.95f && _currentGear < gearRatios.Length - 1)
//         {
//            UpGear(context); 
//         }
//         else if (_rpm < maxRpm * 0.6f && _currentGear > 0)
//         {
//             DownGear(context);
//         }
//     }
//     public float GetRpm()
//     {
//         return _rpm;
//     }
//     public bool IsSwitchingGears()
//     {
//         return _switchingGears;
//     }
// }
[Serializable]
public class WheelProperties
{
    public Vector3 localPosition;
    public float turnAngle = 0f;
    [HideInInspector] public float lastSuspensionLength = 0f;
    // [HideInInspector] public TrailRenderer skidTrail;
    public float mass = 1.5f; // mass of wheel in kg
    public float size = 0.5f; // size of wheel in m (radius)
    public float engineTorque = 40f; // torque of *wheel* in Nm from engine
    public float brakeStrength = 0.5f;
    public bool sliding = false;
    [HideInInspector] public Vector3 localSlipDirection;
    [HideInInspector] public Vector3 worldSlipDirection;
    [HideInInspector] public Vector3 suspensionForceDirection;
    [HideInInspector] public Vector3 wheelWorldPosition;
    [HideInInspector] public float wheelCircumference;
    [HideInInspector] public float torque = 0.0f;
    [HideInInspector] public GameObject wheelObject;
    [HideInInspector] public float hitPointForce;
    [HideInInspector] public Vector3 localVelocity;
    [HideInInspector] public float normalForce;
    [HideInInspector] public float angularVelocity; // rad/sec
    [HideInInspector] public float brake = 0;
    [HideInInspector] public float slipHistory = 0f;
    [HideInInspector] public float tcsReduction = 0f;
    [HideInInspector] public float slip;
    [HideInInspector] public Vector2 input = Vector2.zero;
}
public class Car : MonoBehaviour
{
    // public Engine e;
    public GameObject skidMarkPrefab;
    public float smoothTurn = 0.03f;
    // public float smoothTurn = GameManager.Instance.steeringSensitivity;
    float _coefStaticFriction = 1.95f;
    float _coefKineticFriction = 0.95f;
    public GameObject wheelPrefab;
    public WheelProperties[] wheels;
    public float wheelGripX = 14f;
    public float wheelGripZ = 14f;
    public float suspensionForce = 90f; // sprint constant
    public float dampAmount = 2.5f;
    public float suspensionForceClamp = 200f;
    private Vector2 _input = Vector2.zero; // hori=steer, vert = gas & brake
    public Rigidbody rb;
    public int braking = 0; // why is this?
    [HideInInspector] public bool forwards = true;
    [SerializeField] private float maxSpeed = 30f;
    public AnimationCurve torqueCurve;
    
    
    // Driving Assists
    public bool steeringAssist = true;
    [Range(0f, 1f)] public float steeringAssistStrength = 0.2f;
    public bool throttleAssist = true;
    public bool brakeAssist = true;
    [HideInInspector] public Vector2 userInput = Vector2.zero;
    public float downforce = 0.16f;
    [HideInInspector] public float isBraking = 0f;
    public Vector3 comOffest = new Vector3(0f, -0.2f, 0f);
    public float inertia = 1.2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (!rb) rb = gameObject.AddComponent<Rigidbody>();
        foreach (var wheel in wheels)
        {
            WheelProperties w = wheel;
            w.wheelObject = Instantiate(wheelPrefab, transform);
            w.wheelObject.transform.localPosition = w.localPosition;
            w.wheelObject.transform.eulerAngles = transform.eulerAngles;
            w.wheelObject.transform.localScale = 2f * new Vector3(wheel.size, wheel.size, wheel.size);
            w.wheelCircumference = 2f * Mathf.PI * wheel.size;
        }

        foreach (var w in wheels)
        {
            w.tcsReduction = 0f;
            w.slipHistory = 0f;
        }
        rb.centerOfMass += comOffest;
        rb.inertiaTensor *= inertia;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance)
        {
            smoothTurn = GameManager.Instance.steeringSensitivity;
        }

        _input = new Vector2(Mathf.Lerp(_input.x, Input.GetAxisRaw("Horizontal"), smoothTurn), Input.GetAxisRaw("Vertical"));
        braking = Input.GetKey(KeyCode.Space) ? 1 : 0;

        if (Input.GetKeyDown(KeyCode.R)) // resets the car if flipped
        {
            transform.rotation = Quaternion.identity;
            transform.position += Vector3.up * 2f;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        userInput.x = Mathf.Lerp(userInput.x, Input.GetAxisRaw("Horizontal") / (1 + rb.linearVelocity.magnitude / 28f), 0.2f);
        userInput.y = Mathf.Lerp(userInput.y, Input.GetAxisRaw("Vertical"), 0.2f);
        float forwardVelocity = Vector3.Dot(transform.forward, rb.linearVelocity);
        bool isBraking = Input.GetKey(KeyCode.S) && forwardVelocity > 0.5f;
    
        for (int i = 0; i < wheels.Length; i++)
        {
            var w = wheels[i];

            if (float.IsNaN(w.slip) || float.IsInfinity(w.slip))
                w.slip = 0;

            w.brake = (isBraking == true ? 1 : 0) * (1 - w.tcsReduction);

            float s = Mathf.Clamp01(w.slip);
            w.input.x = Mathf.Lerp(w.input.x, userInput.x, Time.deltaTime * 60f);
            if (s > 0.3f && s < 1.5f && steeringAssist)
                w.input.x = Mathf.Lerp(w.input.x, 0, s * Time.deltaTime * steeringAssistStrength);
            
            float finalThrottle = userInput.y * (1f - w.tcsReduction);
            if (float.IsNaN(finalThrottle) || float.IsInfinity(finalThrottle))
                finalThrottle = 0f;
            w.input.y = Mathf.Lerp(w.input.y, finalThrottle, 0.95f * Time.deltaTime * 60f);
            if (float.IsNaN(w.input.y) || float.IsInfinity(w.input.y))
                w.input.y = 0f;
        }
        
        Debug.Log($"SmoothTurn: {smoothTurn}");
    }

    void FixedUpdate()
    {
        rb.AddForce(-transform.up * rb.linearVelocity.magnitude * downforce);
        float averageWheelAngularVelocity = 0f;
        float currentSpeed = rb.linearVelocity.magnitude;
        float speedRatio = Mathf.Clamp01(currentSpeed / maxSpeed);
        float powerMultiplier = Mathf.Max(0f,torqueCurve.Evaluate(speedRatio));
        float finalPower = powerMultiplier;
        float forwardVelocity = Vector3.Dot(transform.forward, rb.linearVelocity);
        forwards = forwardVelocity > -0.5f;

        
        
        
        foreach (var w in wheels)
        {
            Transform wheelObj = w.wheelObject.transform; // what is "steered"
            Transform wheelVisual = wheelObj.GetChild(0); // what rotates

            wheelObj.localRotation = Quaternion.Lerp(
                wheelObj.localRotation,
                Quaternion.Euler(0, w.turnAngle * _input.x, 0),
                Time.fixedDeltaTime * 100f
            );

            if (w.input.y < 0)
            {
                finalPower *= 0.55f;
            }
            
            w.wheelWorldPosition = transform.TransformPoint(w.localPosition);
            Vector3 velocityAtWheel = rb.GetPointVelocity(w.wheelWorldPosition);
            w.localVelocity = wheelObj.InverseTransformDirection(velocityAtWheel);

            // w.torque = w.engineTorque * w.input.y * e.GetCurrentPower(this);
            w.torque = w.engineTorque * w.input.y * finalPower;
            float inertia = w.mass * w.size * w.size / 2f;
            float lateralFriction = -wheelGripX * w.localVelocity.x;
            float longitudinalFriction = -wheelGripZ * (w.localVelocity.z - w.angularVelocity);

            w.angularVelocity += (w.torque - longitudinalFriction * w.size) / inertia * Time.fixedDeltaTime;
            w.angularVelocity *= 1 - braking * w.brakeStrength * Time.fixedDeltaTime;

            Vector3 totalLocalForce = new Vector3(lateralFriction, 0f, longitudinalFriction) * w.normalForce * _coefStaticFriction * Time.fixedDeltaTime;
            float currentMaxFrictionForce = w.normalForce * _coefStaticFriction;
            
            w.sliding = totalLocalForce.magnitude > currentMaxFrictionForce;

            totalLocalForce = Vector3.ClampMagnitude(totalLocalForce, currentMaxFrictionForce);
            totalLocalForce *= w.sliding ? (_coefKineticFriction / _coefStaticFriction) : 1;
            
            Vector3 totalWorldForce = wheelObj.TransformDirection(totalLocalForce);
            w.worldSlipDirection = totalWorldForce;
            
            RaycastHit hit;
            if (Physics.Raycast(w.wheelWorldPosition, -transform.up, out hit, w.size * 2f))
            {
                float rayLen = w.size * 2f; // how much spring is compressed
                float compression = rayLen - hit.distance;
                float damping = (w.lastSuspensionLength - hit.distance) * dampAmount; // difference from last frame
                w.normalForce = (compression + damping) * suspensionForce;
                w.normalForce = Mathf.Clamp(w.normalForce, 0f, suspensionForceClamp);

                Vector3 springDir = hit.normal * w.normalForce;
                w.suspensionForceDirection = springDir;
                
                rb.AddForceAtPosition(springDir + totalWorldForce, hit.point);
                
                w.lastSuspensionLength = hit.distance;
                wheelObj.position = hit.point + transform.up * w.size;
            }
            else
            {
                wheelObj.position = w.wheelWorldPosition - transform.up * w.size;
            }

            averageWheelAngularVelocity += w.angularVelocity;
            
            wheelVisual.Rotate(Vector3.right, w.angularVelocity * Mathf.Rad2Deg * Time.fixedDeltaTime / w.size, Space.Self);
            
            // Debug.Log($"Torque: {w.torque} | Multiplier: {powerMultiplier} | Input: {w.input.y} | Speed Ratio: {speedRatio}");
            
            
        }
        averageWheelAngularVelocity /= wheels.Length;
        // Debug.Log(averageWheelAngularVelocity);
        
        // e.SetRpm(averageWheelAngularVelocity);
    }
}
