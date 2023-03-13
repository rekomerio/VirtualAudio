using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Linq;
using NAudio;
using NAudio.Wave;
using System.Windows.Input;

namespace VirtualAudio.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var hook = new TarkovKeyboardHook())
            {
                hook.SetupKeyboardHooks();
                Application.Run();
            }
        }
    }
}