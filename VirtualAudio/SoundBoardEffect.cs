using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualAudio
{
    public class SoundBoardEffect
    {
        public string Filename { get; set; } = string.Empty;
        public long InitialStreamPosition { get; set; }
        public int Index { get; set; }
    }
}
