using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    // public TextMeshPro text
    public TextMeshProUGUI text;
    private Car _car;
    float[] _wheelSlip;
    void Start()
    {
        _car = GetComponent<Car>();
        _wheelSlip = new float[_car.wheels.Length];
        // set all to 0
        for (int i = 0; i < _wheelSlip.Length; i++)
        {
            _wheelSlip[i] = 0.0f;
        }
    }
    public void SetText(string newText)
    {
        if (text != null)
        {
            text.text = newText;
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI component is not assigned.");
        }
    }

    void Update()
    {
        String wheelStates = "";
        int at = 0;
        foreach (WheelProperties wheel in _car.wheels)
        {
            float slip = float.IsNaN(wheel.slip) ? 0f : wheel.slip;
            _wheelSlip[at] = slip;

            string slipText = _wheelSlip[at].ToString("F2");
            if (_wheelSlip[at] > 1f)
                slipText = "<color=blue>" + slipText + "</color>";
            else if (_wheelSlip[at] > 0.9f)
                slipText = "<color=red>" + slipText + "</color>";
            else if (_wheelSlip[at] > 0.7f)
                slipText = "<color=yellow>" + slipText + "</color>";
            else
                slipText = "<color=green>" + slipText + "</color>";

            wheelStates += slipText + " ";
            at++;
        }

        // float currentRpm = _car.e.GetRpm();
        // float maxRpm = _car.e.maxRpm;  // Assumes you have this accessible
        // string rpmText = _car.e.GetCurrentGear() + " " + currentRpm.ToString("F0");
        // if (_car.e.IsSwitchingGears())
        // {
        //     rpmText = "<color=blue>" + rpmText + "</color>";
        // }
        // else if (currentRpm > 0.8f * maxRpm)
        //     rpmText = "<color=red>" + rpmText + "</color>";
        // else if (currentRpm > 0.6f * maxRpm)
        //     rpmText = "<color=yellow>" + rpmText + "</color>";
        // else
        //     rpmText = "<color=green>" + rpmText + "</color>";
        //
        // rpmText += " " + _car.e.GetCurrentPower(this).ToString("F2");

        string tcsFactor = "TCS: ";
        for (int i = 0; i < _car.wheels.Length; i++)
        {
            tcsFactor += _car.wheels[i].tcsReduction.ToString("F2") + " ";
        }
        
        // car speeds
        float rawSpeed = _car.rb.linearVelocity.magnitude;
        float kmhSpeed = rawSpeed * 3.6f;
        
        if (GameManager.Instance.useImperial)
        {
            float mphSpeed = kmhSpeed * 0.62137f;
            text.text =
                (mphSpeed).ToString("F0") + " mph";
        }
        else
        {
            text.text =
                (kmhSpeed).ToString("F0") + " kph";
        }
    }
}