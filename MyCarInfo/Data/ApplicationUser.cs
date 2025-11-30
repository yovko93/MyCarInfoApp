using Microsoft.AspNetCore.Identity;
using MyCarInfo.Models;

namespace MyCarInfo.Data
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

        public List<Vehicle> Vehicles { get; set; } = new();
    }
}
