using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoScavHelperTool.Models.DataTypeModels.Base;
using NeoScavHelperTool.Models.ValueObjects;
using SQLite;
using static System.Net.Mime.MediaTypeNames;

namespace NeoScavHelperTool.Models.DataTypeModels
{
    public class ItemTypesItem : DataTypeModelBase
    {
        protected override DataType DataType => DataType.ItemTypes;
        public override string Identifier => Id.ToString();

        [PrimaryKey, Column("id")]
        public int Id { get; set; }

        [Column("nGroupID")]
        public int GroupId { get; set; }

        [Column("nSubgroupID")]
        public int SubgroupId { get; set; }

        [Column("strName")]
        public string Name { get; set; }

        [Column("strDesc")]
        public string Description { get; set; }

        [Column("strDescAlt")]
        public string DescriptionAlternative { get; set; }

        [Ignore]
        public ModRefValue<int> ConditionId { get; set; } = ModRefValue<int>.Of("0", 1);

        [Column("nCondID")]
        public string ConditionIdDb
        {
            get => ConditionId?.ToString();
            set => ConditionId = ModRefValue<int>.Parse(value);
        }

        [Ignore]
        public StringList<ModRefValue<string>, string> ImageList { get; set; }

        [Column("vImageList")]
        public string ImageListDb
        {
            get => ImageList?.ToString();
            set => ImageList = StringList<ModRefValue<string>, string>.Parse(value);
        }

        [Column("vSpriteList")]
        public string SpriteList { get; set; }

        [Column("vImageUsage")]
        public string ImageUsage { get; set; }

        [Column("fWeight")]
        public double Weight { get; set; } = 0;

        [Column("fMonetaryValue")]
        public double MonetaryValue { get; set; } = 0;

        [Column("fMonetaryValueAlt")]
        public double MonetaryValueAlternative { get; set; } = 0;

        [Column("fDurability")]
        public double Durability { get; set; } = 1;

        [Column("fDegradePerHour")]
        public double DegradePerHour { get; set; } = 0;

        [Column("fEquipDegradePerHour")]
        public double EquipDegradePerHour { get; set; } = 0;

        [Column("fDegradePerUse")]
        public double DegradePerUse { get; set; } = 0;

        [Column("vDegradeTreasureIDs")]
        public string DegradeTreasureIds { get; set; } = "3,3";

        [Column("aEquipConditions")]
        public string EquipConditions { get; set; }

        [Column("aPossessConditions")]
        public string PossessConditions { get; set; }

        [Column("aUseConditions")]
        public string UseConditions { get; set; }

        [Column("aCapacities")]
        public string Capacities { get; set; }

        [Column("vEquipSlots")]
        public string EquipSlots { get; set; }

        [Column("vUseSlots")]
        public string UseSlots { get; set; }

        [Column("bSocketLocked")]
        public bool SocketLocked { get; set; } = false;

        [Ignore]
        public StringList<ModRefValue<int>, int> Properties { get; set; }

        [Column("vProperties")]
        public string PropertiesDb
        {
            get => Properties?.ToString();
            set => Properties = StringList<ModRefValue<int>, int>.Parse(value);
        }

        [Ignore]
        public StringList<ModRefValue<int>, int> ContentIds { get; set; }

        [Column("aContentIDs")]
        public string ContentIdsDb
        {
            get => ContentIds?.ToString();
            set => ContentIds = StringList<ModRefValue<int>, int>.Parse(value);
        }

        [Ignore]
        public ModRefValue<int> FormatId { get; set; } = ModRefValue<int>.Of("0", 3);

        [Column("nFormatID")]
        public string FormatIdDb
        {
            get => FormatId?.ToString();
            set => FormatId = ModRefValue<int>.Parse(value);
        }

        [Ignore]
        public ModRefValue<int> TreasureId { get; set; } = ModRefValue<int>.Of("0", 3);

        [Column("nTreasureID")]
        public string TreasureIdDb
        {
            get => TreasureId?.ToString();
            set => TreasureId = ModRefValue<int>.Parse(value);
        }

        [Ignore]
        public ModRefValue<int> ComponentId { get; set; } = ModRefValue<int>.Of("0", 3);

        [Column("nComponentID")]
        public string ComponentIdDb
        {
            get => ComponentId?.ToString();
            set => ComponentId = ModRefValue<int>.Parse(value);
        }

        [Column("bMirrored")]
        public bool Mirrored { get; set; } = false;

        [Column("nSlotDepth")]
        public int SlotDepth { get; set; } = 0;

        [Column("strChargeProfiles")]
        public string ChargeProfiles { get; set; }

        [Column("aAttackModes")]
        public string AttackModes { get; set; }

        [Column("nStackLimit")]
        public int StackLimit { get; set; } = 1;

        [Column("aSwitchIDs")]
        public string SwitchIds { get; set; } = "";

        [Ignore]
        public StringList<ModRefValue<string>, string> Sounds { get; set; } =
            StringList<ModRefValue<string>, string>.Of("0", "cuePickup", "cuePutdown");

        [Column("aSounds")]
        public string SoundsDb
        {
            get => Sounds?.ToString();
            set => Sounds = StringList<ModRefValue<string>, string>.Parse(value);
        }
    }
}
