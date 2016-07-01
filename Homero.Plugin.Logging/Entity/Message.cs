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
        [Key]
        public int Id { get; set; }

        public Server Server { get; set; }

        [Index]
        public Channel Channel { get; set; }

        [Index]
        public Client Client { get; set; }

        public User User { get; set; }

        public string Content { get; set; }

        public DateTime Timestamp { get; set; }
    }
}