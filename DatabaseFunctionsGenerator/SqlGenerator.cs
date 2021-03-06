﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFunctionsGenerator
{
    public class SqlGenerator : IGenerator
    {
        private Database _database;

        public SqlGenerator(Database database)
        {
            _database = database;
        }

        private string GenerateColumnQuery(Column column)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append($"`{column.Name}` ");

            foreach (Table table in _database.Tables)
            {
                //builder.AppendLine(GenerateTableQuery(table));
            }

            builder.Append(column.Type.GetMysqlType());
            builder.Append($"  NOT NULL,");

            return builder.ToString();
        }

        private string GenerateTableQuery(Table table)
        {
            StringBuilder builder = new StringBuilder();
            Column primaryKeyColumn;

            builder.AppendLine($"CREATE TABLE `{table.Name}` (");

            foreach (Column column in table.Columns)
            {
                builder.AppendLine(GenerateColumnQuery(column));
            }

            //remove last comma. 3 because of \r\t at the end of line
            builder = builder.Remove(builder.Length - 3, 1);

            builder.AppendLine($") ENGINE=InnoDB DEFAULT CHARSET=latin1;");
            builder.AppendLine();

            foreach (Column column in table.PrimaryKeyColumns)
            {
                builder.AppendLine($"ALTER TABLE `{table.Name}` ADD PRIMARY KEY(`{column.Name}`); ");

                if (column.Type.AutoIncrement)
                {
                    builder.AppendLine($"ALTER TABLE `{table.Name}`  MODIFY `{column.Name}` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT = 1; ");
                }
            }


            return builder.ToString();
        }

        public void Generate(string path)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("-- generated by database functions generator automatically");
            builder.AppendLine("-- for mysql ");
            builder.AppendLine("SET SQL_MODE = \"NO_AUTO_VALUE_ON_ZERO\";");
            builder.AppendLine("SET time_zone = \"+00:00\";");

            foreach (Table table in _database.Tables)
            {
                builder.AppendLine(GenerateTableQuery(table));
            }

            Helpers.WriteFile($"{path}\\sqlDatabase.sql", builder.ToString());
        }
    }
}
