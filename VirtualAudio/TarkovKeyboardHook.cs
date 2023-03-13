using Microsoft.VisualBasic.Devices;
using NAudio.Wave;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VirtualAudio
{
    public class TarkovKeyboardHook : IDisposable
    {
        private GlobalKeyboardHook? _globalKeyboardHook;
        private TarkovSoundBoard _soundBoard;
        private IAudioFileProvider _audioFileProvider = new AudioFileProvider();

        public TarkovKeyboardHook()
        {
            _audioFileProvider.LoadFile();
            _soundBoard = new TarkovSoundBoard(_audioFileProvider);
        }

        public void SetupKeyboardHooks()
        {
            _globalKeyboardHook = new GlobalKeyboardHook();
            _globalKeyboardHook.KeyboardPressed += OnKeyPressed;
        }

        private void OnKeyPressed(object sender, GlobalKeyboardHookEventArgs e)
        {

            if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown && Control.IsKeyLocked(Keys.Scroll))
            {
                if (e.KeyboardData.VirtualCode is >= (int)Keys.NumPad0 and <= (int)Keys.NumPad9)
                {
                    _soundBoard.PlaySound(e.KeyboardData.VirtualCode - (int)Keys.NumPad0);
                }

                switch ((Keys)e.KeyboardData.VirtualCode)
                {
                    case Keys.Decimal:
                        _soundBoard.Stop();
                        break;
                    case Keys.Enter:
                        _soundBoard.PauseOrPlay();
                        break;
                    case Keys.Add:
                        if (_soundBoard.AudioFileReader is not null)
                        {
                            _soundBoard.SetStreamPosition(450_000, SeekOrigin.Current);
                        }
                        break;
                    case Keys.Subtract:
                        if (_soundBoard.AudioFileReader != null && _soundBoard.AudioFileReader.Position > 450_000)
                        {
                            _soundBoard.SetStreamPosition(-450_000, SeekOrigin.Current);
                        }
                        break;
                }
                //Console.WriteLine(e.KeyboardData.VirtualCode);
                //e.Handled = true; only set to true if you dont want the event to be sent for the active window0,,,
            }
        }

        public void Dispose()
        {
            _globalKeyboardHook?.Dispose();
            _soundBoard?.Dispose();
        }
    }
}
