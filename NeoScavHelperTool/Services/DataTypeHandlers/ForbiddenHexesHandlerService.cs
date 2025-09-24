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
using SQLite;

namespace NeoScavHelperTool.Services.DataTypeHandlers
{
    public class ForbiddenHexesHandlerService
        : NewModBaseHandlerService<ForbiddenHexesHandlerService, ForbiddenHexesItem>
    {
        public const string TABLE = "forbiddenhexes";
        public override string Table => TABLE;

        private const string CREATE_TABLE_SQL =
            @"
            CREATE TABLE IF NOT EXISTS `{0}` (
              `id` INTEGER NOT NULL,
              `nX` INTEGER NOT NULL DEFAULT 0, 
              `nY` INTEGER NOT NULL DEFAULT 0,
              `strName` TEXT NOT NULL,
              `isOverriden` INTEGER NOT NULL,
              `valueModFolder` TEXT NOT NULL,
              PRIMARY KEY(`id`)
            ) WITHOUT ROWID;
            ";
        protected override string CreateTableSql => CREATE_TABLE_SQL;

        private const string UPSERT_ITEM_SQL =
            @"
            INSERT OR REPLACE INTO `{0}_forbiddenhexes` (
              `id`,
              `nX`,
              `nY`,
              `strName`,
              `isOverriden`,
              `valueModFolder`
            ) VALUES (
              @id,
              @nX,
              @nY,
              @strName,
              @isOverriden,
              @valueModFolder
            );
            ";

        public ForbiddenHexesHandlerService(
            ILogger<ForbiddenHexesHandlerService> logger,
            DatabaseService dbService
        )
            : base(logger, dbService) { }

        protected override void AssignColumnValueToItem(
            ref ForbiddenHexesItem item,
            XmlElement column
        )
        {
            string propertyName = column.GetAttribute(XML_COLUMN_NAME_ATTRIBUTE);
            string value = column.InnerText;
            switch (propertyName)
            {
                case "id":
                    item.Id = int.Parse(value);
                    break;
                case "nX":
                    item.X = int.Parse(value);
                    break;
                case "nY":
                    item.Y = int.Parse(value);
                    break;
                case "strName":
                    item.Name = value;
                    break;
                default:
                    throw new Exception(
                        $"Unexpected \"{XML_COLUMN_ELEMENT_NAME}\" \"{XML_COLUMN_NAME_ATTRIBUTE}\" attribute on \"{TABLE}\": \"{propertyName}\":\"{value}\" and file \"{column.BaseURI}\""
                    );
            }
        }

        protected override SQLiteCommand BuildUpsertCommand(
            ModInfo mod,
            ForbiddenHexesItem item,
            Func<string, Dictionary<string, object>, SQLiteCommand> builder
        )
        {
            Dictionary<string, object> values = new Dictionary<string, object>()
            {
                { "@id", item.Id },
                { "@nX", item.X },
                { "@nY", item.Y },
                { "@strName", item.Name },
                { "@isOverriden", item.IsOverriden },
                { "@valueModFolder", item.ValueModFolder },
            };
            return builder.Invoke(string.Format(UPSERT_ITEM_SQL, mod.Name), values);
        }
    }
}
