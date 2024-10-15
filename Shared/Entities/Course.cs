using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Projekt.Shared.Entities
{
    public class Course
    {
        public int CourseId { get; set; }
        [MaxLength(60), Required]
        public string Name { get; set; }
        [MaxLength(160)]
        public string Description { get; set; }
        [ValidateComplexType]
        public Category Category { get; set; }
        [ValidateComplexType]
        public List<Item> Items { get; set; }
        [NotMapped]
        public int Points { get; set; }
        [NotMapped]
        public int MaxPoints { get { return Items != null ? Items.Sum(i => i.ContentPoints) + Items.Sum(i => i.Questions != null ? i.Questions.Sum(q => q.Points) : 0) : 0; } }
        [NotMapped]
        public bool Completed { get; set; }
    }
}
