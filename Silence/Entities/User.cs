using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Silence.Web.Entities
{
    public class User 
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }

        public string Avatar { get; set; }

        public string Salt { get; set; }

        public string PasswordHash { get; set; }

        public string Email { get; set; }   

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        public ICollection<Room> Rooms { get; set; }

        public ICollection<Message> Messages { get; set; }
    }
}
