using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace shoppingWebsite.Models
{
    public class ShowOrderDetail
    {
        public table_Order Order { get; set; }
        public List<table_OrderDetail> OrderDetail { get; set; }

    }
}