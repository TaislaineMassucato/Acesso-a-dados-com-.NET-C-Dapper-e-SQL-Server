using System;
using System.Collections.Generic;

namespace BaltaDataAccess.Models
{
    public class Career
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public IList<CareerItem> Items { get; set; } = new List<CareerItem>();   
    }
}