using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace TestdanCore.Models
{
    public class Movie
    {
        public int ID { get; set; }
        [Required,MaxLength(250)]
        public string Name { get; set; }
        public int Year { get; set; }
        public double Rate { get; set; }
        [Required,MaxLength(2500)]
        public string StoreLine { get; set; }
        [Required]
        public byte[] Poster { get; set; }
        public byte GenreId { get; set; }
        public Genre Genre { get; set; }
    }
}
