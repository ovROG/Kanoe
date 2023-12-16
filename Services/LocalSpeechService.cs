using Kanoe.Shared;
using System.Runtime.InteropServices;
using System.Speech.Synthesis;

namespace Kanoe.Services
{
    public class LocalSpeechService
    {
        public Task TTSToAudoFile(string text, string path, string? voice = "")
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string fullPath = @$"{Directory.GetCurrentDirectory()}{path}";
                SpeechSynthesizer synth = new();

                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? "");
                }
                catch
                {
                    synth.Dispose();
                    Logger.Error("UNABLE TO CREATE TTS FOLDER");
                    return Task.CompletedTask; // Maybe? handle fail
                }

                if (!string.IsNullOrEmpty(voice))
                {
                    synth.SelectVoice(voice);
                }

                synth.SetOutputToWaveFile(fullPath);
                synth.Speak(text);
                synth.Dispose();
            }
            return Task.CompletedTask;
        }

        public List<string> GetTTSVoices()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
#pragma warning disable CA1416 // ignore windows warning in "ConvertAll"
                SpeechSynthesizer synth = new();
                return synth.GetInstalledVoices().ToList().ConvertAll(voice => voice.VoiceInfo.Name);
#pragma warning restore
            }
            return new List<string>();
        }
    }
}
