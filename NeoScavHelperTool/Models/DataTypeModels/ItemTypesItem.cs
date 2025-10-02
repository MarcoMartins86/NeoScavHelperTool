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
        protected override string Identifier => Id.ToString();

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

        [Column("nCondID")]
        public ModRefValue<int> ConditionId { get; set; } = ModRefValue<int>.Of(1);

        [Column("vImageList")]
        public StringList<ModRefValue<string>, string> ImageList { get; set; }

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

        [Column("vProperties")]
        public StringList<ModRefValue<int>, int> Properties { get; set; }

        [Column("aContentIDs")]
        public StringList<ModRefValue<int>, int> ContentIds { get; set; }

        [Column("nFormatID")]
        public ModRefValue<int> FormatId { get; set; } = ModRefValue<int>.Of(3);

        [Column("nTreasureID")]
        public ModRefValue<int> TreasureId { get; set; } = ModRefValue<int>.Of(3);

        [Column("nComponentID")]
        public ModRefValue<int> ComponentId { get; set; } = ModRefValue<int>.Of(3);

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

        [Column("aSounds")]
        public StringList<ModRefValue<string>, string> Sounds { get; set; } =
            StringList<ModRefValue<string>, string>.Of("cuePickup", "cuePutdown");
    }
}
