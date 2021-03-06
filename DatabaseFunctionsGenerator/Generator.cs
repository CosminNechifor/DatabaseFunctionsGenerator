﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class Generator
    {
        private Database _database;

        private SqlGenerator _sqlGenerator;
        private TypescriptGenerator _typescriptGenerator;
        private PhpGenerator _phpGenerator;
        private NotificationSystem _notificationSystem;

        public Generator(Database database)
        {
            _database = database;

            _sqlGenerator = new SqlGenerator(_database);
            _phpGenerator = new PhpGenerator(_database);
            _typescriptGenerator = new TypescriptGenerator(_database);
            _notificationSystem = new NotificationSystem();
        }

        private void AddMissingFields()
        {
            //add notification system
            if(!_database.HasNotificationSystem)
            {
                _database.Tables.Add(_notificationSystem.GenerateNotificationTable());
            }

            foreach(Table table in _database.Tables)
            {
                if (!table.HasPrimaryKey)
                {
                    Column column;
                    ColumnType type;

                    type = new ColumnType(Types.Integer, true, true);
                    column = new Column($"{table.SingularName}Id", type);

                    table.Columns.Insert(0, column);
                }

                if (!table.HasCreationTime)
                {
                    Column column;
                    ColumnType type;

                    type = new ColumnType(Types.DateTime);
                    column = new Column($"CreationTime", type);

                    table.Columns.Add(column);
                }
            }
        }

        private void SetParentChildsFields()
        {
            foreach(Relation relation in _database.Relations)
            {
                switch(relation.Type)
                {
                    case RelationType.OneToMany:
                        relation.Table1.Childs.Add(relation.Table2);
                        relation.Table2.Parents.Add(relation.Table1);

                        relation.Table2.Columns.Insert(1, new Column($"{relation.Table1.SingularName}Id", new ColumnType(Types.Integer, true)));
                        break;
                }
            }
        }

        public void Generate()
        {
            string path;

            path = $"GeneratorResult\\{Helpers.GenerateTimeStamp()}";

            if (!Directory.Exists("GeneratorResult"))
            {
                Directory.CreateDirectory("GeneratorResult");
            }

            Directory.CreateDirectory(path);
            Directory.CreateDirectory($"{path}\\Php");
            Directory.CreateDirectory($"{path}\\Typescript");

            //add missing fields
            AddMissingFields();
            SetParentChildsFields();

            _sqlGenerator.Generate(path);
            _phpGenerator.Generate(path);
            _typescriptGenerator.Generate(path);

            foreach (string file in Directory.EnumerateFiles($"{path}\\Php"))
            {
                File.Copy(file, $"d:\\xampp\\htdocs\\generator\\Test\\{Path.GetFileName(file)}", true);
            }

            foreach (string file in Directory.EnumerateFiles($"{path}\\Php\\Models"))
            {
                File.Copy(file, $"d:\\xampp\\htdocs\\generator\\Test\\Models\\{Path.GetFileName(file)}", true);
            }
        }
    }
}
