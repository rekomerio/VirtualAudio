using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VirtualAudio
{
    internal class TarkovVoiceChatController
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        const int KEY_DOWN_EVENT = 0x0001;
        const int KEY_UP_EVENT = 0x0002;
        private readonly TarkovVoiceChatTimer _timer;

        public bool IsVoiceChatOpen { get; private set; } = false;

        public TarkovVoiceChatController(TarkovVoiceChatTimer vcTimer)
        {
            _timer = vcTimer;
        }

        public void OpenVoiceChat()
        {
            if (!IsVoiceChatOpen)
            {
                keybd_event(75, 0, KEY_DOWN_EVENT, 0);
                IsVoiceChatOpen = true;
                _timer.OnKeyPressed();
                Console.WriteLine("VC OPEN");
            }
        }

        public void CloseVoiceChat()
        {
            if (IsVoiceChatOpen)
            {
                keybd_event(75, 0, KEY_UP_EVENT, 0);
                IsVoiceChatOpen = false;
                _timer.OnKeyReleased();
                Console.WriteLine("VC CLOSED");
            }
        }
    }
}
