using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoScavHelperTool.TableObjects
{
    public class DBItemTypes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long id { get; set; }

        public long nGroupID { get; set; }

        public long nSubgroupID { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string strName { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string strDesc { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string strDescAlt { get; set; }

        public long nCondID { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string vImageList { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string vSpriteList { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string vImageUsage { get; set; }

        [Column(TypeName = "real")]
        public double fWeight { get; set; }

        [Column(TypeName = "real")]
        public double fMonetaryValue { get; set; }

        [Column(TypeName = "real")]
        public double fMonetaryValueAlt { get; set; }

        [Column(TypeName = "real")]
        public double fDurability { get; set; }

        [Column(TypeName = "real")]
        public double fDegradePerHour { get; set; }

        [Column(TypeName = "real")]
        public double fEquipDegradePerHour { get; set; }

        [Column(TypeName = "real")]
        public double fDegradePerUse { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string vDegradeTreasureIDs { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string aEquipConditions { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string aPossessConditions { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string aUseConditions { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string aCapacities { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string vEquipSlots { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string vUseSlots { get; set; }

        public long bSocketLocked { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string vProperties { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string aContentIDs { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string nFormatID { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string nTreasureID { get; set; }

        public long nComponentID { get; set; }

        public long bMirrored { get; set; }

        public long nSlotDepth { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string strChargeProfiles { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string aAttackModes { get; set; }

        public long nStackLimit { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string aSwitchIDs { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string aSounds { get; set; }

        public DBItemTypes(object[] item_db_data)
        {
            id = (long)item_db_data[(int)EDBItemTypesTableColumns.eId];
            nGroupID = (long)item_db_data[(int)EDBItemTypesTableColumns.eNGroupID];
            nSubgroupID = (long)item_db_data[(int)EDBItemTypesTableColumns.eNSubgroupID];
            strName = item_db_data[(int)EDBItemTypesTableColumns.eStrName].ToString();
            strDesc = item_db_data[(int)EDBItemTypesTableColumns.eStrDesc].ToString();
            strDescAlt = item_db_data[(int)EDBItemTypesTableColumns.eStrDescAlt].ToString();
            nCondID = (long)item_db_data[(int)EDBItemTypesTableColumns.eNCondID];
            vImageList = item_db_data[(int)EDBItemTypesTableColumns.eVImageList].ToString();
            vSpriteList = item_db_data[(int)EDBItemTypesTableColumns.eVSpriteList].ToString();
            vImageUsage = item_db_data[(int)EDBItemTypesTableColumns.eVImageUsage].ToString();
            fWeight = (double)item_db_data[(int)EDBItemTypesTableColumns.eFWeight];
            fMonetaryValue = (double)item_db_data[(int)EDBItemTypesTableColumns.eFMonetaryValue];
            fMonetaryValueAlt = (double)item_db_data[(int)EDBItemTypesTableColumns.eFMonetaryValueAlt];
            fDurability = (double)item_db_data[(int)EDBItemTypesTableColumns.eFDurability];
            fDegradePerHour = (double)item_db_data[(int)EDBItemTypesTableColumns.eFDegradePerHour];
            fEquipDegradePerHour = (double)item_db_data[(int)EDBItemTypesTableColumns.eFEquipDegradePerHour];
            fDegradePerUse = (double)item_db_data[(int)EDBItemTypesTableColumns.eFDegradePerUse];
            vDegradeTreasureIDs = item_db_data[(int)EDBItemTypesTableColumns.eVDegradeTreasureIDs].ToString();
            aEquipConditions = item_db_data[(int)EDBItemTypesTableColumns.eAEquipConditions].ToString();
            aPossessConditions = item_db_data[(int)EDBItemTypesTableColumns.eAPossessConditions].ToString();
            aUseConditions = item_db_data[(int)EDBItemTypesTableColumns.eAUseConditions].ToString();
            aCapacities = item_db_data[(int)EDBItemTypesTableColumns.eACapacities].ToString();
            vEquipSlots = item_db_data[(int)EDBItemTypesTableColumns.eVEquipSlots].ToString();
            vUseSlots = item_db_data[(int)EDBItemTypesTableColumns.eVUseSlots].ToString();
            bSocketLocked = (long)item_db_data[(int)EDBItemTypesTableColumns.eBSocketLocked];
            vProperties = item_db_data[(int)EDBItemTypesTableColumns.eVProperties].ToString();
            aContentIDs = item_db_data[(int)EDBItemTypesTableColumns.eAContentIDs].ToString();
            nFormatID = item_db_data[(int)EDBItemTypesTableColumns.eNFormatID].ToString();
            nTreasureID = item_db_data[(int)EDBItemTypesTableColumns.eNTreasureID].ToString();
            nComponentID = (long)item_db_data[(int)EDBItemTypesTableColumns.eNComponentID];
            bMirrored = (long)item_db_data[(int)EDBItemTypesTableColumns.eBMirrored];
            nSlotDepth = (long)item_db_data[(int)EDBItemTypesTableColumns.eNSlotDepth];
            strChargeProfiles = item_db_data[(int)EDBItemTypesTableColumns.eStrChargeProfiles].ToString();
            aAttackModes = item_db_data[(int)EDBItemTypesTableColumns.eAAttackModes].ToString();
            nStackLimit = (long)item_db_data[(int)EDBItemTypesTableColumns.eNStackLimit];
            aSwitchIDs = item_db_data[(int)EDBItemTypesTableColumns.eASwitchIDs].ToString();
            aSounds = item_db_data[(int)EDBItemTypesTableColumns.eASounds].ToString();
        }
    }
}
