using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UTW_Project.Models;
namespace UTW_Project.Classes
{
    public class PieChartElement
    {
        public int ID { get; set; }
        public decimal TotalQuantity { get; set; }
       
        public virtual Stock Stock { get; set; }
    }
}