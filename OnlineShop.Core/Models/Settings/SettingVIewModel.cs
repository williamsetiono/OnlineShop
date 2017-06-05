using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace OnlineShop.Core.Models.Settings
{
    public class SettingVIewModel
    {
        [DisplayName("Setting Key")]
        [Required]
        public string Id { get; set; }
        [Required]
        public string Value { get; set; }
        [DisplayName("Type")]
        public string Type { get; set; } 
    }
}
