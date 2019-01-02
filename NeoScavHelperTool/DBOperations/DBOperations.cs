using NeoScavHelperTool.DBTableAttributes;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace NeoScavHelperTool
{
    public class DBOperations
    {
        private static string _sqlCommandSelectAllTableData = "SELECT * FROM `{0}`";
        private static string _sqlCommandSelectAllTableDataOfSpecificColumns = "SELECT {0} FROM `{1}`";
        private static string _sqlCommandSelectAllFromTableWhereColumnEqualValue = "SELECT * FROM `{0}` WHERE `{1}`=@value";
        private static string _sqlCommandSelectColumnFromTableWhereColumnEqualValue = "SELECT `{0}` FROM `{1}` WHERE `{2}`=@value";
        private static string _sqlCommandSelectColumnFromTableWhereMultipleColumnEqualValue = "SELECT `{0}` FROM `{1}` WHERE @multiple";
        private static string _sqlCommandSelectColumnsFromTableWhereMultipleColumnEqualValue = "SELECT {0} FROM `{1}` WHERE @multiple";
        private static string _sqlCommandSelectColumnsFromTableWhereColumnEqualValue = "SELECT {0} FROM `{1}` WHERE `{2}`=@value";
        private static string _sqlCommandSelectTablesWithNameLike = "SELECT `name` FROM `sqlite_master` WHERE `type`='table' and `name` like @name;";
        private static string _sqlCommandSelectValueFromConfiguration = "SELECT `value` FROM `configuration` WHERE `name`=@name";
        private static string _sqlCommandInsertOrReplaceIntoConfiguration = "INSERT OR REPLACE INTO `configuration` (`name`, `value`) VALUES(@name,@value);";
        private static string _sqlCommandInsertOrReplaceIntoAnyTable = "INSERT OR REPLACE INTO `{0}` ({1}) VALUES(@values);";
        private static string _sqlCommandInsertOrReplaceIntoFileHash = "INSERT OR REPLACE INTO `file_hash` (`file`, `hash`) values (@file, @hash);";
        private static string _sqlCommandPragmaTableInfo = "PRAGMA table_info(`{0}`)";

        private SQLiteConnection _dbFsConnection;
        public SQLiteConnection DbFsConnection => _dbFsConnection;
        private SQLiteConnection _dbMemConnection;
        public SQLiteConnection DbMemConnection => _dbMemConnection;

        public void Init()
        {
            App._splashScreen.UpdateMessage("Initializing database");           
            
            //1st - Create the needed folder and db file on appdata if it doesn't exist already
            var dbFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NeoScavHelperTool", "db.sqlite");

            if (File.Exists(dbFilename) == false)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dbFilename));
                SQLiteConnection.CreateFile(dbFilename);
            }

            _dbFsConnection = new SQLiteConnection(connectionString: "Data Source=" + dbFilename + ";Version=3;Compress=True;UTF8Encoding=True;");
            DbFsConnection.Open();

            //2nd - try to create file hash table if not exists
            SQLiteCommand command = new SQLiteCommand(DbFsConnection);
            command.CommandText = "CREATE TABLE IF NOT EXISTS `file_hash` (`file` TEXT NOT NULL, `hash` BLOB NOT NULL, PRIMARY KEY(`file`)) WITHOUT ROWID;";
            command.ExecuteNonQuery();
            //3rd - try to create configuration table if not exists
            command.Reset();
            command.CommandText = "CREATE TABLE IF NOT EXISTS `configuration` (`name` TEXT NOT NULL, `value` TEXT NOT NULL, PRIMARY KEY(`name`)) WITHOUT ROWID;";
            command.ExecuteNonQuery();
            //4th - check if we have the game directory in the configuration table and if try with known dirs if not ask it to the user
            App._splashScreen.UpdateMessage("Checking for game folder");
            command.Reset();
            command.CommandText = _sqlCommandSelectValueFromConfiguration;
            command.Parameters.Add(new SQLiteParameter("@name", "GameFolder"));
            SQLiteDataReader reader = command.ExecuteReader();
            string strGameFolder = string.Empty;
            if (reader.HasRows && reader.Read())
            {
                strGameFolder = reader.GetString(0);
                reader.Close();
                //Let's just do a quick check to see if it is still valid
                if (File.Exists(Path.Combine(strGameFolder, "NEOScavenger.exe")) == false)
                    strGameFolder = string.Empty; //clear the string since game folder changed
            }
            //check if we need to find the game folder
            if (string.IsNullOrEmpty(strGameFolder))
            {
                //try on steam folders, first on Program Files x86
                strGameFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                    "Steam", "steamapps", "common", "NEO Scavenger");
                if (File.Exists(Path.Combine(strGameFolder, "NEOScavenger.exe")) == false) //if not found try on Program Files
                {
                    strGameFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    "Steam", "steamapps", "common", "NEO Scavenger");
                    if (File.Exists(Path.Combine(strGameFolder, "NEOScavenger.exe")) == false) //if not ask it to the user
                    {
                        strGameFolder = App._splashScreen.AskNeoScavengerGameFolder();
                        if (string.IsNullOrEmpty(strGameFolder))
                        {
                            App.I.DisplayMessageDialog("Choosen directory is not the game folder. Application will now exit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning, true);
                            return;
                        }
                    }
                }
                //either way if it reaches here we can save the value to the database
                command.Reset();
                command.CommandText = _sqlCommandInsertOrReplaceIntoConfiguration;
                command.Parameters.Add(new SQLiteParameter("@name", "GameFolder"));
                command.Parameters.Add(new SQLiteParameter("@value", strGameFolder));
                command.ExecuteNonQuery();
                command.Reset();
            }
            //5th - start parsing vanilla game xml, checking for changes first comparing previous parse hash
            App._splashScreen.UpdateMessage("Parsing vanilla game files into database");
            ParseType0(strGameFolder, "0", "vanilla");
            //6th - start parsing mods xml, checking for changes first comparing previous parse hash
            ParseGetModsFile(strGameFolder);
            //7th - Create a list of simplified mods prefix ordered by loading order
            CreateModsPrefixList();
            //8th - Finally create an in-memory DB with the values as the game see them
            CreateInMemGameDatabase();            
        }

        public void Close()
        {
            if (DbFsConnection != null)
                DbFsConnection.Close();
            if (DbMemConnection != null)
                DbMemConnection.Close();
        }

        private void ParseElementIntoDB(string str_file, XmlReader reader, string str_table_name_prefix, string str_table_name_sufix, SQLiteTransaction sqlTransaction)
        {
            SQLiteCommand command = new SQLiteCommand(_sqlCommandInsertOrReplaceIntoAnyTable, DbFsConnection, sqlTransaction);
            bool bElementStarted = false;
            bool bEndedTableElement = false;
            string strParameterName = string.Empty;
            List<string> listParameterNames = new List<string>();
            List<string> listParameterValues = new List<string>();
            while (reader.Read())
            {
                if ((reader.NodeType == XmlNodeType.Element) &&
                    ((string.Compare(reader.Name.ToLower(), "column") == 0)))
                {
                    if (!bElementStarted) //continue only if no element was started
                    {
                        strParameterName = reader.GetAttribute(0); //get the parameter name
                        bElementStarted = true;
                    }
                    else
                        throw new Exception("Error parsing " + str_file);
                }
                else if (reader.NodeType == XmlNodeType.Text)
                {
                    if (bElementStarted)
                    {
                        listParameterNames.Add(strParameterName);
                        string value = reader.Value;
                        listParameterValues.Add(value);
                        strParameterName = string.Empty;
                    }
                    else
                        throw new Exception("Error parsing " + str_file);

                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    string strElementName = reader.Name.ToLower();
                    if (string.Compare(strElementName, "column") == 0)
                    {
                        //if strParameterName is not empty it means that there were no value
                        if (string.IsNullOrEmpty(strParameterName) == false)
                        {
                            listParameterNames.Add(strParameterName);
                            listParameterValues.Add("");
                        }
                        bElementStarted = false;
                    }
                    else if (string.Compare(strElementName, "table") == 0)
                    {
                        bEndedTableElement = true;
                        break; //all needed parameters to execute the DB query were read at this point
                    }
                }
            }
            if (bEndedTableElement)
            {
                command.CommandText = string.Format(command.CommandText,
                    str_table_name_prefix + "_" + str_table_name_sufix,//parameter {0} table name
                    $"`{string.Join("`,`", listParameterNames)}`" //parameter {1} columns
                    );
                command.AddArrayParameters("values", listParameterValues);
                command.ExecuteNonQuery();
            }
            else
                throw new Exception("Error parsing " + str_file);
        }

        private void ParseXMLFile(string str_file, string str_table_name_prefix, SQLiteTransaction sqlTransaction)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;
            settings.IgnoreProcessingInstructions = true;
            bool bErrorWhileParsing = true;
            //since many mods have wrong comments we need to remove them
            string strXMLFile = File.ReadAllText(str_file);
            strXMLFile = Regex.Replace(strXMLFile, "<!--[\\s\\S]*?(?=-->)--\\>", string.Empty);
            using (TextReader sr = new StringReader(strXMLFile))
            {
                using (XmlReader reader = XmlReader.Create(sr, settings))
                {
                    reader.MoveToContent();
                    //read until reach the <database name="neogame"> entry so then we can start parsing for real
                    while (reader.Read())
                    {
                        if ((reader.NodeType == XmlNodeType.Element) &&
                            (string.Compare(reader.Name.ToLower(), "database") == 0) &&
                            (string.Compare(reader.GetAttribute(0).ToLower(), "neogame") == 0))
                        {
                            bErrorWhileParsing = false;
                            break;
                        }
                    }
                    //Check if the expected element was found
                    if (bErrorWhileParsing)
                        throw new Exception("Error parsing " + str_file);
                    //Now we will continue parsing and adding values to the DB
                    while (reader.Read())
                    {
                        if ((reader.NodeType == XmlNodeType.Element) &&
                            ((string.Compare(reader.Name.ToLower(), "table") == 0)))
                        {
                            ParseElementIntoDB(str_file, reader, str_table_name_prefix, reader.GetAttribute(0).ToLower(), sqlTransaction);
                        }
                    }
                    /*switch (reader.NodeType)
                       {
                           case XmlNodeType.Element:
                               Console.Write("<{0}>", reader.Name);
                               break;
                           case XmlNodeType.Text:
                               Console.Write(reader.Value);
                               break;
                           case XmlNodeType.CDATA:
                               Console.Write("<![CDATA[{0}]]>", reader.Value);
                               break;
                           case XmlNodeType.ProcessingInstruction:
                               Console.Write("<?{0} {1}?>", reader.Name, reader.Value);
                               break;
                           case XmlNodeType.Comment:
                               Console.Write("<!--{0}-->", reader.Value);
                               break;
                           case XmlNodeType.XmlDeclaration:
                               Console.Write("<?xml version='1.0'?>");
                               break;
                           case XmlNodeType.Document:
                               break;
                           case XmlNodeType.DocumentType:
                               Console.Write("<!DOCTYPE {0} [{1}]", reader.Name, reader.Value);
                               break;
                           case XmlNodeType.EntityReference:
                               Console.Write(reader.Name);
                               break;
                           case XmlNodeType.EndElement:
                               Console.Write("</{0}>", reader.Name);
                               break;
                       }*/
                }
            }
        }

        private bool CheckSavedFileHash(string file, out byte[] file_hash)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(file))
                {
                    file_hash = md5.ComputeHash(stream);
                }
            }

            SQLiteCommand checkHashCommand = new SQLiteCommand(DbFsConnection);
            checkHashCommand.CommandText = "SELECT COUNT(`file`) FROM `file_hash` WHERE `file`=@file and `hash`=@hash";
            checkHashCommand.Parameters.Add(new SQLiteParameter("@file", file.ToLower()));
            checkHashCommand.Parameters.Add(new SQLiteParameter("@hash", file_hash));

            bool bAreEqual = checkHashCommand.ExecuteScalar().ToString() != "0";
            return bAreEqual;
        }

        private void SaveFileHash(string file, byte[] file_hash, SQLiteTransaction transaction)
        {
            SQLiteCommand saveHashCommand = new SQLiteCommand(_sqlCommandInsertOrReplaceIntoFileHash, DbFsConnection, transaction);
            saveHashCommand.Parameters.Add(new SQLiteParameter("@file", file.ToLower()));
            saveHashCommand.Parameters.Add(new SQLiteParameter("@hash", file_hash));
            saveHashCommand.ExecuteNonQuery();
        }

        private void ParseGetImagesFile(string file, string table_name, string str_folder)
        {
            //only parse the file if it was changed since last time
            byte[] fileHash = null;
            if (CheckSavedFileHash(file, out fileHash) == false)
            {
                string strFileContent = File.ReadAllText(file); 
                string[] splittedFileContent = strFileContent.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                Dictionary<string, string[]> dicImages = new Dictionary<string, string[]>(); 
                //Ignore the first two since they will correspond to "nRows=2326" and "nCols=2"
                for (int index = 2; index < splittedFileContent.Length; index++)
                {                    
                    //Access only to the name of the image
                    string strImageName = splittedFileContent[index].Split('=')[1].TrimEnd(new char[] { '\r', '\n' });

                    string strImageBaseName = Path.GetFileNameWithoutExtension(strImageName);

                    int iSmallBigIndex = 0; //0 = small, 1 = big
                    if (strImageBaseName.StartsWith("x2"))
                    {
                        iSmallBigIndex = 1;
                        strImageBaseName = strImageBaseName.Substring(3); //remove the x2_
                    }

                    if (dicImages.ContainsKey(strImageBaseName) == false)
                        dicImages.Add(strImageBaseName, new string[2] { "", "" });

                    dicImages[strImageBaseName][iSmallBigIndex] = strImageName;  
                }

                //Let's do one transaction per file to speed things up
                SQLiteTransaction sqlTransaction = DbFsConnection.BeginTransaction();
                //Create the table if it does not exist already
                SQLiteCommand command = new SQLiteCommand(DBTableAttributtesFetcher.GetSqlCreation(EDBTable.eImages), DbFsConnection, sqlTransaction);
                command.CommandText = string.Format(command.CommandText, table_name);
                command.ExecuteNonQuery();
                string strImageFolderPath = Path.Combine(str_folder, "img"); //since this will be equal for all images we do it outside of for loop to improve performance
                foreach (KeyValuePair<string, string[]> image in dicImages)
                {
                    command.Reset();
                    command.CommandText = string.Format(_sqlCommandInsertOrReplaceIntoAnyTable,
                       table_name,//parameter {0} table name
                       $"`{string.Join("`,`", new List<string> { "name", "small", "big" })}`" //parameter {1} columns
                       );

                    //Check if any of the strings are empty, and if they are don't prepend the path
                    string strSmallImagePath = string.IsNullOrEmpty(image.Value[0]) ? string.Empty : Path.Combine(strImageFolderPath, image.Value[0]);
                    string strBigImagePath = string.IsNullOrEmpty(image.Value[1]) ? string.Empty : Path.Combine(strImageFolderPath, image.Value[1]);

                    command.AddArrayParameters("values", new List<string> { image.Key, strSmallImagePath, strBigImagePath});
                    command.ExecuteNonQuery();
                }

                //now save file hash to table
                SaveFileHash(file, fileHash, sqlTransaction);
                //finally commit the transaction
                sqlTransaction.Commit();
            }
        }

        private void ParseType1(string str_folder, string str_mod_name, string str_mod_folder_name)
        {
            string strType1NeogameFile = Path.Combine(str_folder, "neogame.xml").ToLower();

            App._splashScreen.UpdateMessage("Parsing " + str_mod_folder_name + ": neogame.xml");
            //only parse the file if it was changed since last time
            byte[] fileHash = null;
            if (CheckSavedFileHash(strType1NeogameFile, out fileHash) == false)
            {
                //Let's do one transaction per file to speed things up
                SQLiteTransaction sqlTransaction = DbFsConnection.BeginTransaction();
                //Create all tables for this mod
                SQLiteCommand command = new SQLiteCommand(DbFsConnection);
                command.Transaction = sqlTransaction;
                for (int nIndex = 0; nIndex < (int)EDBTable.eTotal; nIndex++)
                {
                    EDBTable eDBTable = (EDBTable)nIndex;
                    command.Reset();
                    string strTableName = str_mod_name + "_" + str_mod_folder_name + "_" + DBTableAttributtesFetcher.GetNameSufix(eDBTable);
                    string strSqlCreation = DBTableAttributtesFetcher.GetSqlCreation(eDBTable);
                    command.CommandText = string.Format(strSqlCreation, strTableName);
                    command.ExecuteNonQuery();
                }
                //parse the xml file to DB
                ParseXMLFile(strType1NeogameFile, str_mod_name + "_" + str_mod_folder_name, sqlTransaction);
                //now save file hash to table
                SaveFileHash(strType1NeogameFile, fileHash, sqlTransaction);
                //finally commit the transaction
                sqlTransaction.Commit();
            }

            //After parsing all the data files let's check if there are any getimages.php to parse
            string strImageFile = Path.Combine(str_folder, "getimages.php");
            if (File.Exists(strImageFile))
            {
                App._splashScreen.UpdateMessage("Parsing " + str_mod_folder_name + ": getimages.php");
                ParseGetImagesFile(strImageFile, str_mod_name + "_" + str_mod_folder_name + "_" + "images", str_folder);
            }
            //If everything went well we will add to the mod list
            App.I.Mods.Add(str_mod_name + "_" + str_mod_folder_name);
        }
        //Type 0 (ones with data folder)
        private void ParseType0(string str_folder, string str_mod_name, string str_mod_folder_name)
        {
            string strType0DataFolder = Path.Combine(str_folder, "data");
            IEnumerable<string> enumFiles = Directory.EnumerateFiles(strType0DataFolder);
            foreach (string file in enumFiles)
            {
                string strFileName = Path.GetFileName(file);
                object oDBTable = DBTableAttributtesFetcher.Parse(Path.GetFileNameWithoutExtension(strFileName).ToLower());               
                if (oDBTable != null && (string.Compare(Path.GetExtension(strFileName), ".xml") == 0)) //only parse supported xml files
                {
                    EDBTable eDBTable = (EDBTable)oDBTable; 
                    App._splashScreen.UpdateMessage("Parsing " + str_mod_folder_name + ": " + strFileName);
                    //only parse the file if it was changed since last time
                    byte[] fileHash = null;
                    if (CheckSavedFileHash(file, out fileHash) == false)
                    {
                        //Let's do one transaction per file to speed things up
                        SQLiteTransaction sqlTransaction = DbFsConnection.BeginTransaction();
                        //Create the table if it does not exist already
                        string strTableName = str_mod_name + "_" + str_mod_folder_name + "_" + DBTableAttributtesFetcher.GetNameSufix(eDBTable);
                        SQLiteCommand command = new SQLiteCommand(DBTableAttributtesFetcher.GetSqlCreation(eDBTable), DbFsConnection, sqlTransaction);
                        command.CommandText = string.Format(command.CommandText, strTableName);
                        command.ExecuteNonQuery();
                        //parse the xml file to DB
                        ParseXMLFile(file, str_mod_name + "_" + str_mod_folder_name, sqlTransaction);
                        //now save file hash to table
                        SaveFileHash(file, fileHash, sqlTransaction);
                        //finally commit the transaction
                        sqlTransaction.Commit();
                    }
                }
            }
            //After parsing all the data files let's check if there are any getimages.php to parse
            string strImageFile = Path.Combine(str_folder, "getimages.php");
            if (File.Exists(strImageFile))
            {
                App._splashScreen.UpdateMessage("Parsing " + str_mod_folder_name + ": getimages.php");
                ParseGetImagesFile(strImageFile, str_mod_name + "_" + str_mod_folder_name + "_" + "images", str_folder);
            }
            //If everything went well we will add to the mod list
            App.I.Mods.Add(str_mod_name + "_" + str_mod_folder_name);
        }

        private void ParseGetModsFile(string str_game_folder)
        {
            string strGetModsFile = Path.Combine(str_game_folder, "getmods.php");
            if (File.Exists(strGetModsFile))
            {
                string strGetModsContent = File.ReadAllText(strGetModsFile);
                string[] splittedContent = strGetModsContent.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                //let's ignore the first line
                for (int index = 1; index < splittedContent.Length; index++)
                {
                    string strModName = splittedContent[index].Split('=')[1].TrimEnd(new char[] { '\r', '\n' }); //split the name
                    index++; //increase the index since the folder should be next
                    string strModFolder = splittedContent[index].Split('=')[1].TrimEnd(new char[] { '\r', '\n', '\\', '/' }); //split the folder
                    string strModBaseFolder = string.Empty;
                    //to differenciate let's add two folders to the table name or else there could be collision
                    string[] splittedFolders = strModFolder.Split('/');
                    strModBaseFolder = splittedFolders[splittedFolders.Length - 2] + "_" + splittedFolders[splittedFolders.Length - 1];

                    //now let's check for mod type
                    string strModFullFolderPath = Path.Combine(str_game_folder, strModFolder);
                    string strType1NeogameFile = Path.Combine(strModFullFolderPath, "neogame.xml");
                    if (File.Exists(strType1NeogameFile))//If neogame.xml file exists, this is a type 1 mod
                    {
                        ParseType1(strModFullFolderPath, strModName, strModBaseFolder);
                    }
                    else
                    {
                        ParseType0(strModFullFolderPath, strModName, strModBaseFolder);
                    }
                }
            }
        }

        private void CreateModsPrefixList()
        {
            App._splashScreen.UpdateMessage("Setting mods loading order");

            foreach(string mod in App.I.Mods)
            {
                string[] strModSplitted = mod.Split('_');

                if(-1 == App.I.ModsPrefix.FindIndex(new Predicate<string>(str => String.Equals(strModSplitted[0], str))))
                {
                    App.I.ModsPrefix.Add(strModSplitted[0]);
                }
            }
        }

        private void CreateInMemGameDatabase()
        {
            App._splashScreen.UpdateMessage("Finalizing in-game database");
#if DEBUG
            //For debug porpose only
            var dbFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NeoScavHelperTool", "mem.sqlite");
            _dbMemConnection = new SQLiteConnection(connectionString: "Data Source=" + dbFilename + ";Version=3;Compress=True;UTF8Encoding=True;");
#else
            _dbMemConnection = new SQLiteConnection(connectionString: "Data Source=:memory:;Version=3;Compress=True;UTF8Encoding=True;");
#endif
            DbMemConnection.Open();

            ////let's fill the TreeViewViewerTypes with the types
            //DBTableAttributtesFetcher.GetAllNameSufix().ForEach(type =>
            //{
            //    TreeViewItem treeItem = new TreeViewItem();
            //    treeItem.Header = type;
            //    MainWindow.ListTypeTreeItems.Add(treeItem);
            //});
            ////let's fill the TreeViewViewerMods with the mods where 0 will only be represented as 0_vanilla
            //App.I.Mods.Select(mod => mod.Split('_')[0]).Distinct().ToList().ForEach(modPrefix =>
            //{
            //    TreeViewItem treeItem = new TreeViewItem();
            //    treeItem.Header = modPrefix;
            //    MainWindow.ListModsTreeItems.Add(treeItem);
            //});

            //Now we will fetch by mod order data to fill the DB overwriting the previous mod data
            SQLiteCommand fsCommand = new SQLiteCommand(DbFsConnection);
            foreach (string mod in App.I.Mods)
            {
                fsCommand.Reset();
                fsCommand.CommandText = _sqlCommandSelectTablesWithNameLike;
                fsCommand.Parameters.Add(new SQLiteParameter("@name", mod + "%"));
                //First get existent tables in file system database for this mod
                List<string> listTables = new List<string>();
                using (SQLiteDataReader reader = fsCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listTables.Add(reader.GetString(0));
                    }
                }
                //now fetch all values from the file system table and insert them on the memory table   
                foreach (string tableName in listTables)
                {
                    SQLiteTransaction dataTableTransaction = DbMemConnection.BeginTransaction(); //transaction per table                    
                    SQLiteCommand command = new SQLiteCommand(DbMemConnection);
                    command.Transaction = dataTableTransaction;
                    //First fetch table column names
                    fsCommand.Reset();
                    fsCommand.CommandText = string.Format(_sqlCommandPragmaTableInfo, tableName);
                    List<string> listColumnNames = new List<string>();
                    using (SQLiteDataReader reader = fsCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listColumnNames.Add(reader.GetString(1));
                        }
                    }
                    //Now fetch their values while insert them into the mem DB
                    fsCommand.Reset();
                    fsCommand.CommandText = string.Format(_sqlCommandSelectAllTableData, tableName);
                    using (SQLiteDataReader reader = fsCommand.ExecuteReader())
                    {
                        if (reader.HasRows) //continue only if there are values on this table
                        {
                            string[] splittedTableName = tableName.Split('_');
                            string strTableNameSufix = splittedTableName[splittedTableName.Length - 1];
                            string[] splittedModName = mod.Split('_');
                            string strMemTableName = splittedModName[0] + '_' + strTableNameSufix;
                            //create this mod table if needed
                            command.CommandText = string.Format(DBTableAttributtesFetcher.GetSqlCreation(strTableNameSufix), strMemTableName);
                            command.ExecuteNonQuery();

                            while (reader.Read())
                            {
                                object[] arrayColumnValues = new object[listColumnNames.Count];
                                reader.GetValues(arrayColumnValues);
                                List<object> listColumnValues = arrayColumnValues.ToList();

                                command.Reset();
                                command.CommandText = string.Format(_sqlCommandInsertOrReplaceIntoAnyTable,
                                    strMemTableName,//parameter {0} table name
                                    $"`{string.Join("`,`", listColumnNames)}`" //parameter {1} columns
                                    );
                                command.AddArrayParameters("values", listColumnValues);
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    dataTableTransaction.Commit();
                }
            }
        }

        public List<object> GetColumnsValuesFromMemoryTable(string str_column_search, string str_column_value_search,
            string str_table_name, List<string> list_column_values_retrieves)
        {
            SQLiteCommand command = new SQLiteCommand(_dbMemConnection);
            command.CommandText = string.Format(_sqlCommandSelectColumnsFromTableWhereColumnEqualValue,
                $"`{string.Join("`,`", list_column_values_retrieves)}`", str_table_name, str_column_search);
            command.Parameters.Add(new SQLiteParameter("value", str_column_value_search));
            object[] values = new object[list_column_values_retrieves.Count];
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                reader.Read();
                reader.GetValues(values);
            }

            return values.ToList();
        }

        public List<object> GetColumnsValuesFromMemoryTableMultipleAndConditions(List<string> list_column_search, List<string> list_column_value_search,
            string str_table_name, List<string> list_column_values_retrieves)
        {
            SQLiteCommand command = new SQLiteCommand(_dbMemConnection);
            command.CommandText = string.Format(_sqlCommandSelectColumnsFromTableWhereMultipleColumnEqualValue,
                $"`{string.Join("`,`", list_column_values_retrieves)}`", str_table_name);
            command.AddMultipleAndConditions("multiple", list_column_search, list_column_value_search);
            List<object> listReturn = null;
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    object[] values = new object[list_column_values_retrieves.Count];
                    reader.Read();
                    reader.GetValues(values);
                    listReturn = values.ToList();
                }
            }

            return listReturn;
        }

        public string GetColumnValueFromMemoryTableMultipleAndConditions(List<string> list_column_search, List<string> list_column_value_search,
            string str_table_name, string str_column_value_retrieve)
        {
            SQLiteCommand command = new SQLiteCommand(_dbMemConnection);
            command.CommandText = string.Format(_sqlCommandSelectColumnFromTableWhereMultipleColumnEqualValue, str_column_value_retrieve, str_table_name);
            command.AddMultipleAndConditions("multiple", list_column_search, list_column_value_search);

            return command.ExecuteScalar().ToString();
        }

        public string GetColumnValueFromMemoryTable(string str_column_search, string str_column_value_search,
            string str_table_name, string str_column_value_retrieve)
        {
            SQLiteCommand command = new SQLiteCommand(_dbMemConnection);
            command.CommandText = string.Format(_sqlCommandSelectColumnFromTableWhereColumnEqualValue, str_column_value_retrieve, str_table_name, str_column_search);
            command.Parameters.Add(new SQLiteParameter("@value", str_column_value_search));

            return command.ExecuteScalar().ToString();
        }

        public string GetImagePathFromMemory(string str_name, string str_table_name, bool is_big)
        {
            SQLiteCommand command = new SQLiteCommand(_dbMemConnection);
            command.CommandText = string.Format(_sqlCommandSelectColumnFromTableWhereColumnEqualValue, is_big ? "big" : "small", str_table_name, "name");
            command.Parameters.Add(new SQLiteParameter("@value", str_name));

            return command.ExecuteScalar().ToString();
        }

        public object[] GetAllDataOfAnItemFromMemory(string str_value, string str_column_name, string str_table_name)
        {
            object [] arrayReturn = null;

            SQLiteCommand command = new SQLiteCommand(_dbMemConnection);
            command.CommandText = string.Format(_sqlCommandSelectAllFromTableWhereColumnEqualValue, str_table_name, str_column_name);
            command.Parameters.Add(new SQLiteParameter("@value", str_value));
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                reader.Read();
                arrayReturn = new object[reader.FieldCount];
                reader.GetValues(arrayReturn);
            }

            return arrayReturn;
        }


        public List<object[]> GetAllTableDataOfSpecificColumnsFromMemory(string str_table_name, List<string> list_column_values_retrieves)
        {
            List<object[]> listValues = new List<object[]>();
            SQLiteCommand command = new SQLiteCommand(_dbMemConnection);
            command.CommandText = string.Format(_sqlCommandSelectAllTableDataOfSpecificColumns,
                $"`{string.Join("`,`", list_column_values_retrieves)}`", str_table_name);
            try
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        object[] values = new object[list_column_values_retrieves.Count];
                        reader.GetValues(values);
                        listValues.Add(values);
                    }
                }
            }
            catch (SQLiteException sqle)
            {
                if (sqle.ErrorCode != 1) throw sqle; //if the table does not exist is an expected error
            }

            return listValues;
        }
    }
}
