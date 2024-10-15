using Projekt.Shared.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Projekt.Shared.Models
{
    public class QuestionModel
    {
        public int QuestionId { get; set; }
        [MaxLength(160), Required]
        public string Description { get; set; }
        public int Points { get; set; }
        public bool Open { get; set; }
        [Required]
        [ValidateComplexType]
        public List<Answer> Answers { get; set; }
        public int ItemId { get; set; }
        public bool IsNew { get; set; }
    }
}