using System;
using UnityEngine;
using UnityEngine.Events;

namespace World
{
    //[ExecuteInEditMode]
    public class TimeController : MonoBehaviour
    {
        public static TimeController Instance;

        [SerializeField] public AnimationCurve sunCurve;
        [SerializeField, Range(0, 24)] public float dayTime;
        [SerializeField] public int dayCounter = 1;

        public float timeSpeedMultiplier = 1;
        public static DayEnum DayEnum;
        public bool stopTime;

        public UnityEvent OnDayChanged;

        public void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (SaveSystem.saveContent.dayCounter != null)
            {
                dayCounter = SaveSystem.saveContent.dayCounter.Value;
            }

            if (SaveSystem.saveContent.dayTime != null)
            {
                dayTime = SaveSystem.saveContent.dayTime.Value;
            }

            SaveSystem.OnUpdateSaveContent += UpdateSave;
        }

        private void Update()
        {
            if (stopTime == false)
            {
                dayTime += Time.deltaTime / 60 * timeSpeedMultiplier;
                if (dayTime >= 24)
                {
                    dayTime -= 24;
                    dayCounter++;
                    OnDayChanged.Invoke();
                }
                UpdateDayEnum();
                UpdateClockUI();
            }

            if (dayCounter >= 100)
            {
                Debug.Log("Time to complete main storyline has ended, game over :)))");
                SaveSystem.DeleteSave(SaveSystem.currentSaveName);
                GameLoader.Instance.LoadLoseScene();
                //GameLoader.Instance.LoadMainMenu();
            }
        }

        private void UpdateDayEnum()
        {
            if (dayTime >= (int)DayEnum.NIGHT)
            {
                DayEnum = DayEnum.NIGHT;
            }
            else if (dayTime >= (int)DayEnum.SUNSET)
            {
                DayEnum = DayEnum.SUNSET;
            }
            else if (dayTime >= (int)DayEnum.AFTERNOON)
            {
                DayEnum = DayEnum.AFTERNOON;
            }
            else if (dayTime >= (int)DayEnum.NOON)
            {
                DayEnum = DayEnum.NOON;
            }
            else if (dayTime >= (int)DayEnum.MORNING)
            {
                DayEnum = DayEnum.MORNING;
            }
            else if (dayTime >= (int)DayEnum.SUNRISE)
            {
                DayEnum = DayEnum.SUNRISE;
            }
        }

        private void UpdateClockUI()
        {
            CanvasController.Instance.clockInterface.SetDay(dayCounter);
            CanvasController.Instance.clockInterface.SetHour(dayTime);
        }

        public void SkipTime(int minutes)
        {
            if (dayTime + (minutes / 60f) >= 24)
            {
                dayTime = (dayTime + (minutes / 60f)) - 24;
            }
            else
            {
                dayTime += (minutes / 60f);
            }

            UpdateClockUI();
        }

        public void SkipTime(DayEnum dayEnum)
        {
            dayTime = (int)dayEnum;
            UpdateClockUI();
        }

        public void SetTime(int hour)
        {
            dayTime = hour;
            UpdateClockUI();
        }

        public void SetTimeSpeed(int speed)
        {
            timeSpeedMultiplier = speed;
        }


        public void UpdateSave()
        {
            SaveSystem.saveContent.dayCounter = dayCounter;
            SaveSystem.saveContent.dayTime = dayTime;
        }

        private void OnDestroy()
        {
            SaveSystem.OnUpdateSaveContent -= UpdateSave;
        }
    }

    public enum DayEnum
    {
        SUNRISE = 6,
        MORNING = 8,
        NOON = 12,
        AFTERNOON = 16,
        SUNSET = 20,
        NIGHT = 22
    }
}