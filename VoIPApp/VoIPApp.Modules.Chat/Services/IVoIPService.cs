﻿using MongoDB.Bson;
using SharedCode.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoIPApp.Modules.Chat.Services
{
    public interface IVoIPService
    {
        Task StartCallSession(Friend f);
        Task AcceptCall(Friend f);
        Task CancelCall(Friend f);
    }
}
