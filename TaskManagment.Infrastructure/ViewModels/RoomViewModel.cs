using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Infrastructure.ViewModels
{
    public class RoomViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Admin { get; set; }
    }
}
