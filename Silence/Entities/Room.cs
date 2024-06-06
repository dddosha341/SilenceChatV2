using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Silence.Web.Entities
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public User Admin { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
