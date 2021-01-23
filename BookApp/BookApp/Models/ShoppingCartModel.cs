using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookApp.Models
{
    public class ShoppingCartModel
    {
        public string ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int Total { get; set; }
        public string ImagePath { get; set; }
        public string ItemName { get; set; }
    }
}