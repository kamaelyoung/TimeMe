using System;
using System.Threading;

namespace TimeMe.Services
{
    public class TimerService
    {
        private int _msBetweenEvents = 100;
        private Timer _timer;

        public TimerService(TimerCallback del)
        {
            _timer = new Timer(del, new TimeSpan(0, 0, 0, 0, _msBetweenEvents), System.Threading.Timeout.Infinite, _msBetweenEvents);
        }

        public void Start()
        {
            _timer.Change(_msBetweenEvents, _msBetweenEvents);
        }

        public void Stop()
        {
            _timer.Change(System.Threading.Timeout.Infinite, _msBetweenEvents);
        }

        public void Pause()
        {
            _timer.Change(System.Threading.Timeout.Infinite, _msBetweenEvents);
        }
    }
}
