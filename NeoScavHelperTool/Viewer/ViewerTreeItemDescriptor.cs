﻿using NeoScavHelperTool;
using NeoScavHelperTool.DBTableAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoScavHelperTool.Viewer
{
    public class ViewerTreeItemDescriptor
    {
        private EDBTable _type;
        public EDBTable Type => _type;
        private int _modIndex;
        public int ModIndex => _modIndex;
        private string _primaryKeyName;
        public string PrimaryKeyName => _primaryKeyName;
        private string _description;
        private string _primaryKeyValue;
        public string PrimaryKeyValue => _primaryKeyValue;
        private int[] _treeIndexByType;
        public int[] TreeIndexByType => _treeIndexByType;
        private int[] _treeIndexByMod;
        public int[] TreeIndexByMod => _treeIndexByMod;
        private string _tableName;
        public string TableName => _tableName;
        private string _treeText;
        public string TreeText
        {
            get
            {
                return _treeText;
            }
            set
            {
                _treeText = value;
            }
        }

        public ViewerTreeItemDescriptor(object [] columns_values, EDBTable type, int n_mod_index, int [] tree_index_type, int[] tree_index_mod, string table_name)
        {
            _primaryKeyValue = columns_values[0].ToString();
            _primaryKeyName = DBTableAttributtesFetcher.GetPrimaryKeyName(type);

            switch(columns_values.Length)
            {
                case 1:
                    _treeText = _primaryKeyValue;
                    _description = string.Empty;
                    break;
                case 2: // Most of them
                    _description = columns_values[1].ToString();
                    _treeText = string.Format("{0}_{1}", _primaryKeyValue, _description);
                    break;
                case 3: // BarterHexes
                    _description = string.Format("({0}, {1})", columns_values[1], columns_values[2]);
                    _treeText = string.Format("{0}_{1}", _primaryKeyValue, _description);
                    break;
                case 4: // ForbiddenHexes
                    _description = string.Format("({0}, {1})_{2}", columns_values[1], columns_values[2], columns_values[3]);
                    _treeText = string.Format("{0}_{1}", _primaryKeyValue, _description);
                    break;
            }

            _type = type;
            _modIndex = n_mod_index;

            _treeIndexByType = tree_index_type;
            _treeIndexByMod = tree_index_mod;

            _tableName = table_name;
        }
    }
}
