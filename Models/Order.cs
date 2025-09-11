using System;
using System.Collections.Generic;

namespace OnlineStoreMVC.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }

        public User User { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
