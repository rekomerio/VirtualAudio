using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualAudio
{
    internal class TarkovSoundOut : IDisposable
    {
        private IWavePlayer? _physicalOutputDevice;
        private IWavePlayer? _virtualOutputDevice;
        private AudioFileReader? _physicalAudioReader;
        private AudioFileReader? _virtualAudioReader;
        private SoundBoardEffect? _soundBoardEffect;
        private bool _hasAudio = false;

        public TarkovSoundOut()
        {
            InitDevices();
        }

        public AudioFileReader? AudioFileReader { get => _virtualAudioReader; }
        public PlaybackState PlaybackState => _virtualOutputDevice.PlaybackState;
        public WaveFormat OutputWaveFormat => _virtualOutputDevice.OutputWaveFormat;

        public event EventHandler<StoppedEventArgs> PlaybackStopped;

        public void SetStreamPosition(long offset, SeekOrigin origin)
        {
            _virtualAudioReader.Seek(offset, origin);
            _physicalAudioReader.Seek(offset, origin);
        }

        // Start playing audio from given sound effect
        public void Play(SoundBoardEffect sound)
        {
            InitPlay(sound);
            Play();
        }

        /// <summary>
        /// Play the current audio source
        /// </summary>
        public void Play()
        {
            if (!_hasAudio) return;

            _virtualOutputDevice?.Play();
            _physicalOutputDevice?.Play();
        }

        public void Pause()
        {
            if (!_hasAudio) return;

            _virtualOutputDevice?.Pause();
            _physicalOutputDevice?.Pause();
        }

        public void Stop()
        {
            if (!_hasAudio) return;

            _virtualOutputDevice?.Stop();
            _physicalOutputDevice?.Stop();
            _virtualAudioReader?.Seek(_soundBoardEffect.InitialStreamPosition, SeekOrigin.Begin);
            _physicalAudioReader?.Seek(_soundBoardEffect.InitialStreamPosition, SeekOrigin.Begin);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if the playback ended because the track has run till the end</returns>
        public bool HasTrackEnded()
        {
            return _virtualAudioReader?.Position != _soundBoardEffect?.InitialStreamPosition && _virtualOutputDevice?.PlaybackState == PlaybackState.Stopped;
        }

        private void InitDevices()
        {
            Dispose();

            var v = DirectSoundOut.Devices.Where(x => x.Description.StartsWith("CABLE")).First();
            var p = DirectSoundOut.Devices.Where(x => x.Description.StartsWith("Realtek") || x.Description.StartsWith("Kuulokkeet")).First();

            _virtualOutputDevice = new DirectSoundOut(v.Guid);
            _physicalOutputDevice = new DirectSoundOut(p.Guid);
        }

        private void InitPlay(SoundBoardEffect sound)
        {
            InitDevices();

            _soundBoardEffect = sound;

            _physicalAudioReader = new AudioFileReader(_soundBoardEffect.Filename);
            _physicalAudioReader.Seek(_soundBoardEffect.InitialStreamPosition, SeekOrigin.Begin);

            _virtualAudioReader = new AudioFileReader(_soundBoardEffect.Filename);
            _virtualAudioReader.Seek(_soundBoardEffect.InitialStreamPosition, SeekOrigin.Begin);

            _virtualOutputDevice.Init(_virtualAudioReader);
            _physicalOutputDevice.Init(_physicalAudioReader);

            _physicalAudioReader.Volume = 0.1f;

            _hasAudio = true;
        }

        public void Dispose()
        {
            _virtualOutputDevice?.Dispose();
            _physicalOutputDevice?.Dispose();
            _virtualAudioReader?.Dispose();
            _physicalAudioReader?.Dispose();
        }
    }
}
