﻿using GenericSqlProvider.SqlServer;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace GenericSqlProvider.Examples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string MainWindowSavedUserInputsFile = "ConfigurationRecord.xml";
        ReadOnlyObservableCollection<DatabaseProviderInfo> databaseProviderOptions;
        ConfigurationRecord GuiConfiguration;
        ConfigurationLoader configManager = new ConfigurationLoader(MainWindowSavedUserInputsFile);

        public ObservableCollection<DatabaseProviderInfo> Providers { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;
            GuiConfiguration = configManager.GetConfigurationRecord();
            this.DataContext = GuiConfiguration;
            PrepareComboBoxOptions();
        }

        private void PrepareComboBoxOptions()
        {
            Providers = new ObservableCollection<DatabaseProviderInfo>();
            Providers.Add(new DatabaseProviderInfo() { DisplayName="Oracle", InvariantName= "Oracle.ManagedDataAccess.Client" });
            Providers.Add(new DatabaseProviderInfo() { DisplayName = "SQL Server", InvariantName = "System.Data.SqlClient" });
            databaseProviderOptions = new ReadOnlyObservableCollection<DatabaseProviderInfo>(Providers);
            this.cboDatabaseProvider.ItemsSource = databaseProviderOptions;
            if (GuiConfiguration.DatabaseProvider is null)
            {
                GuiConfiguration.DatabaseProvider = Providers[0];
            }
        }
        
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ValidateForm();
            SaveConfiguration();
            string connectionString = GetConnectionString();
            var sqlProviderFactory = GetSqlProviderFactory(connectionString);

            // TODO: use provider factory in examples

            this.Close();
        }
        
        private void ValidateForm()
        {
            try
            {
                ValidateInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid user inputs: " + ex.Message, "Validation Failed", MessageBoxButton.OK);
                throw;
            }
        }

        private void SaveConfiguration()
        {
            try
            {
                configManager.SaveConfiguration(GuiConfiguration);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving user inputs: {ex.Message}", "Failure to save user inputs", MessageBoxButton.OK);
                throw;
            }
        }

        private string GetConnectionString()
        {
            var connectionInfo = GetParametersForConnectionString();
            var connectionStringBuilder = new ConnectionStringBuilder(connectionInfo);
            return connectionStringBuilder.GetConnectionString(GuiConfiguration.DatabaseProvider.InvariantName);
        }

        private IDbProviderFactory GetSqlProviderFactory(string connectionString)
        {
            switch (GuiConfiguration.DatabaseProvider.InvariantName)
            {
                case "Oracle.ManagedDataAccess.Client":
                    return new GenericOracleProviderFactory(connectionString);
                case "System.Data.SqlClient":
                    return new GenericSqlServerProviderFactory(connectionString);
                default:
                    throw new NotImplementedException($"Generic SQL Providers for {GuiConfiguration.DatabaseProvider.InvariantName} are not supported yet.");
            }
        }

        private void ValidateInputs()
        {
            // TODO: Reject other empty inputs for required fields and other form field validation
            if (string.IsNullOrEmpty(pwdDatabaseUserPassword.Password))
            {
                throw new Exception("Empty password. Please enter the passwords for the database connections.");
            }
        }

        private DatabaseConnectionInfo GetParametersForConnectionString()
        {
            var connectionStringParams = new DatabaseConnectionInfo()
            {
                HostName = GuiConfiguration.DatabaseHostName,
                Port = GuiConfiguration.DatabasePort,
                Name = GuiConfiguration.DatabaseName,
                UserName = GuiConfiguration.DatabaseUserName,
                UserPassword = GuiConfiguration.DatabaseUserPassword
            };
            return connectionStringParams;
        }
    }
}