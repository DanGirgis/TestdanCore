using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TestdanCore.ViewModels
{
    public class GenreFormViewModel
    {
        
        public byte ID { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }
    }
}
