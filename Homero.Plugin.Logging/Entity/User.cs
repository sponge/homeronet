﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homero.Plugin.Logging.Entity
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Index]
        public string Name { get; set; }
    }
}