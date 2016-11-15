using CPPWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoIPApp.Modules.Chat.VoiceServiceReference;

namespace VoIPApp.Modules.Chat.Services
{
    public class VoIPService : IVoIPService
    {
        private readonly AudioStreamingService audioStreamingService;
        private readonly VoiceServiceClient voiceServiceClient;

        public VoIPService(AudioStreamingService audioStreamingService)
        {
            if(audioStreamingService == null)
            {
                throw new ArgumentNullException("audioStreamingService");
            }

            this.audioStreamingService = audioStreamingService;

            this.voiceServiceClient = new VoiceServiceClient();
        }

        public void StartCall(string ip)
        {
            audioStreamingService.StartAsync(ip, 10000);
            Console.WriteLine(voiceServiceClient.Call(1));
        }

        public void StopCall()
        {
            audioStreamingService.StopAsync();
        }
    }
}
