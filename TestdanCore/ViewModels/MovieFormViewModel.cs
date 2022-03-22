using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TestdanCore.Models;

namespace TestdanCore.ViewModels
{
    public class MovieFormViewModel
    {
        public int Id { get; set; }
        [Required, StringLength(250)]
        [Display(Name ="Title")]
        public string Name { get; set; }
        public int Year { get; set; }
        [Range(1,10)]
        public double Rate { get; set; }
        [Required, StringLength(2500)]
        public string StoreLine { get; set; }
        [Display(Name ="select Poster.....")]

        public byte[] Poster { get; set; }
        [Display(Name ="Genre")]
        public byte GenreId { get; set; }
        public IEnumerable<Genre> Genres { get; set; }
    }
}
