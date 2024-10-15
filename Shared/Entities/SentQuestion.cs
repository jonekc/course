using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Projekt.Shared.Entities
{
    public class SentQuestion
    {
        public int SentQuestionId { get; set; }
        [Required]
        public Question Question { get; set; }
        public List<Answer> Answers { get; set; }
        [Required]
        public SentItem SentItem { get; set; }
    }
}
