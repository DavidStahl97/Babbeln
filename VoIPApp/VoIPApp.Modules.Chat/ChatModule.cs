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

namespace VoIPApp.Modules.Chat
{
    public class ChatModule : IModule
    {
        private readonly IRegionManager regionManager;
        private readonly IUnityContainer container;
        private readonly AudioStreamingService audioStreamer;
        private readonly BackgroundWorker audioInitWorker;
        private readonly IChatService chatService;

        public ChatModule(IRegionManager regionManager, IUnityContainer container, AudioStreamingService audioStreamer)
        {
            this.regionManager = regionManager;
            this.container = container;

            this.chatService = new ChatService();
            this.audioStreamer = audioStreamer;

            this.regionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, typeof(ChatNavigationItemView));

            this.audioInitWorker = new BackgroundWorker();
            audioInitWorker.DoWork += audioInitWorker_DoWork;
        }

        public void Initialize()
        {          
            this.container.RegisterInstance(chatService);
            this.container.RegisterType<VoiceChatViewModel>();
            this.container.RegisterType<object, ChatView>(NavigationURIs.chatViewUri.OriginalString);

            audioInitWorker.RunWorkerAsync();
        }

        private void audioInitWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            audioStreamer.Init();    
        }
    }
}