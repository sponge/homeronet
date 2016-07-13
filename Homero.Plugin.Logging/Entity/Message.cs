using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homero.Plugin.Logging.Entity
{
    public class Message
    {
        [Column(Order=0), Key]
        public DateTime Timestamp { get; set; }
        [Column(Order=1), Key, Index]
        public string Channel { get; set; }
        [Index]
        public string User { get; set; }

        public string Content { get; set; }

    }
}