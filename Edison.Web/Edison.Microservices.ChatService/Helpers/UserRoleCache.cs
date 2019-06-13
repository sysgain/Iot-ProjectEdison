﻿using System.Collections.Generic;
using Edison.Core.Common.Models;

namespace Edison.ChatService.Helpers
{
    public static class UserRoleCache
    {
        private static Dictionary<string, ChatUserRole> _userRoles;

        public static Dictionary<string, ChatUserRole> UserRoles {
            get
            {
                if (_userRoles == null)
                    _userRoles = new Dictionary<string, ChatUserRole>();
                return _userRoles;
            }
        }
    }
}
