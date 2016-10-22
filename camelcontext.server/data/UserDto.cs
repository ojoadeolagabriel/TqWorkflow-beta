﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace camelcontext.server.data
{
    public class UserDto
    {
        public UserInstitution InstitutionUser { get; set; }
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public long InstitutionId { get; set; }
        public bool IsEnabled { get; set; }

        public class UserInstitution
        {
            public string InstitutionDescription { get; set; }
        }
    }
}