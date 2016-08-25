using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homero.Plugin.Weather
{
    public class WeatherUser
    {
        [Column(Order=2), Key]
        public string Username { get; set; }
        [Column(Order=1), Key]
        public string Server { get; set; }
        [Column(Order = 0), Key]
        public string Client { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string Address { get; set; }
        public bool IsMetric { get; set; }
    }
}
