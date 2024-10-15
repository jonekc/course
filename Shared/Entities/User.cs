using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Projekt.Shared.Entities
{
    [Index(nameof(Login), IsUnique = true)]
    public class User
    {
        public int UserId { get; set; }
        [Required]
        [MaxLength(60)]
        public string Login { get; set; }
        [MaxLength(80), Required]
        public string Password { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        public Role Role { get; set; }
        public List<SentItem> FilledItems { get; set; }
    }
}
