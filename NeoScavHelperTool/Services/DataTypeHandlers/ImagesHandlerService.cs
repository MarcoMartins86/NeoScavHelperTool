using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Logging;
using NeoScavHelperTool.Attributes;
using NeoScavHelperTool.Models;
using NeoScavHelperTool.Services.DataTypeHandlers.Base;
using SQLite;

namespace NeoScavHelperTool.Services.DataTypeHandlers
{
    public class ImagesHandlerService : DataTypeBaseHandlerService<ImagesHandlerService, ImageInfo>
    {
        private static string IMAGES_FOLDER = "img";

        public const string TABLE = "images";

        public override string Table => TABLE;

        private const string CREATE_TABLE_SQL =
            @"
            CREATE TABLE IF NOT EXISTS `{0}` (
              `name` TEXT NOT NULL,
              `small` TEXT NOT NULL,
              `big` TEXT NOT NULL DEFAULT '',
              `isOverriden` INTEGER NOT NULL,
              `valueModFolder` TEXT NOT NULL,
              PRIMARY KEY(`name`)
            ) WITHOUT ROWID;
            ";

        protected override string CreateTableSql => CREATE_TABLE_SQL;

        private const string UPSERT_ITEM_SQL =
            @"
            INSERT OR REPLACE INTO `{0}_images` (
              `name`,
              `small`,
              `big`,
              `isOverriden`,
              `valueModFolder`
            ) VALUES (
              @name,
              @small,
              @big,
              @isOverriden,
              @valueModFolder
            );
            ";

        public ImagesHandlerService(ILogger<ImagesHandlerService> logger, DatabaseService dbService)
            : base(logger, dbService) { }

        public override void LoadIntoDb(string gamePath, ModInfo mod, DataTypeAttribute _)
        {
            // Create the DB table if needed
            bool willOverride = !CreateTableIfNotExists(mod);

            // Iterate over all images and add into DB
            _dbService.Connection.BeginTransaction();
            foreach (var item in mod.Images)
            {
                ReadItemIntoDb(item, mod, willOverride);
            }
            _dbService.Connection.Commit();

            _logger.LogDebug("\"{mod}\" images were successfuly loaded", mod.Name);
        }

        public override void ReadItemIntoDb(ImageInfo image, ModInfo mod, bool willOverride)
        {
            Dictionary<string, object> values = new Dictionary<string, object>()
            {
                { "@name", image.Name },
                { "@small", image.Files[(int)ImageType.Small] },
                { "@big", image.Files[(int)ImageType.Big] },
                { "@isOverriden", willOverride },
                { "@valueModFolder", mod.Folder },
            };
            SQLiteCommand command = _dbService.Connection.CreateCommand(
                string.Format(UPSERT_ITEM_SQL, mod.Name),
                values
            );
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Failed to insert \"{mod.Name}\" image {image.Name} and message: \"{ex.Message}\""
                );
            }
        }
    }
}
