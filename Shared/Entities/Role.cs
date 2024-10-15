using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Projekt.Shared.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class Role
    {
        public int RoleId { get; set; }
        [StringLength(50), Required]
        public string Name { get; set; }
    }
}