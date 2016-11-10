using CPPWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoIPApp.Modules.Chat.Services
{
    public class VoIPService : IVoIPService
    {
        private readonly AudioStreamingService audioStreamingService;

        public VoIPService(AudioStreamingService audioStreamingService)
        {
            if(audioStreamingService == null)
            {
                throw new ArgumentNullException("audioStreamingService");
            }

            this.audioStreamingService = audioStreamingService;
        }

        public void StartCall(string ip)
        {
            audioStreamingService.StartAsync(ip, 10000);
        }

        public void StopCall()
        {
            audioStreamingService.StopAsync();
        }
    }
}
