using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace NeoScavHelperTool.Helpers
{
    public class LazierHelper<T> : Lazy<T>
        where T : class
    {
        public LazierHelper(IServiceProvider provider)
            : base(provider.GetRequiredService<T>) { }
    }
}
