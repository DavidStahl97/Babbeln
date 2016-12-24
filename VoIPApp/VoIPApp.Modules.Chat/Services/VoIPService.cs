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

        public VoIPService(AudioStreamingService audioStreamingService, ServerServiceProxy serverServiceProxy)
        {
            if(audioStreamingService == null)
            {
                throw new ArgumentNullException("audioStreamingService");
            }

            this.audioStreamingService = audioStreamingService;
            this.serverServiceProxy = serverServiceProxy;
        }

        public async Task<bool> StartCallSession(Friend f)
        {
            return await serverServiceProxy.ServerService.CallAsync(f._id);
        }

        public async Task AcceptCall(Friend f)
        {
            audioStreamingService.StartAsync(f.IP, 10000);
            await serverServiceProxy.ServerService.AcceptCallAsync(f._id);
        }

        public async Task CancelCall(Friend f)
        {
            audioStreamingService.StopAsync();
            await serverServiceProxy.ServerService.CancelCallAsync(f._id);
        }
    }
}
