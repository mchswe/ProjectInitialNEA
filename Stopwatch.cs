using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Stopwatch : MonoBehaviour
{
    private float _currentTime;
    private bool _stopwatchActive = false;
    public TextMeshProUGUI currentTimeText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (_stopwatchActive == true)
        {
            _currentTime += Time.deltaTime;
        }
        TimeSpan time = TimeSpan.FromSeconds(_currentTime);
        currentTimeText.text = time.Minutes.ToString("0") + ":" + time.Seconds.ToString("00") + ":" + time.Milliseconds.ToString("000");
        ToggleStopwatch();
        ResetStopwatch();
    }

    public void StartStopwatch()
    {
        _stopwatchActive = true;
    }

    public void StopStopwatch()
    {
        _stopwatchActive = false;
    }

    void ToggleStopwatch()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (_stopwatchActive == false)
            {
                _stopwatchActive = true;
            }
            else if (_stopwatchActive == true)
            {
                _stopwatchActive = false;
            }
        }
    }

    void ResetStopwatch()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            _currentTime = 0;
            _stopwatchActive = false;
        }
    }
}
