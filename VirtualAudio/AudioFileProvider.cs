using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

namespace VirtualAudio
{
    public interface IAudioFileProvider
    {
        SoundBoardEffect? GetEffect(int index);
        void LoadFile();
    }

    internal class AudioFileProvider : IAudioFileProvider
    {
        private List<SoundBoardEffect> _sounds = new();

        public SoundBoardEffect? GetEffect(int index)
        {
            if (_sounds.Count == 0)
            {
                LoadFile();
            }

            return _sounds.FirstOrDefault(x => x.Index == index);
        }

        public void LoadFile()
        {
            try
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(PascalCaseNamingConvention.Instance)
                    .Build();

                using var stream = new FileStream(@"C:\Users\Admin\Desktop\Audio\config.yaml", FileMode.Open);
                using var streamReader = new StreamReader(stream);
                var output = deserializer.Deserialize<List<SoundBoardEffect>>(streamReader);

                _sounds = output.Select(x =>
                {
                    x.Filename = @$"C:\Users\Admin\Desktop\Audio\{x.Filename}";
                    return x;
                }).ToList();

                Console.WriteLine("YAML loaded");
                if (_sounds.Count == 0)
                {
                    Console.WriteLine("YAML has no songs defined");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to load audio config, please check the file and restart the application");
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
