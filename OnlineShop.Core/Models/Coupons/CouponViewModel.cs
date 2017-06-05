using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Core.Models.Coupons
{
    public class CouponViewModel
    {
        [DisplayName("Key")]
        public string Id { get; set; }
        public string StrStatus { get; set; }
        public bool Status { get; set; }
        [DisplayName("Start date")]
        [Required]
        public DateTime StartDate { get; set; }
        public string StrStartDate { get { return StartDate.ToShortDateString(); } }
        [DisplayName("End date")]
        [Required]
        public DateTime EndDate { get; set; }
        public string StrEndDate { get { return EndDate.ToShortDateString(); }}
        [Required]
        public Decimal Discount { get; set; }
    }
}
