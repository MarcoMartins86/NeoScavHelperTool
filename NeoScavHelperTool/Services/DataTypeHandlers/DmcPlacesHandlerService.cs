using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using System.Xml;
using Microsoft.Extensions.Logging;
using NeoScavHelperTool.Attributes;
using NeoScavHelperTool.Models;
using NeoScavHelperTool.Models.DataTypeModels;
using NeoScavHelperTool.Services.DataTypeHandlers.Base;
using SQLite;

namespace NeoScavHelperTool.Services.DataTypeHandlers
{
    public class DmcPlacesHandlerService
        : NewModBaseHandlerService<DmcPlacesHandlerService, DmcPlacesItem>
    {
        public const string TABLE = "dmcplaces";
        public override string Table => TABLE;

        private const string CREATE_TABLE_SQL =
            @"
            CREATE TABLE IF NOT EXISTS `{0}` (
              `id` INTEGER NOT NULL,
              `strImg` TEXT NOT NULL,
              `nEncounterID` INTEGER NOT NULL DEFAULT 1,
              `nX` INTEGER NOT NULL DEFAULT 0,
              `nY` INTEGER NOT NULL DEFAULT 0, 
              `isOverriden` INTEGER NOT NULL,
              `valueModFolder` TEXT NOT NULL,
              PRIMARY KEY(`id`)
            ) WITHOUT ROWID;
            ";
        protected override string CreateTableSql => CREATE_TABLE_SQL;

        private const string UPSERT_ITEM_SQL =
            @"
            INSERT OR REPLACE INTO `{0}_dmcplaces` (
              `id`,
              `strImg`,
              `nEncounterID`,
              `nX`,
              `nY`,
              `isOverriden`,
              `valueModFolder`
            ) VALUES (
              @id,
              @strImg,
              @nEncounterID,
              @nX,
              @nY,
              @isOverriden,
              @valueModFolder
            );
            ";

        public DmcPlacesHandlerService(
            ILogger<DmcPlacesHandlerService> logger,
            DatabaseService dbService,
            ModsMetadataService modsMetadataService
        )
            : base(logger, dbService, modsMetadataService) { }

        protected override void AssignColumnValueToItem(ref DmcPlacesItem item, XmlElement column)
        {
            string propertyName = column.GetAttribute(XML_COLUMN_NAME_ATTRIBUTE);
            string value = column.InnerText;
            switch (propertyName)
            {
                case "id":
                    item.Id = int.Parse(value);
                    break;
                case "strImg":
                    item.Image = value;
                    break;
                case "nEncounterID":
                    item.EncounterId = int.Parse(value);
                    break;
                case "nX":
                    item.X = int.Parse(value);
                    break;
                case "nY":
                    item.Y = int.Parse(value);
                    break;
                default:
                    throw new Exception(
                        $"Unexpected \"{XML_COLUMN_ELEMENT_NAME}\" \"{XML_COLUMN_NAME_ATTRIBUTE}\" attribute on \"{TABLE}\": \"{propertyName}\":\"{value}\" and file \"{column.BaseURI}\""
                    );
            }
        }

        protected override SQLiteCommand BuildUpsertCommand(
            ModInfo mod,
            DmcPlacesItem item,
            Func<string, Dictionary<string, object>, SQLiteCommand> builder
        )
        {
            Dictionary<string, object> values = new Dictionary<string, object>()
            {
                { "@id", item.Id },
                { "@strImg", item.Image },
                { "@nEncounterID", item.EncounterId },
                { "@nX", item.X },
                { "@nY", item.Y },
                { "@isOverriden", item.IsOverriden },
                { "@valueModFolder", item.ValueModFolder },
            };
            return builder.Invoke(string.Format(UPSERT_ITEM_SQL, mod.Name), values);
        }
    }
}
