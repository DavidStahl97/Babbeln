using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using VoIPApp.Common;
using System;
using CPPWrapper;
using System.ComponentModel;
using VoIPApp.Modules.Chat.Services;
using VoIPApp.Modules.Chat.ViewModels;
using VoIPApp.Modules.Chat.Views;
using System.ServiceModel;
using System.ServiceModel.Description;
using VoiceServiceLibrary;

namespace VoIPApp.Modules.Chat
{
    public class ChatModule : IModule, IDisposable
    {
        private readonly IRegionManager regionManager;
        private readonly IUnityContainer container;
        private readonly AudioStreamingService audioStreamer;
        private readonly BackgroundWorker audioInitWorker;
        private readonly IFriendsService chatService;
        private readonly IMessageService messageService;
        private readonly IVoIPService voIPService;
        private readonly VoiceServiceManager voiceServiceManager;
        private readonly VoiceService voiceService;

        public ChatModule(IRegionManager regionManager, IUnityContainer container, AudioStreamingService audioStreamer)
        {
            this.regionManager = regionManager;
            this.container = container;

            this.chatService = new FriendsService();
            this.messageService = new MessageService();
            this.voIPService = new VoIPService(audioStreamer);
            this.audioStreamer = audioStreamer;

            this.voiceService = new VoiceService();
            this.voiceServiceManager = new VoiceServiceManager(voiceService);

            this.regionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, typeof(ChatNavigationItemView));

            this.audioInitWorker = new BackgroundWorker();
            audioInitWorker.DoWork += audioInitWorker_DoWork;
        }

        public void Initialize()
        {          
            this.container.RegisterInstance(chatService);
            this.container.RegisterInstance(messageService);
            this.container.RegisterInstance(voIPService);
            this.container.RegisterInstance(voiceService);
            this.container.RegisterType<VoiceChatViewModel>();
            this.container.RegisterType<object, ChatView>(NavigationURIs.chatViewUri.OriginalString);

            audioInitWorker.RunWorkerAsync();

            voiceServiceManager.StartVoiceService();
        }

        private void audioInitWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            audioStreamer.Init();
        }

        public void Dispose()
        {
            voiceServiceManager.StopVoiceService();
        }
    }
}