using System.ComponentModel.DataAnnotations;

namespace SmartCartCarbonFootprintApi.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
   
        public ICollection<UserRole> UserRoles { get; set; } 

        public virtual ICollection<User>Users { get; set; }
    }
}
