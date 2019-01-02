using NeoScavHelperTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoScavHelperTool
{
    public class ComboBoxEquipmentItem
    {
        private string _name;
        public string Name => _name;

        private string _sprite;
        public string Sprite => _sprite;

        private string _tableName;
        public string TableName => _tableName;

        private int _bodyPart;
        public int BodyPart => _bodyPart;

        public ComboBoxEquipmentItem(string name, string sprite, string table, int body_part)
        {
            _sprite = sprite;
            _tableName = table;
            App.GetFinalItemAndTableFromEncapsulatedItemAndTableWithSufix(ref _sprite,ref _tableName, "images");
            _name = name;
            _sprite = Path.GetFileNameWithoutExtension(sprite);
            _bodyPart = body_part;
        }

        public override string ToString()
        {
            return _name;
        }
        
        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(ComboBoxEquipmentItem))
            {
                ComboBoxEquipmentItem other = (ComboBoxEquipmentItem)obj;
                return (string.Compare(_name, other.Name) == 0) && (string.Compare(_sprite, other.Sprite) == 0) 
                    && (string.Compare(_tableName, other.TableName) == 0) && _bodyPart == other.BodyPart;
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return (_name + "|" + _sprite + "|" + _tableName + "|" + _bodyPart).GetHashCode();
        }
    }
}
