using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Projekt.Shared.Entities
{
    public class SentItem
    {
        public int SentItemId { get; set; }
        [Required]
        public Item Item { get; set; }
        [Required]
        public User User { get; set; }
        public List<SentQuestion> SentQuestions { get; set; }
        public DateTime SentDate { get; set; }
    }
}