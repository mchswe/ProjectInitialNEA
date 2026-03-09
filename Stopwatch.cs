using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Stopwatch : MonoBehaviour
{
    private float _currentTime; // stores the time elapsed in seconds
    private bool _stopwatchActive = false; // controls whether the stopwatch is counting or not
    public TextMeshProUGUI currentTimeText;

    // at the start of the game, the stopwatch is initialised to 0.
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentTime = 0;
    }

    // Update is called once per frame
    void Update()
    {    
        // if the boolean is  true, it begins to count time. deltaTime was used as it isn't affected by FPS
        if (_stopwatchActive == true)
        {
            _currentTime += Time.deltaTime;
        }

        // converts the time to a TimeSpan object
        TimeSpan time = TimeSpan.FromSeconds(_currentTime);
        // displays the stopwatch. seconds are padded with two leading zeros. milliseconds are padded with three leading zeros like that of a racing stopwatch in games
        currentTimeText.text = time.Minutes.ToString("0") + ":" + time.Seconds.ToString("00") + ":" + time.Milliseconds.ToString("000");
        // listens for keyboard inputs that control the stopwatch
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
            // if "I" is pressed and stopwatch isn't active, the stopwatch is activated
            if (_stopwatchActive == false)
            {
                _stopwatchActive = true;
            }

            // if "I" is pressed and stopwatch is already active, the stopwatch is activated. this pauses the stopwatch and doesn't reset it
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
            // if "O" is pressed, the stopwatch is reset back to 0 and disabled
            _currentTime = 0;
            _stopwatchActive = false;
        }
    }
}

