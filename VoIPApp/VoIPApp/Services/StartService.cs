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

        public async Task<string> Connect()
        {
            return await Task.Run(() =>
            {
                bool connected = serverService.Connect();
                if (!connected)
                {
                    return "Verbindung mit dem Server fehlgeschlagen";
                }

                dataBaseService.Connect();

                return null;
            });
        }

        public async Task<string> LogIn(string userName, string password)
        {

            bool loggedIn = await serverService.LogIn(userName, password);
            if(!loggedIn)
            {
                return "Passwort oder Benutzername falsch";
            }

            return null;
        }

        public async Task<string> Register(string userName, string password, string email)
        {
            return await serverService.Register(userName, password, email);
        }
    }
}
