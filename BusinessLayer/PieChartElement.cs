using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAccessLayer;
namespace BusinessLayer
{
    public class PieChartElement
    {
        public int ID { get; set; }
        public decimal TotalQuantity { get; set; }
       
        public virtual Stock Stock { get; set; }
    }
}