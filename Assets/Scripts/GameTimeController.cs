using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameTimeController : MonoBehaviour
{
    public float dayCycleInMinutes = 30;
    public float timeMultiplier = 1;

    public const float SECOND = 1;
    public const float MINUTE = 60 * SECOND;
    public const float HOUR = 60 * MINUTE;
    public const float DAY = 24 * HOUR;
    public const float MONTH = 30 * DAY;
    public const float YEAR = 12 * MONTH;

    private const float DEGREES_PER_SECOND = 360 / DAY;

    private float _degreeRotation;

    private float _timeofDay;
    private int _secondsSinceStart;
    private int _lastSecondsSinceStart;
    private int _secondsElapsed;
    private int _years;
    private int _months;
    private int _days;
    private int _hours;
    private int _minutes;
    private int _seconds;

    private int _secondsInOneMinute;

    public Text gameTimeText; // UI element used to display the game clock

    public Slider energySlider; // UI slider element used to display player energy

    // Use this for initialization
    void Start()
    {
        energySlider = GameObject.Find("EnergySlider").GetComponent<Slider>();
        energySlider.value = 300;
        _timeofDay = 0; // start time of day at zero
        _secondsSinceStart = 0;
        _lastSecondsSinceStart = 0;
        _seconds = 0; // start seconds at zero
        _minutes = 0; // start minutes at zero
        _hours = 6; // start hour of day at 6am
        _days = 1; // start on day 1
        _months = 1; // start in month 1
        _years = 1; // start in year 1
        _degreeRotation = DEGREES_PER_SECOND * DAY / (dayCycleInMinutes * MINUTE);
        Time.timeScale = 1.0f;
        gameTimeText.text = ""; //Initialize game time text to a blank string.
        _secondsInOneMinute = 0;
    }

    // Update is called once per frame
    void Update()
    {
        _timeofDay += Time.deltaTime;
        _secondsSinceStart = (int)(Time.time * timeMultiplier);
        _secondsElapsed = _secondsSinceStart - _lastSecondsSinceStart;

        if (_secondsElapsed > 0) // Every time more than a second elapsed, add those elapsed seconds to our 'seconds' variable
        {
            _seconds += _secondsElapsed; // add any elapsed seconds to the minutes variable accelerated by a multiplier
            // Every time seconds exceeds 60, add 1 to minutes and take seconds back by 60
            if (_seconds >= 60)
            {
                if(_seconds >= 120)
                {
                    _minutes += 2;
                    energySlider.value -= 2; // decrease player energy by 2 points
                    _seconds -= 120;
                }
                else
                {
                    _minutes++;
                    energySlider.value -= 1; // decrease player energy by 1 point
                    _seconds -= 60;
                }
            }
            // Every time minutes hits 60, add 1 to hours and set minutes to 0
            if (_minutes >= 60)
            {
                _hours++;
                _minutes -= 60;
            }
            // Every time hours hits 24, add 1 to days and set hours back to 6am
            if (_hours == 24)
            {
                _days++;
                _hours = 6;
                energySlider.value = 100; // reset energy for a new day!
            }
            // Every time days hit 31, add 1 to months and set day to 1
            if (_days == 31)
            {
                _months++;
                _days = 1;
            }
            // Every time months hit 13, add 1 to years and set month to 1
            if (_months == 13)
            {
                _years++;
                _months = 1;
            }

            _lastSecondsSinceStart = _secondsSinceStart; // finally, store the current elapsed seconds for next pass.
            // Set a UI display string of DAY x HOUR::MIN::SECS
            // Only update the time at '10 minute' intervals, and decrease energy by 1 pt
            if (_minutes % 10 == 0)
            {
                gameTimeText.text = "DAY " + _days.ToString("D2") + " " + _hours.ToString("D2") + ":" + _minutes.ToString("D2");
            }
            
        }
              
    }

}