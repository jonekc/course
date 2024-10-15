using System.ComponentModel.DataAnnotations;

namespace Projekt.Shared.Entities
{
    public class Answer
    {
        public int AnswerId { get; set; }
        [MaxLength(60), Required]
        public string Name { get; set; }
        public bool Correct { get; set; }
    }
}