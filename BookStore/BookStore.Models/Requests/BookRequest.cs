﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models.Requests
{
    public class BookRequest
    {
        public int Id { get; set; }
        public int AuthorId { get; init; }
        public string Title { get; init; }
        public DateTime LastUpdated { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
