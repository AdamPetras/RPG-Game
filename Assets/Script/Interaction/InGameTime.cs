using System;
using System.Runtime.Remoting.Messaging;
using Assets.Script.HUD;
using Assets.Script.Menu;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Script.Interaction
{

    [Serializable]
    public class InGameTime : MonoBehaviour
    {
        [Serializable]
        public class MyTime
        {
            private bool Equals(MyTime other)
            {
                return Seconds == other.Seconds && Minutes == other.Minutes && Hours == other.Hours;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((MyTime) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = Seconds;
                    hashCode = (hashCode * 397) ^ Minutes;
                    hashCode = (hashCode * 397) ^ Hours;
                    return hashCode;
                }
            }

            public int Seconds;
            public int Minutes;
            public int Hours;

            public MyTime()
            {
                Seconds = 0;
                Minutes = 0;
                Hours = 0;
            }

            public MyTime(int hours, int minutes, int seconds)
            {
                Seconds = seconds;
                Minutes = minutes;
                Hours = hours;
            }

            public override string ToString()
            {
                return Hours + ":" + Minutes + ":" + Seconds;
            }

            public static MyTime operator +(MyTime c1, MyTime c2)
            {
                if (c1 == null || c2 == null)
                    return null;
                MyTime time = new MyTime
                {
                    Seconds = PlusHelp(c1.Seconds, ref c1.Minutes, c2.Seconds, 60),
                    Minutes = PlusHelp(c1.Minutes, ref c1.Hours, c2.Minutes, 60)
                };
                int hours = 0;
                time.Hours = PlusHelp(c1.Hours, ref hours, c2.Hours, 24);
                time.Hours += hours;
                return time;
            }

            public static MyTime operator -(MyTime c1, MyTime c2)
            {
                if (c1 == null || c2 == null)
                    return null;
                MyTime time = new MyTime
                {
                    Seconds = MinusHelp(c1.Seconds, ref c1.Minutes, c2.Seconds, 60),
                    Minutes = MinusHelp(c1.Minutes, ref c1.Hours, c2.Minutes, 60)
                };
                int hours = 0;
                time.Hours = MinusHelp(c1.Hours, ref hours, c2.Hours, 24);
                time.Hours += hours;
                return time;
            }

            public static void TimePlusFloat(MyTime actTime, float timeConst)
            {
                MyTime time = new MyTime();
                actTime.Hours += (int) Math.Truncate(timeConst);
                actTime.Minutes += (int) (Mathf.Repeat(timeConst, 1) * 60);
                while (actTime.Minutes >= 60)
                {
                    actTime.Minutes -= 60;
                    actTime.Hours++;
                }
                if (actTime.Hours >= 24)
                {
                    actTime.Hours %= 24;
                }
            }

            /*public static bool operator ==(MyTime c1, MyTime c2)
            {
                if (c1 == null || c2 == null)
                    return false;
                return c1.Hours == c2.Hours && c1.Minutes == c2.Minutes && c1.Seconds == c2.Seconds;
            }

            public static bool operator !=(MyTime c1, MyTime c2)
            {
                return !(c1 == c2);
            }*/

            public static bool operator <=(MyTime c1, MyTime c2)
            {
                if (c1 == null || c2 == null)
                    return false;
                if (c1.Hours < c2.Hours)
                {
                    return true;
                }
                if (c1.Hours == c2.Hours)
                {
                    if (c1.Minutes < c2.Minutes)
                    {
                        return true;
                    }
                    if (c1.Minutes == c2.Minutes)
                    {
                        if (c1.Seconds < c2.Seconds)
                            return true;
                        if (c1.Seconds == c2.Seconds)
                            return true;
                    }
                }
                return false;
            }

            public static bool operator >=(MyTime c1, MyTime c2)
            {
                if (c1 == null || c2 == null)
                    return false;
                if (c1.Hours > c2.Hours)
                {
                    return true;
                }
                if (c1.Hours == c2.Hours)
                {
                    if (c1.Minutes > c2.Minutes)
                    {
                        return true;
                    }
                    if (c1.Minutes == c2.Minutes)
                    {
                        if (c1.Seconds > c2.Seconds)
                            return true;
                        if (c1.Seconds == c2.Seconds)
                            return true;
                    }
                }
                return false;
            }

            static int mod(int x, int m)
            {
                return (x % m + m) % m;
            }

            private static int MinusHelp(int time1, ref int time1Up, int time2, int offset)
            {
                int decrem = 0;
                while (time1 - time2 < 0)
                {
                    decrem -= 1;
                    time2 -= offset;
                }
                time1Up += decrem;
                return time1 - time2;
            }

            private static int PlusHelp(int time1, ref int time1Up, int time2, int offset)
            {
                int increm = 0;
                while (time1 + time2 > offset)
                {
                    increm += 1;
                    time2 -= offset;
                }
                time1Up += increm;
                return time1 + time2;
            }

            public static bool IfTimeIsSmallerThan(MyTime time1, MyTime referenceTime, MyTime maxLimit)
            {
                if (time1 == null || referenceTime == null || maxLimit == null)
                    return false;
                if ((referenceTime - time1) <= maxLimit)
                {
                    Debug.Log((referenceTime - time1));
                    return true;
                }
                return false;
            }
        }

        public enum EDay
        {
            Monday,
            Tuesday,
            Wednesday,
            Thursday,
            Friday,
            Saturday,
            Sunday
        }

        private MyTime _actualTime;
        private int numOfDay;
        private float _hoursHand;
        private Transform _clockFace;
        private Image _clockHand;
        private Button _sleepButton;
        private Slider _sleepSetter;
        private Text _textSleepTime;
        private Text _textActTime;
        private Image _sleepImageShow;
        private GameObject _background;
        private GameObject _sun;
        public static bool Visible;
        private static readonly MyTime MAXSLEEPTIME = new MyTime(12, 00, 00);

        private void Start()
        {
            _actualTime = new MyTime(12, 0, 0);
            InvokeRepeating("TimeRun", 0, 0.2f);
            _clockFace = transform.Find("Clock");
            _clockHand = transform.Find("Clock").Find("ClockHand").GetComponent<Image>();
            _sleepSetter = transform.Find("SetSlider").GetComponent<Slider>();
            _textSleepTime = transform.Find("TextSleepTime").GetComponent<Text>();
            _textActTime = transform.Find("TextActTime").GetComponent<Text>();
            _sleepImageShow = transform.Find("Clock").Find("Image").GetComponent<Image>();
            _sleepSetter.onValueChanged.AddListener(ShowSleepTime);
            _sleepButton = transform.Find("SetTime").GetComponent<Button>();
            _background = transform.Find("Background").gameObject;
            _sleepButton.onClick.AddListener(Sleep);
            numOfDay = 0;
            _sun = GameObject.Find("Sun");
            TimeToSunRotation(_actualTime);
            OnHide();
        }

        public void Update()
        {
            if (BlackScreen.Visible || MainMenu.Visible)
                return;
            if (Input.GetKeyUp(KeyCode.T) && !Visible)
            {
                MainPanel.CloseAllWindows();
                OnVisible();
            }
            else if ((Input.GetKeyUp(KeyCode.T) || Input.GetKeyUp(KeyCode.Escape)) && Visible)
                OnHide();
            if (Input.GetKeyUp(KeyCode.RightArrow) && Visible)
            {
                _sleepSetter.value++;
                ShowSleepTime(_sleepSetter.value);
            }
            if (Input.GetKeyUp(KeyCode.LeftArrow) && Visible)
            {
                _sleepSetter.value--;
                ShowSleepTime(_sleepSetter.value);
            }
        }

        public void OnVisible()
        {
            _clockHand.gameObject.SetActive(true);
            _sleepSetter.gameObject.SetActive(true);
            _sleepButton.gameObject.SetActive(true);
            _sleepImageShow.gameObject.SetActive(true);
            _textSleepTime.gameObject.SetActive(true);
            _textActTime.gameObject.SetActive(true);
            _clockFace.gameObject.SetActive(true);
            _background.SetActive(true);
            Visible = true;
        }

        public void OnHide()
        {
            _clockHand.gameObject.SetActive(false);
            _sleepSetter.gameObject.SetActive(false);
            _sleepButton.gameObject.SetActive(false);
            _sleepImageShow.gameObject.SetActive(false);
            _textSleepTime.gameObject.SetActive(false);
            _textActTime.gameObject.SetActive(false);
            _clockFace.gameObject.SetActive(false);
            _background.SetActive(false);
            Visible = false;
        }

        private void ShowSleepTime(float value)
        {
            if (Math.Truncate(value) < 10)
                _textSleepTime.text = "0";
            else _textSleepTime.text = "";
            _textSleepTime.text += Math.Truncate(value) + ":";
            if (Mathf.Repeat(value, 1) * 60 < 10)
                _textSleepTime.text += "0";
            _textSleepTime.text += (int) (Mathf.Repeat(value, 1) * 60) + ":" + "00";
            _sleepImageShow.fillAmount = value / 12;
        }

        public float PointsToAngle(Vector2 this_, Vector2 to)
        {
            Vector2 direction = to - this_;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (angle < 0f) angle += 360f;
            return angle - 90;
        }

        private void Sleep()
        {
            SliderToTime();
            SetUpHand();
        }

        private int TimeToAngle(MyTime tmpTime)
        {
            return (int)(0.25*(60 * tmpTime.Hours + tmpTime.Minutes));
        }

        private float TimeToSunRotation(MyTime time)
        {
            if (time >= new MyTime(6, 0, 0) && time <= new MyTime(22, 0, 0))
            {
                _sun.GetComponent<Light>().intensity = 0.6f;
                return (time.Hours*60 - 6*60 + time.Minutes) * 0.1875f;
            }
            if (time <= new MyTime(24, 0, 0) && time >= new MyTime(22, 0, 0))
            {
                _sun.GetComponent<Light>().intensity = 0.6f;
                return -180 + (time.Hours * 60 - 22 * 60 + time.Minutes) * 0.375f;
            }
            _sun.GetComponent<Light>().intensity = 0.6f;
            return (45 + (time.Hours * 60 + time.Minutes) * 0.375f) - 180;
        }

        private void SliderToTime()
        {
            float value = _sleepSetter.value;
            MyTime.TimePlusFloat(_actualTime, value);
            _sleepSetter.value = 0;
            
        }

        private void SetUpHand()
        {
            _hoursHand = (60 * _actualTime.Hours + _actualTime.Minutes) / 2;
            _clockHand.transform.localRotation = Quaternion.Euler(0, 0, -_hoursHand);
            _sleepImageShow.transform.localRotation = Quaternion.Euler(0, 0, -_hoursHand);
            _sun.transform.localRotation = Quaternion.Euler(TimeToSunRotation(_actualTime), 0, 0);
            // SetupActualTimeShower();
        }

        private void SetupActualTimeShower()
        {
            _textActTime.text = "Actual time:\n";
            if (_actualTime.Hours < 10)
                _textActTime.text += "0";
            _textActTime.text += _actualTime.Hours + ":";
            if (_actualTime.Minutes < 10)
                _textActTime.text += "0";
            _textActTime.text += _actualTime.Minutes + ":";
            if (_actualTime.Seconds < 10)
                _textActTime.text += "0";
            _textActTime.text += _actualTime.Seconds;
        }

        private void TimeRun()
        {
            SetupActualTimeShower();
            _actualTime.Seconds++;
            if (_actualTime.Seconds >= 60)
            {
                SetUpHand();
                _actualTime.Seconds = 0;
                _actualTime.Minutes++;
                if (_actualTime.Minutes >= 60)
                {
                    _actualTime.Minutes = 0;
                    _actualTime.Hours++;
                    if (_actualTime.Hours >= 24)
                    {
                        _actualTime.Hours = 0;
                    }
                }
            }
        }

        public MyTime GetActualTime()
        {
            return _actualTime;
        }

        public void SetActualTime(MyTime time)
        {
            _actualTime = time;
            TimeToSunRotation(_actualTime);
            SetUpHand();
        }
    }
}