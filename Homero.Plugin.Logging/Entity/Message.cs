using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Homero.Plugin.Logging.Entity
{
    public class Message
    {
        [Column(Order = 0), Key]
        public DateTime Timestamp { get; set; }

        [Column(Order = 1), Key, Index]
        public string Channel { get; set; }

        [Index]
        public string User { get; set; }

        public string Content { get; set; }
    }
}