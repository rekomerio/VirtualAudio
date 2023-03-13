using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualAudio
{
    internal class AudioReaderObserver
    {
        private PeriodicTask _timer;
        private IWavePlayer _player;

        public AudioReaderObserver(IWavePlayer player)
        {
            _player = player;
            _timer = new(OnTick, TimeSpan.FromMilliseconds(25));
            _timer.StartTask();
        }

        private PlaybackState playbackState;

        public void OnTick()
        {

        }
    }
}
