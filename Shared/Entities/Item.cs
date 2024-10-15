using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projekt.Shared.Entities
{
    public class Item
    {
        public int ItemId { get; set; }
        [MaxLength(60), Required]
        public string Name { get; set; }
        [MaxLength(160)]
        public string Description { get; set; }
        public string Content { get; set; }
        public int ContentPoints { get; set; }
        public List<Question> Questions { get; set; }
        [Required]
        public Course Course { get; set; }
        public DateTime UpdatedDate { get; set; }
        public List<SentItem> FilledItems { get; set; }
        [NotMapped]
        public int PrevItemId { get; set; }
        [NotMapped]
        public int NextItemId { get; set; }
    }
}