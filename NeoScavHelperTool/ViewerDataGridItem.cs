using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoScavModHelperTool
{
    public class ViewerDataGridItem
    {
        private string _name;
        public string Name => _name;
        private object _value;
        public object Value => _value;

        public ViewerDataGridItem(string name, object value)
        {
            _name = name;
            _value = value;
        }
    }
}
