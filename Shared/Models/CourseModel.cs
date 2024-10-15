using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Projekt.Shared.Models
{
    public class CourseModel
    {
        public int CourseId { get; set; }
        [MaxLength(60), Required(ErrorMessage = "Wpisz nazwę")]
        public string Name { get; set; }
        [MaxLength(160)]
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        [ValidateComplexType]
        public List<ItemModel> Items { get; set; }
        public int Points { get; set; }
        public int MaxPoints { get; set; }
        public bool Completed { get; set; }
    }
}
