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

        [RegularExpression("^.+NEOScavenger\\.exe$", ErrorMessage = "Unexpected value")]
        public string NeoScavExePath { get; set; } = string.Empty;

        [RegularExpression(
            "^file$|^memory$",
            ErrorMessage = "SQLite value can only be 'file' or 'memory'"
        )]
        public string SQLite { get; set; } = string.Empty;
    }
}
