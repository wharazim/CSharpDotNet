using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcPartialViews.Models
{
    public class Residence
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public Address Address { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}