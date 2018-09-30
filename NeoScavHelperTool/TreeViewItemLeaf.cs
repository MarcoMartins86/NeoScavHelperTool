using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoScavModHelperTool
{
    public class TreeViewItemLeaf
    {
        public enum DataType
        {
            eAttackmodes,
            eBarterhexes,
            eBattlemoves,
            eCamptypes,
            eChargeprofiles,
            eConditions,
            eContainertypes,
            eCreatures,
            eCreaturesources,
            eDatafiles,
            eDmcplaces,
            eEcounters,
            eEncountertriggers,
            eFactions,
            eForbiddenhexes,
            eGamevars,
            eHeadlines,
            eHextypes,
            eImages,
            eIngredients,
            eItemprops,
            eItemtypes,
            eMaps,
            eRecipes,
            eTreasuretable
        }

        private string _primaryKeyName;
        public string PrimaryKeyName => _primaryKeyName;        
        private string _description;
        private bool _isDescriptionPrimaryKey;
        private string _primaryKeyValue;
        public string PrimaryKeyValue => _isDescriptionPrimaryKey ? _description : _primaryKeyValue;
        private DataType _type;
        public DataType Type => _type;
        private string _tableName;
        public string TableName => _tableName;

        public TreeViewItemLeaf(string first, string second, string db_primary_key_name, bool is_second_primary_key, DataType type, string table_name)
        {
            _primaryKeyName = db_primary_key_name;
            _primaryKeyValue = first;
            _description = second;
            _isDescriptionPrimaryKey = is_second_primary_key;
            _type = type;
            _tableName = table_name;
        }

        public override string ToString()
        {
            if (_isDescriptionPrimaryKey)
                return _description;

            return string.IsNullOrEmpty(_description) ? _primaryKeyValue : _primaryKeyValue + "_" + _description;
        }
    }
}
