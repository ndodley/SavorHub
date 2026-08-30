using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavorHub.Models
{
    public class Review
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        public int MenuItemId { get; set; }
        [ForeignKey("MenuItemId")]
        public MenuItem MenuItem { get; set; }

        [Required]
        [StringLength(1000)]
        public string Content { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        public DateTime Date { get; set; }
    }
}
