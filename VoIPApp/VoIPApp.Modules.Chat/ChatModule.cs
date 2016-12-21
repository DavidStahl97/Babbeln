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
using VoIPApp.Common.Services;

namespace VoIPApp.Modules.Chat
{
    /// <summary>
    /// will be loaded by the module manager
    /// </summary>
    public class ChatModule : IModule
    {
        /// <summary>
        /// <see cref="IRegionManager"/> of the application
        /// </summary>
        private readonly IRegionManager regionManager;
        /// <summary>
        /// <see cref="IUnityContainer"/> of the application
        /// </summary>
        private readonly IUnityContainer container;
        /// <summary>
        /// <see cref="AudioStreamingService"/> that will be used to voice chat
        /// </summary>
        private readonly AudioStreamingService audioStreamer;
        /// <summary>
        /// <see cref="BackgroundWorker"/> for initializing the <see cref="audioStreamer"/> asynchronously
        /// </summary>
        private readonly BackgroundWorker audioInitWorker;
        /// <summary>
        /// service for managing the friend list
        /// </summary>
        private readonly IFriendsService friendsService;
        /// <summary>
        /// service for managing messaging
        /// </summary>
        private readonly IMessageService messageService;
        /// <summary>
        /// service for voice chatting
        /// </summary>
        private readonly IVoIPService voIPService;

        /// <summary>
        /// creates a new instance of the <see cref="ChatModule"/> class
        /// </summary>
        /// <param name="regionManager">injected by the <see cref="IUnityContainer"/>, stored in <see cref="regionManager"/></param>
        /// <param name="container">injected by the <see cref="IUnityContainer"/>, stored in <see cref="container"/></param>
        /// <param name="audioStreamer">injected by the <see cref="IUnityContainer"/>, stored in <see cref="audioStreamer"/></param>
        public ChatModule(IRegionManager regionManager, IUnityContainer container, ModuleManager moduleManager, AudioStreamingService audioStreamer, ServerServiceProxy serverServiceProxy)
        {
            this.regionManager = regionManager;
            this.container = container;

            this.friendsService = new FriendsService(container);
            this.messageService = new MessageService(container, moduleManager, serverServiceProxy);
            this.voIPService = new VoIPService(audioStreamer, serverServiceProxy);
            this.audioStreamer = audioStreamer;

            this.regionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, typeof(ChatNavigationItemView));

            this.audioInitWorker = new BackgroundWorker();
            audioInitWorker.DoWork += audioInitWorker_DoWork;

            moduleManager.LoadModuleCompleted += (s, e) =>
            {
                audioInitWorker.RunWorkerAsync();
            };
        }

        /// <summary>
        /// registers the types and singleton instance for the <see cref="IUnityContainer"/>, starts the <see cref="audioInitWorker"/>
        /// and the <see cref="voiceServiceManager"/>
        /// </summary>
        public void Initialize()
        {          
            this.container.RegisterInstance(friendsService);
            this.container.RegisterInstance(messageService);
            this.container.RegisterInstance(voIPService);
            this.container.RegisterType<VoiceChatViewModel>();
            this.container.RegisterType<object, ChatView>(NavigationURIs.chatViewUri.OriginalString);
        }

        /// <summary>
        /// initializes the <see cref="audioStreamer"/>
        /// </summary>
        /// <param name="sender">sender of the callback event</param>
        /// <param name="e">do work event args</param>
        private void audioInitWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            audioStreamer.Init();
        }

    }
}