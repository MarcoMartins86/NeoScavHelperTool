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
    public class GameVarsHandlerService
        : NewModBaseHandlerService<GameVarsHandlerService, GameVarsItem>
    {
        public const string TABLE = "gamevars";
        public override string Table => TABLE;

        private const string CREATE_TABLE_SQL =
            @"
            CREATE TABLE IF NOT EXISTS `{0}` (
              `strName` TEXT NOT NULL, 
              `strType` TEXT NOT NULL, 
              `strValue` TEXT NOT NULL,
              `isOverriden` INTEGER NOT NULL,
              `valueModFolder` TEXT NOT NULL,
              PRIMARY KEY(`strName`)
            ) WITHOUT ROWID;
            ";
        protected override string CreateTableSql => CREATE_TABLE_SQL;

        private const string UPSERT_ITEM_SQL =
            @"
            INSERT OR REPLACE INTO `{0}_gamevars` (
              `strName`,
              `strType`,
              `strValue`,
              `isOverriden`,
              `valueModFolder`
            ) VALUES (
              @strName,
              @strType,
              @strValue,
              @isOverriden,
              @valueModFolder
            );
            ";

        public GameVarsHandlerService(
            ILogger<GameVarsHandlerService> logger,
            DatabaseService dbService
        )
            : base(logger, dbService) { }

        protected override void AssignColumnValueToItem(ref GameVarsItem item, XmlElement column)
        {
            string propertyName = column.GetAttribute(XML_COLUMN_NAME_ATTRIBUTE);
            string value = column.InnerText;
            switch (propertyName)
            {
                case "strName":
                    item.Name = value;
                    break;
                case "strType":
                    item.Type = value;
                    break;
                case "strValue":
                    item.Value = value;
                    break;
                default:
                    throw new Exception(
                        $"Unexpected \"{XML_COLUMN_ELEMENT_NAME}\" \"{XML_COLUMN_NAME_ATTRIBUTE}\" attribute on \"{TABLE}\": \"{propertyName}\":\"{value}\" and file \"{column.BaseURI}\""
                    );
            }
        }

        protected override SQLiteCommand BuildUpsertCommand(
            ModInfo mod,
            GameVarsItem item,
            Func<string, Dictionary<string, object>, SQLiteCommand> builder
        )
        {
            Dictionary<string, object> values = new Dictionary<string, object>()
            {
                { "@strName", item.Name },
                { "@strType", item.Type },
                { "@strValue", item.Value },
                { "@isOverriden", item.IsOverriden },
                { "@valueModFolder", item.ValueModFolder },
            };
            return builder.Invoke(string.Format(UPSERT_ITEM_SQL, mod.Name), values);
        }
    }
}
