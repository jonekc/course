using System;
using System.ComponentModel.DataAnnotations;

namespace Projekt.Shared.Models
{
    public class ItemModel
    {
        public int ItemId { get; set; }
        [MaxLength(60), Required(ErrorMessage = "Wpisz tytuł")]
        public string Name { get; set; }
        [MaxLength(160)]
        public string Description { get; set; }
        public string Content { get; set; }
        public int ContentPoints { get; set; }
        public int QuestionsPoints { get; set; }
        public int QuestionsCount { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int PrevItemId { get; set; }
        public int NextItemId { get; set; }
    }
}
