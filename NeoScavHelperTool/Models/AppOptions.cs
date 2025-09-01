using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoScavHelperTool.Models
{
    public class AppOptions
    {
        public const string Section = "App";

        [Required(AllowEmptyStrings = false)]
        public string NeoScavExePath { get; set; } = string.Empty;
    }
}
