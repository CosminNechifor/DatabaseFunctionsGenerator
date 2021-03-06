﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DatabaseFunctionsGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Database _database;
        private Table _selectedTable;

        public Table SelectedTable
        {
            get { return _selectedTable; }
            set
            {
                _selectedTable = value;
                OnPropertyChanged("SelectedTable");
            }
        }

        public Database Database
        {
            get { return _database; }
            set
            {
                _database = value;
                OnPropertyChanged("Database");
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Database = new Database();

            Database.Tables.Add(new Table("Users"));
            Database.Tables.Add(new Table("Locations"));
            Database.Tables.Add(new Table("AccessLogs"));
            Database.Tables.Add(new Table("Doors"));
            Database.Tables.Add(new Table("Pins"));

            //            Database.Tables[0].Columns.Add(new Column("Id", new ColumnType(Types.Integer, true, true)));
            Database.Tables[0].Columns.Add(new Column("Username", new ColumnType(Types.Varchar, 20)));
            Database.Tables[0].Columns.Add(new Column("Age", new ColumnType(Types.Integer)));
            Database.Tables[0].Columns.Add(new Column("Description", new ColumnType(Types.Text)));
            Database.Tables[0].Columns.Add(new Column("CNP", new ColumnType(Types.Varchar, 20)));
            Database.Tables[0].Columns.Add(new Column("DateOfBirth", new ColumnType(Types.DateTime)));

            Database.Tables[2].Columns.Add(new Column("CardValue", new ColumnType(Types.Varchar, 20)));

            Database.Tables[3].Columns.Add(new Column("Name", new ColumnType(Types.Varchar, 20)));

            Database.Tables[4].Columns.Add(new Column("Pin", new ColumnType(Types.Varchar, 20)));

            Database.Relations.Add(new Relation(Database.Tables[0], Database.Tables[2], RelationType.OneToMany));
            Database.Relations.Add(new Relation(Database.Tables[3], Database.Tables[2], RelationType.OneToMany));
            Database.Relations.Add(new Relation(Database.Tables[2], Database.Tables[4], RelationType.OneToMany));

            SelectedTable = Database.Tables[0];

            Generator generator = new Generator(_database);
            generator.Generate();
            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
