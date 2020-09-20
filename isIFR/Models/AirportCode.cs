using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace isIFR.Models
{
    public class AirportCode
    {
        [Required]
        [MinLength(4, ErrorMessage = "Please enter ICAO 4-character code, e.g. KATL")]
        public string code { get; set; }
    }
}
