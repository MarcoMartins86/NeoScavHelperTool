using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Logging;

namespace NeoScavHelperTool.Services.DataTypeHandler
{
    public class AtackModesService
    {
        ILogger<AtackModesService> _logger;

        public AtackModesService(ILogger<AtackModesService> logger)
        {
            _logger = logger;
        }

        // public XmlDocument ValidateXml(string modName, string pathXml) { }
    }
}
