using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Silence.Web.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public User FromUser { get; set; }
        public int ToRoomId { get; set; }
        public Room ToRoom { get; set; }
    }
}
