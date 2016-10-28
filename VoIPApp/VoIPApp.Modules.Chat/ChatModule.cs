using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using VoIPApp.Common;
using System;
using CPPWrapper;
using System.ComponentModel;
using VoIPApp.Modules.Chat.Services;
using VoIPApp.Modules.Chat.ViewModels;

namespace VoIPApp.Modules.Chat
{
    public class ChatModule : IModule
    {
        private readonly IRegionManager regionManager;
        private readonly IUnityContainer container;
        private readonly BackgroundWorker audioInitWorker;
        private readonly AudioStreamingService audioStreamer;
        private readonly IChatService chatService;

        public ChatModule(RegionManager regionManager, IUnityContainer container)
        {
            this.regionManager = regionManager;
            this.container = container;

            this.audioStreamer = new AudioStreamingService();
            this.chatService = new ChatService();

            this.audioInitWorker = new BackgroundWorker();
            audioInitWorker.DoWork += audioInitWorker_DoWork;
        }

        public void Initialize()
        {
            this.container.RegisterInstance(audioStreamer);
            this.container.RegisterInstance(chatService);
            this.container.RegisterType<VoiceChatViewModel>();

            this.regionManager.RegisterViewWithRegion(RegionNames.ChatRegion, () => this.container.Resolve<Views.ChatView>());
            
            audioInitWorker.RunWorkerAsync();
        }

        private void audioInitWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            audioStreamer.Init();    
        }
    }
}