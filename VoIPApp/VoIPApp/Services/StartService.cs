using MongoDB.Bson;
using SharedCode.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoIPApp.Common.Services;

namespace VoIPApp.Services
{
    public class StartService
    {
        private DataBaseService dataBaseService;
        private ServerServiceProxy serverService;

        public StartService(DataBaseService dataBaseService, ServerServiceProxy serverService)
        {
            this.dataBaseService = dataBaseService;
            this.serverService = serverService;
        }

        public async Task Connect()
        {
            await Task.Run(() =>
            {
                Connected = serverService.Connect();

                dataBaseService.Connect();
            });
        }

        public async Task<string> LogIn(string userName, string password)
        {
            bool loggedIn = await serverService.LogIn(userName, password);
            if(!loggedIn)
            {
                return "Passwort oder Benutzername falsch";
            }

            return string.Empty;
        }

        public async Task<string> Register(string userName, string password, string email)
        {
            return await serverService.Register(userName, password, email);
        }

        public static bool Connected { get; set; }
    }
}
