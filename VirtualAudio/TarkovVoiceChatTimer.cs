using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualAudio
{
    internal class TarkovVoiceChatTimer
    {
        enum TimerState
        {
            Idle,
            Active,
            Cooldown
        }

        public Action? OnTimerHitZero { get; set; }
        public Action? OnTimerReset { get; set; }

        public int TimeLeftInVc { get; private set; }

        private int _ticks = 0;
        private PeriodicTask _timer;
        private TimerState _timerState = TimerState.Idle;

        private int _vcTimeSeconds = 20;
        private int _cooldownSeconds = 4;

        public TarkovVoiceChatTimer(int vcTimeSeconds = 20, int cooldownSeconds = 4)
        {
            _vcTimeSeconds = vcTimeSeconds;
            _cooldownSeconds = cooldownSeconds;
            TimeLeftInVc = vcTimeSeconds;
            _timer = new(OnTick, TimeSpan.FromSeconds(1));
            _timer.StartTask();
        }

        public bool IsAudioAllowed()
        {
            return TimeLeftInVc > 0;
        }

        private void OnTick()
        {
            if (_timerState == TimerState.Idle) return;

            _ticks++;

            if (_timerState == TimerState.Active && TimeLeftInVc > 0)
            {
                TimeLeftInVc--;

                if (TimeLeftInVc == 0)
                {
                    OnTimerHitZero?.Invoke();
                }
            }

            if (_timerState == TimerState.Cooldown)
            {
                if (_ticks >= _cooldownSeconds)
                {
                    TimeLeftInVc = _vcTimeSeconds;
                    _timerState = TimerState.Idle;
                    OnTimerReset?.Invoke();
                }
            }

            Console.WriteLine($"Time left: {TimeLeftInVc} s");
        }

        public void OnKeyReleased()
        {
            _timerState = TimerState.Cooldown;
            _ticks = 0;
        }

        public void OnKeyPressed()
        {
            _timerState = TimerState.Active;
            _ticks = 0;
        }
    }
}
