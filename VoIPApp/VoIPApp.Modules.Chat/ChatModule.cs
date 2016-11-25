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
    /// <summary>
    /// will be loaded by the module manager
    /// </summary>
    public class ChatModule : IModule, IDisposable
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
        private readonly IFriendsService chatService;
        /// <summary>
        /// service for managing messaging
        /// </summary>
        private readonly IMessageService messageService;
        /// <summary>
        /// service for voice chatting
        /// </summary>
        private readonly IVoIPService voIPService;
        /// <summary>
        /// manages the wcf service for the <see cref="voIPService"/>
        /// </summary>
        private readonly VoiceServiceManager voiceServiceManager;
        /// <summary>
        /// wcf service for voice chatting
        /// </summary>
        private readonly VoiceService voiceService;

        /// <summary>
        /// creates a new instance of the <see cref="ChatModule"/> class
        /// </summary>
        /// <param name="regionManager">injected by the <see cref="IUnityContainer"/>, stored in <see cref="regionManager"/></param>
        /// <param name="container">injected by the <see cref="IUnityContainer"/>, stored in <see cref="container"/></param>
        /// <param name="audioStreamer">injected by the <see cref="IUnityContainer"/>, stored in <see cref="audioStreamer"/></param>
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

        /// <summary>
        /// registers the types and singleton instance for the <see cref="IUnityContainer"/>, starts the <see cref="audioInitWorker"/>
        /// and the <see cref="voiceServiceManager"/>
        /// </summary>
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

        /// <summary>
        /// initializes the <see cref="audioStreamer"/>
        /// </summary>
        /// <param name="sender">sender of the callback event</param>
        /// <param name="e">do work event args</param>
        private void audioInitWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            audioStreamer.Init();
        }

        /// <summary>
        /// called when the module is disposed, stops the <see cref="voiceServiceManager"/>
        /// </summary>
        public void Dispose()
        {
            voiceServiceManager.StopVoiceService();
        }
    }
}