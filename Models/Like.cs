﻿using JSON_DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhotoManager.Models
{
    public class Like : Record
    {
        public int UserId { get; set; }
        public int PhotoId { get; set; }
    }
}