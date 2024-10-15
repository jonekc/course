using Projekt.Shared.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Projekt.Shared.Models
{
    public class SentQuestionModel
    {
        [Required]
        public int ItemId { get; set; }
        [Required]
        public int QuestionId { get; set; }
        public bool Open { get; set; }
        public List<Answer> Answers { get; set; }
        public int NextItemId { get; set; }
        public int Points { get; set; }
    }
}
