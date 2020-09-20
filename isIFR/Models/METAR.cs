using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace isIFR.Models
{
    public class METAR
    {
       [Display (Name = "Flight Rules")]
       public string flight_rules { get; set; }

    }
}
