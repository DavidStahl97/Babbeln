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
using VoiceSessionInitializerLibrary;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace VoIPApp.Modules.Chat
{
    public class ChatModule : IModule
    {
        private readonly IRegionManager regionManager;
        private readonly IUnityContainer container;
        private readonly AudioStreamingService audioStreamer;
        private readonly BackgroundWorker audioInitWorker;
        private readonly BackgroundWorker sessionInitWorker;
        private readonly IFriendsService chatService;
        private readonly IMessageService messageService;
        private readonly IVoIPService voIPService;
        private SessionInitializerService sessionInitializerService;

        public ChatModule(IRegionManager regionManager, IUnityContainer container, AudioStreamingService audioStreamer)
        {
            this.regionManager = regionManager;
            this.container = container;

            this.chatService = new FriendsService();
            this.messageService = new MessageService();
            this.voIPService = new VoIPService(audioStreamer);
            this.audioStreamer = audioStreamer;

            this.regionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, typeof(ChatNavigationItemView));

            this.audioInitWorker = new BackgroundWorker();
            audioInitWorker.DoWork += audioInitWorker_DoWork;

            this.sessionInitWorker = new BackgroundWorker();
            sessionInitWorker.DoWork += sessionInitWorker_DoWork;
        }

        public void Initialize()
        {          
            this.container.RegisterInstance(chatService);
            this.container.RegisterInstance(messageService);
            this.container.RegisterInstance(voIPService);
            this.container.RegisterType<VoiceChatViewModel>();
            this.container.RegisterType<object, ChatView>(NavigationURIs.chatViewUri.OriginalString);

            audioInitWorker.RunWorkerAsync();
            sessionInitWorker.RunWorkerAsync();
        }

        private void audioInitWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            audioStreamer.Init();    
        }

        private void sessionInitWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.sessionInitializerService = new SessionInitializerService();

            Uri baseAddress = new Uri("http://localhost:8000/VoIPApp/");
            ServiceHost host = new ServiceHost(sessionInitializerService, baseAddress);

            try
            {
                host.AddServiceEndpoint(typeof(ISessionInitializerService), new WSHttpBinding(), "SessionInitialService");

                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                host.Description.Behaviors.Add(smb);

                host.Open();
            }
            catch (CommunicationException ce)
            {
                Console.WriteLine(ce.ToString());
                host.Abort();
            }
        }
    }
}