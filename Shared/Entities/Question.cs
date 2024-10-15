using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projekt.Shared.Entities
{
    public class Question
    {
        public int QuestionId { get; set; }
        [MaxLength(160), Required]
        public string Description { get; set; }
        public int Points { get; set; }
        public bool Open { get; set; }
        [ValidateComplexType]
        public List<Answer> Answers { get; set; }
        [Required]
        public Item Item { get; set; }
        [NotMapped]
        public bool IsNew { get; set; }
        public List<SentQuestion> SentQuestions { get; set; }
    }
}