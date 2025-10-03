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
using NeoScavHelperTool.Models.ValueObjects;
using NeoScavHelperTool.Services.DataTypeHandlers.Base;
using SQLite;

namespace NeoScavHelperTool.Services.DataTypeHandlers
{
    public class IngredientsHandlerService
        : NewModBaseHandlerService<IngredientsHandlerService, IngredientsItem>
    {
        public const string TABLE = "ingredients";
        public override string Table => TABLE;

        private const string CREATE_TABLE_SQL =
            @"
            CREATE TABLE IF NOT EXISTS `{0}` (
              `nID` INTEGER NOT NULL,
              `strName` TEXT NOT NULL, 
              `strRequiredProps` TEXT NOT NULL DEFAULT '', /* added default value, vanilla is working without this */
              `strForbidProps` TEXT NOT NULL DEFAULT '', /* added default value, vanilla is working without this */
              `isOverriden` INTEGER NOT NULL,
              `valueModFolder` TEXT NOT NULL,
              PRIMARY KEY(`nID`)
            ) WITHOUT ROWID;
            ";
        protected override string CreateTableSql => CREATE_TABLE_SQL;

        private const string UPSERT_ITEM_SQL =
            @"
            INSERT OR REPLACE INTO `{0}_ingredients` (
              `nID`,
              `strName`,
              `strRequiredProps`,
              `strForbidProps`,
              `isOverriden`,
              `valueModFolder`
            ) VALUES (
              @nID,
              @strName,
              @strRequiredProps,
              @strForbidProps,
              @isOverriden,
              @valueModFolder
            );
            ";

        public IngredientsHandlerService(
            ILogger<IngredientsHandlerService> logger,
            DatabaseService dbService
        )
            : base(logger, dbService) { }

        protected override void AssignColumnValueToItem(ref IngredientsItem item, XmlElement column)
        {
            string propertyName = column.GetAttribute(XML_COLUMN_NAME_ATTRIBUTE);
            string value = column.InnerText;
            switch (propertyName)
            {
                case "nID":
                    item.Id = int.Parse(value);
                    break;
                case "strName":
                    item.Name = value;
                    break;
                case "strRequiredProps":
                    item.RequiredProps = StringList<AndModRefValues<int>, int>.Parse(value);
                    break;
                case "strForbidProps":
                    item.ForbidProps = StringList<AndModRefValues<int>, int>.Parse(value);
                    break;
                default:
                    throw new Exception(
                        $"Unexpected \"{XML_COLUMN_ELEMENT_NAME}\" \"{XML_COLUMN_NAME_ATTRIBUTE}\" attribute on \"{TABLE}\": \"{propertyName}\":\"{value}\" and file \"{column.BaseURI}\""
                    );
            }
        }

        protected override SQLiteCommand BuildUpsertCommand(
            ModInfo mod,
            IngredientsItem item,
            Func<string, Dictionary<string, object>, SQLiteCommand> builder
        )
        {
            Dictionary<string, object> values = new Dictionary<string, object>()
            {
                { "@nID", item.Id },
                { "@strName", item.Name },
                { "@strRequiredProps", item.RequiredProps?.ToString() },
                { "@strForbidProps", item.ForbidProps?.ToString() },
                { "@isOverriden", item.IsOverriden },
                { "@valueModFolder", item.ValueModFolder },
            };
            return builder.Invoke(string.Format(UPSERT_ITEM_SQL, mod.Name), values);
        }
    }
}
