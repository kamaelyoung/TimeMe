using System;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Collections.ObjectModel;
using System.Linq;
using TimeMe.Services;
using System.IO;

namespace TimeMe.ViewModels
{
    public class ApplicationModel : INotifyPropertyChanged
    {
        private TimerService _timerService;
        private TimerService TimerService
        {
            get
            {
                return _timerService;
            }
            set
            {
                _timerService = value;
            }
        }

        private TimeSpan _duration;
        public TimeSpan Duration
        {
            get
            {
                return _duration;
            }
            set
            {
                _duration = value;
                NotifyPropertyChanged("Duration");
                NotifyPropertyChanged("DurationAsString");
            }
        }

        public string DurationAsString
        {
            get
            {
                return string.Format("{0:00}:{1:00}.{2:0}", (int)Duration.TotalMinutes, Duration.Seconds, Duration.Milliseconds / 100);
            }
        }

        private ObservableCollection<LapTimeModel> _lapsList;
        public ObservableCollection<LapTimeModel> LapsList
        {
            get 
            {
                if (_lapsList == null)
                    _lapsList = new ObservableCollection<LapTimeModel>();
                return _lapsList; 
            }
        }

        private ApplicationStateType _applicationState = ApplicationStateType.Ready;
        public ApplicationStateType ApplicationState 
        {
            get
            {
                return _applicationState;
            }
            set
            {
                _applicationState = value;
            }
        }

        public bool IsInitialized
        {
            get;
            private set;
        }

        public delegate void TimerServiceTimeChangedHandler(TimeSpan elapsedTime);
        public event TimerServiceTimeChangedHandler TimerServiceTimeChanged;

        public void StartRun()
        {
            TimerService.Start();
            ApplicationState = ApplicationStateType.Running;
        }

        public void ResetRun()
        {
            LapsList.Clear();
            Duration = new TimeSpan(0, 0, 0);
            ApplicationState = ApplicationStateType.Ready;
        }

        public delegate void MaxTimeReachedHandler();
        public event MaxTimeReachedHandler MaxTimeReached;

        public void IncrementDuration(TimeSpan elapsedTime)
        {
            var newDuration = Duration.Add(elapsedTime);
            if ((int)newDuration.TotalMinutes > 99)
            {
                Duration = new TimeSpan(0,0,99,59,900);
                if (MaxTimeReached != null)
                    MaxTimeReached();
            }
            else
            {
                Duration = newDuration;
            }
        }

        public LapTimeModel RecordLapTime()
        {
            LapTimeModel newLap;
            LapTimeModel lastLap = LapsList.LastOrDefault();
            if (lastLap != null)
            {
                newLap = new LapTimeModel()
                {
                    LapNo = LapsList.Count + 1,
                    LapTime = Duration - lastLap.TimeStamp,
                    TimeStamp = Duration
                };
            }
            else
            {
                newLap = new LapTimeModel()
                {
                    LapNo = 1,
                    LapTime = Duration,
                    TimeStamp = Duration
                };
            }

            LapsList.Add(newLap);

            return newLap;
        }

        public LapTimeModel StopRun()
        {
            TimerService.Stop();
            var newLap = RecordLapTime();
            ApplicationState = ApplicationStateType.Completed;
            return newLap;
        }

        public void Initialize()
        {
            TimerService = new TimerService(InvokeTimerChanged);
            IsInitialized = true;
        }

        private const string appStateFileName = "TimeMe.state";

        public void HibernateRun()
        {
            if (ApplicationState == ApplicationStateType.Running)
                StopRun();

            if (ApplicationState == ApplicationStateType.Completed)
            {

                using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (BinaryWriter bw = new BinaryWriter(new IsolatedStorageFileStream(appStateFileName, FileMode.Create, isoStore)))
                    {
                        bw.Write((int)ApplicationState);
                        bw.Write(Duration.Hours);
                        bw.Write(Duration.Minutes);
                        bw.Write(Duration.Seconds);
                        bw.Write(Duration.Milliseconds);
                        bw.Write(LapsList.Count);
                        foreach (LapTimeModel lap in LapsList)
                        {
                            lap.Save(bw);
                        }
                        bw.Close();
                    }
                }
            }
        }

        public void ResumeRun()
        {
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isoStore.FileExists(appStateFileName))
                {
                    using (BinaryReader br = new BinaryReader(new IsolatedStorageFileStream(appStateFileName, FileMode.Open, isoStore)))
                    {
                        ApplicationState = (ApplicationStateType)(br.ReadInt32());
                        int hours = br.ReadInt32();
                        int minutes = br.ReadInt32();
                        int seconds = br.ReadInt32();
                        int miliseconds = br.ReadInt32();
                        Duration = new TimeSpan(0, hours, minutes, seconds, miliseconds);
                        int lapCount = br.ReadInt32();
                        for (int i = 0; i < lapCount; i++)
                        {
                            LapsList.Add(LapTimeModel.Load(br));
                        }
                        br.Close();
                    }
                    isoStore.DeleteFile(appStateFileName);
                }
            }
        }

        #region Private methods

        private void InvokeTimerChanged(object state)
        {
            if (TimerServiceTimeChanged != null)
                TimerServiceTimeChanged((TimeSpan)state);
        }

        #endregion


        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}