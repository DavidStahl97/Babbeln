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
    public class LoginService
    {
        private DataBaseService dataBaseService;
        private ServerServiceProxy serverService;

        public LoginService(DataBaseService dataBaseService, ServerServiceProxy serverService)
        {
            this.dataBaseService = dataBaseService;
            this.serverService = serverService;
        }

        public async Task<string> LogIn(string userName, string password)
        {
            bool connected = serverService.Connect();
            if(!connected)
            {
                return "Verbindung mit dem Server fehlgeschlagen";
            }

            bool loggedIn = await serverService.LogIn(userName, password);
            if(!loggedIn)
            {
                return "Passwort oder Benutzername falsch";
            }

            dataBaseService.Connect();
            return null;
        } 
    }
}
