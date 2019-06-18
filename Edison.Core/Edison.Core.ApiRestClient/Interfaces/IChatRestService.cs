﻿using Edison.Core.Common.Models;
using System.Threading.Tasks;

namespace Edison.Core.Interfaces
{
    public interface IChatRestService
    {
        Task<ChatUserTokenContext> GetToken();
    }
}
