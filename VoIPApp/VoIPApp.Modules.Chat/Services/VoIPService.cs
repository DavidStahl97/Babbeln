using CPPWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using VoIPApp.Modules.Chat.VoiceServiceReference;

namespace VoIPApp.Modules.Chat.Services
{
    public class VoIPService : IVoIPService
    {
        private readonly AudioStreamingService audioStreamingService;
        private readonly IVoiceService voiceServiceClient;

        public VoIPService(AudioStreamingService audioStreamingService)
        {
            if(audioStreamingService == null)
            {
                throw new ArgumentNullException("audioStreamingService");
            }

            this.audioStreamingService = audioStreamingService;

            BasicHttpBinding binding = new BasicHttpBinding();
            EndpointAddress endpoint = new EndpointAddress("http://localhost/VoIPApp/VoiceService");
            ChannelFactory<IVoiceService> channelFactory = new ChannelFactory<IVoiceService>(binding, endpoint);

            try
            {
                voiceServiceClient = channelFactory.CreateChannel();
            }
            catch
            {
                if(voiceServiceClient != null)
                {
                    ((ICommunicationObject)voiceServiceClient).Abort();
                }
            }
        }

        public async void StartCall(string ip)
        {
            audioStreamingService.StartAsync(ip, 10000);
            int i = await voiceServiceClient.CallAsync(1);
        }

        public void StopCall()
        {
            audioStreamingService.StopAsync();
            if(voiceServiceClient != null)
            {
                ((ICommunicationObject)voiceServiceClient).Close();
            }
        }
    }
}
