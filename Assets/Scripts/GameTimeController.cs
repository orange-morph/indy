using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;

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

    //private float _degreeRotation;

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

    private bool _sunRising; // Use this to control sunrise ambient light effect at start of day
    private bool _sunSetting; // Use this to control sunset ambient light effect at end of day
    private bool _dusk; // Use this to control dusk ambient light effect at end of day

    private int _secondsInOneMinute;

    [Header("Game Time Display Elements")]
    public Text gameTimeText; // UI element used to display the game clock - set in UI
    public Text infoText; // UI element used to display the game clock - set in UI

    [Header("Player Health and Energy Sliders")]
    public MyPlayer player; // Assign in UI, will be needed to get / set health and energy
    public Slider healthSlider; // UI slider element used to display player health
    public Slider energySlider; // UI slider element used to display player energy
    
    // Variables for controlling sunrise / sunset / dusk ambient light effects.
    [Header("Sunrise Transition Controls")]
    public float sunriseDuration = 30; // Duration time of sunrise in seconds.
    public float sunriseSmoothness = 0.02f; // This will determine the smoothness of the lerp. Smaller values are smoother. Really it's the time between updates.
    public Color sunriseStartColor = Color.blue;
    public Color sunriseTransitionColor1 = Color.yellow;
    public Color sunriseTransitionColor2 = Color.yellow;
    public Color sunriseEndColor = Color.white;
    Color sunriseCurrentColor = Color.white; // This is the state of the color in the current interpolation.

    [Header("Sunset Transition Controls")]
    public float sunsetDuration = 30; // Duration time of sunset in seconds.
    public float sunsetSmoothness = 0.02f; // This will determine the smoothness of the lerp. Smaller values are smoother. Really it's the time between updates.
    public Color sunsetStartColor = Color.white;
    public Color sunsetTransitionColor = Color.yellow;
    public Color sunsetEndColor = Color.yellow;
    Color sunsetCurrentColor = Color.white; // This is the state of the color in the current interpolation.

    public float duskDuration = 30; // Duration time of dusk in seconds.
    public float duskSmoothness = 0.02f; // This will determine the smoothness of the lerp. Smaller values are smoother. Really it's the time between updates.
    public Color duskStartColor = Color.yellow;
    public Color duskTransitionColor = Color.blue;
    public Color duskEndColor = Color.black;
    Color duskCurrentColor = Color.white; // This is the state of the color in the current interpolation.

    [Header("Pausable UI Windows")]
    public UnityUIQuestLogWindow questLogWindow; // Assign in inspector
    public GameObject startScreen; // Assign in inspector

    // Also need: various Inventory UI windows (inventory, character)

    // Use this for initialization
    void Start()
    {
        healthSlider = GameObject.Find("HealthSlider").GetComponent<Slider>();
        energySlider = GameObject.Find("EnergySlider").GetComponent<Slider>();
        //healthSlider.value = player.health;
        //energySlider.value = player.energy;
        _timeofDay = 0; // start time of day at zero
        _secondsSinceStart = 0;
        _lastSecondsSinceStart = 0;
        _seconds = 0; // start seconds at zero
        _minutes = 0; // start minutes at zero
        _hours = 6; // start hour of day at 6am
        _days = 1; // start on day 1
        _months = 1; // start in month 1
        _years = 1; // start in year 1
        //_degreeRotation = DEGREES_PER_SECOND * DAY / (dayCycleInMinutes * MINUTE);
        gameTimeText.text = ""; //Initialize game time text to a blank string.
        infoText.text = "";
        _secondsInOneMinute = 0;
        _sunRising = false;
        _sunSetting = false;
        _dusk = false;
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
                if (_seconds >= 120)
                {
                    _minutes += 2;
                    _seconds -= 120;
                }
                else
                {
                    _minutes++;
                    _seconds -= 60;
                }
                if (_minutes % 10 == 0)
                {
                    energySlider.value -= 1; // decrease player energy by 1 point
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
                energySlider.value = 300; // reset energy for a new day!
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
            // Only update the time at '10 minute' intervals
            if (_minutes % 10 == 0)
            {
                gameTimeText.text = "DAY " + _days.ToString("D2") + " " + _hours.ToString("D2") + ":" + _minutes.ToString("D2");
            }    
        }

        ambientLighting(); // handle sunrise, sunset, and dusk light transitions.
        
    }

    // check for and execute effects needed for sunrise, sunset and dusk ambient light transitions.
    public void ambientLighting()
    {
        if (_hours >= 6 && _hours <= 9)
        {
            if (_sunRising == false)
            {
                _sunRising = true;
                StartCoroutine("RunSunrise");   
            }
            _sunSetting = false;
            _dusk = false;
        }
        else
        {
            if (_hours >= 19 && _hours <= 24)
            {
                if (_sunSetting == false)
                {
                    _sunSetting = true;
                    StartCoroutine("RunSunset");
                }
                _sunRising = false;
                _dusk = false;
            }
            else
            {
                //if (_hours >= 21 && _hours <= 24)
                //{
                //    if (_dusk == false)
                //    {
                //        _dusk = true;
                //        StartCoroutine("RunDusk");
                //    }
                //    _sunSetting = false;
                //    _sunRising = false;
                //}
                //else
                //{
                    _sunRising = false;
                    _sunSetting = false;
                    _dusk = false;
                    infoText.text = "";
                //}
            }
        }
    }

    IEnumerator RunSunrise()
    {
        float progress = 0; //This float will serve as the 3rd parameter of the lerp function.
        float increment = sunriseSmoothness / sunriseDuration; // The amount of change to apply.
        while (progress < 1)
        {
            sunriseCurrentColor = Color.Lerp(sunriseStartColor, sunriseTransitionColor1, progress);
            RenderSettings.ambientLight = sunriseCurrentColor;
            progress += increment;
            infoText.text = "SUN RISING 1: " + progress.ToString();
            yield return new WaitForSeconds(sunriseSmoothness);
        }
        progress = 0;
        while (progress < 1)
        {
            sunriseCurrentColor = Color.Lerp(sunriseTransitionColor1, sunriseTransitionColor2, progress);
            RenderSettings.ambientLight = sunriseCurrentColor;
            progress += increment;
            infoText.text = "SUN RISING 2: " + progress.ToString();
            yield return new WaitForSeconds(sunriseSmoothness);
        }
        progress = 0;
        while (progress < 1)
        {
            sunriseCurrentColor = Color.Lerp(sunriseTransitionColor2, sunriseEndColor, progress);
            RenderSettings.ambientLight = sunriseCurrentColor;
            progress += increment;
            infoText.text = "SUN RISING 3: " + progress.ToString();
            yield return new WaitForSeconds(sunriseSmoothness);
        }
        infoText.text = "";
        yield return true;
    }

    IEnumerator RunSunset()
    {
        float progress = 0; //This float will serve as the 3rd parameter of the lerp function.
        float increment = sunsetSmoothness / sunsetDuration; // The amount of change to apply.
        while (progress < 1)
        {
            sunsetCurrentColor = Color.Lerp(sunsetStartColor, sunsetTransitionColor, progress);
            RenderSettings.ambientLight = sunsetCurrentColor; 
            progress += increment;
            infoText.text = "SUN SETTING 1: " + progress.ToString();
            yield return new WaitForSeconds(sunsetSmoothness);
        }
        progress = 0;
        while (progress < 1)
        {
            sunsetCurrentColor = Color.Lerp(sunsetTransitionColor, sunsetEndColor, progress);
            RenderSettings.ambientLight = sunsetCurrentColor;
            progress += increment;
            infoText.text = "SUN SETTING 2: " + progress.ToString();
            yield return new WaitForSeconds(sunsetSmoothness);
        }
        progress = 0;
        while (progress < 1)
        {
            duskCurrentColor = Color.Lerp(duskStartColor, duskTransitionColor, progress);
            RenderSettings.ambientLight = duskCurrentColor;
            progress += increment;
            infoText.text = "DUSK 1: " + progress.ToString();
            yield return new WaitForSeconds(duskSmoothness);
        }
        progress = 0;
        while (progress < 1)
        {
            duskCurrentColor = Color.Lerp(duskTransitionColor, duskEndColor, progress);
            RenderSettings.ambientLight = duskCurrentColor;
            progress += increment;
            infoText.text = "DUSK 2: " + progress.ToString();
            yield return new WaitForSeconds(duskSmoothness);
        }
        infoText.text = "";
        yield return true;
    }

    public void PauseTime()
    {
        Time.timeScale = 0.0f;
    }

    public void UnPauseTime()
    {
        Time.timeScale = 1.0f;
    }

}