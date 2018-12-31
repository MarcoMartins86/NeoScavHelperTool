using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoScavHelperTool.Viewer
{
    public class GeneralTreeNode
    {
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        private int _hashCode;
        public int HashCode
        {
            get
            {
                return _hashCode;
            }
            set
            {
                _hashCode = value;
            }
        }

        public GeneralTreeNode(string name)
        {
            _name = name;
            _hashCode = name.GetHashCode();
        }
    }

    public class Tier1TreeNode : GeneralTreeNode
    {        
        private ObservableCollection<Tier2TreeNode> _tier2 = new ObservableCollection<Tier2TreeNode>();
        public ObservableCollection<Tier2TreeNode> Tier2 => _tier2;

        public Tier1TreeNode(string name) : base(name)
        {
        }


    }

    public class Tier2TreeNode : GeneralTreeNode
    {
        private ObservableCollection<ViewerTreeItemDescriptor> _items = new ObservableCollection<ViewerTreeItemDescriptor>();
        public ObservableCollection<ViewerTreeItemDescriptor> Items => _items;

        public Tier2TreeNode(string name) : base(name)
        {
        }
    }
}
