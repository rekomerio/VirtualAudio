using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualAudio
{
    class PeriodicTask
    {
        public PeriodicTask(Action onTick, TimeSpan interval)
        {
            _onTick = onTick;
            _timer = new PeriodicTimer(interval);
        }

        public long Ticks { get; private set; } = 0;

        private Task? _task;
        private readonly PeriodicTimer _timer;
        private readonly Action _onTick;
        private CancellationTokenSource _token = new CancellationTokenSource();

        public void StartTask()
        {
            _token ??= new CancellationTokenSource();

            _task = Task.Run(async () =>
            {
                while (await _timer.WaitForNextTickAsync(_token.Token))
                {
                    _onTick();
                }
            });
        }

        public void StopTask()
        {
            _token.Cancel();
        }
    }
}
