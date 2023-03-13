using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VirtualAudio
{
    internal class TarkovSoundBoard : IDisposable
    {
        private readonly TarkovSoundOut _outputDevice;
        private readonly TarkovVoiceChatTimer _vcTimer = new(20, 4);
        private readonly TarkovVoiceChatController _vcController;
        private readonly IAudioFileProvider _audioFileProvider;
        private PlaybackState _userDesiredPlaybackState = PlaybackState.Stopped;
        private Queue<SoundBoardEffect> _requestQueue = new();

        public PlaybackState PlaybackState { get => _outputDevice.PlaybackState; }
        public AudioFileReader? AudioFileReader { get => _outputDevice.AudioFileReader; }

        public TarkovSoundBoard(IAudioFileProvider audioFileProvider)
        {
            _audioFileProvider = audioFileProvider;
            _outputDevice = new TarkovSoundOut();
            _vcController = new(_vcTimer);
            _vcTimer.OnTimerHitZero = () =>
            {
                InternalPause();
            };

            _vcTimer.OnTimerReset = () =>
            {
                if (_userDesiredPlaybackState == PlaybackState.Playing)
                {
                    if (_requestQueue.Count > 0)
                    {
                        PlayTrackFromQueue();
                    }
                    else
                    {
                        InternalContinue();
                    }
                }
            };
        }

        public void SetStreamPosition(long offset, SeekOrigin origin)
        {
            _outputDevice.SetStreamPosition(offset, origin);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns>true if the track exist for given index</returns>
        public bool PlaySound(int index)
        {
            var sound = _audioFileProvider.GetEffect(index);

            if (sound != null)
            {
                _userDesiredPlaybackState = PlaybackState.Playing;
                _requestQueue.Clear();
                _requestQueue.Enqueue(sound);

                if (!_vcTimer.IsAudioAllowed())
                {
                    // We found the track but audio is not allowed now, so it is queued for future
                    return true;
                }

                PlayTrackFromQueue();
                return true;
            }

            return false;
        }

        public void PauseOrPlay()
        {
            // We must pause or at least request pausing if the audio is playing or if it has been stopped by the timer
            if (_outputDevice.PlaybackState == PlaybackState.Playing || !_vcTimer.IsAudioAllowed())
            {
                Pause();
            }
            else
            {
                Continue();
            }
        }

        public void Continue()
        {
            InternalContinue();
            _userDesiredPlaybackState = PlaybackState.Playing;
        }

        public void Stop()
        {
            InternalStop();
            _userDesiredPlaybackState = PlaybackState.Stopped;
        }

        public void Pause()
        {
            InternalPause();
            _userDesiredPlaybackState = PlaybackState.Paused;
        }

        private void PlayTrackFromQueue()
        {
            if (_outputDevice.PlaybackState == PlaybackState.Playing)
            {
                _outputDevice.Stop();
            }

            var sound = _requestQueue.Dequeue();

            _outputDevice.Play(sound);
            OpenVoiceChat();
        }

        private void InternalContinue()
        {
            if (!_vcTimer.IsAudioAllowed()) return;

            if (_outputDevice.HasTrackEnded())
            {
                _outputDevice.Stop();
            }

            OpenVoiceChat();
            _outputDevice.Play();
        }

        private void InternalStop()
        {
            CloseVoiceChat();
            _outputDevice.Stop();
        }

        private void InternalPause()
        {
            CloseVoiceChat();
            _outputDevice.Pause();
        }

        private void OpenVoiceChat()
        {
            _vcController.OpenVoiceChat();
        }

        private void CloseVoiceChat()
        {
            _vcController.CloseVoiceChat();
        }

        public void Dispose()
        {
            _outputDevice.Dispose();
        }
    }
}
