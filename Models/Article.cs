using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Models
{
    public class Article
    {
        public int Id { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]{1,13}$")]
        public long BarCode { get; set; }
        [Required]
        [MinLength(2, ErrorMessage = "Too short product name")]
        [Display(Name = "product name")]
        [MaxLength(40, ErrorMessage = " Too long name, do not exceed {0}")]
        public string ProductName { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public float Price { get; set; }
        [NotMapped]
        public IFormFile File { get; set; }
        public string FileName { get; set; }
        public int CategoryId {  get; set; }
        public Category Category { get; set; } 
    }
}
