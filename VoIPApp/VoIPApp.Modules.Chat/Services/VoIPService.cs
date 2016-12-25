using CPPWrapper;
using MongoDB.Bson;
using SharedCode.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using VoIPApp.Common.Services;

namespace VoIPApp.Modules.Chat.Services
{
    public class VoIPService : IVoIPService
    {
        private readonly AudioStreamingService audioStreamingService;
        private readonly ServerServiceProxy serverServiceProxy;
        private bool startedAudioStreaming;

        public VoIPService(AudioStreamingService audioStreamingService, ServerServiceProxy serverServiceProxy)
        {
            if(audioStreamingService == null)
            {
                throw new ArgumentNullException("audioStreamingService");
            }

            this.audioStreamingService = audioStreamingService;
            this.serverServiceProxy = serverServiceProxy;
        }

        public async Task StartCallSession(Friend f)
        {
            await serverServiceProxy.ServerService.CallAsync(f._id);
        }

        public async Task AcceptCall(Friend f)
        {
            startedAudioStreaming = true;
            audioStreamingService.StartAsync(f.IP, 10000);
            await serverServiceProxy.ServerService.AcceptCallAsync(f._id);
        }

        public async Task CancelCall(Friend f)
        {
            if(startedAudioStreaming)
            {
                audioStreamingService.StopAsync();
                startedAudioStreaming = false;
            }
            await serverServiceProxy.ServerService.CancelCallAsync(f._id);
        }
    }
}
