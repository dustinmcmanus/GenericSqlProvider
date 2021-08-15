using GenericSqlProvider.SqlServer;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Security;
using System.Windows;
using GenericSqlProvider.Configuration;
using GenericSqlProvider.Oracle;
using System.Collections.Generic;

namespace GenericSqlProvider.Examples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string MainWindowSavedUserInputsFile = "ConfigurationRecord.xml";
        ReadOnlyObservableCollection<GenericSqlProvider.Configuration.DatabaseProviderInfo> databaseProviderOptions;
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
#if DEBUG
            pwdDatabaseUserPassword.Password = "db";
#endif
        }

        private void PrepareComboBoxOptions()
        {
            databaseProviderOptions = DatabaseProviders.GetSupportedProviders();
            var providers2 = DatabaseProviders.GetSupportedProviders();
            var providers3 = DatabaseProviders.GetSupportedProviders();

            if (object.ReferenceEquals(providers2, providers3))
            {
                System.Diagnostics.Debug.WriteLine("same");
                if (providers2.Contains(new DatabaseProviderInfo() { InvariantName = "Oracle.ManagedDataAccess.Client" })) {

                    System.Diagnostics.Debug.WriteLine("same2");
                }
            }
            //databaseProviderOptions = new ReadOnlyObservableCollection<DatabaseProviderInfo>(DatabaseProviders.GetSupportedProviders());
            this.cboDatabaseProvider.ItemsSource = databaseProviderOptions;
            if (GuiConfiguration.DatabaseProvider is null)
            {
                GuiConfiguration.DatabaseProvider = Providers[0];
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            GuiConfiguration.DatabaseUserPassword = pwdDatabaseUserPassword.Password;
            ValidateForm();
            SaveConfiguration();
            string connectionString = GetConnectionString();
            var sqlProviderFactory = GetSqlProviderFactory(connectionString);

            // TODO: use provider factory in examples
            using (IDbConnection connection = sqlProviderFactory.CreateConnection())
            {
                connection.Open();
                //using (IDbCommand command = connection.CreateCommand())
                //{
                //    command.CommandText = @"INSERT INTO USER_SETTING (SETTING_NAME, VALUE, USER_NAME) VALUES (@SETTING_NAME, @VALUE, @USER_NAME)";
                //    GenericSqlProvider.DatabaseUtils.AddParameter(command, "@SETTING_NAME", "FONT");
                //    GenericSqlProvider.DatabaseUtils.AddParameter(command, "@VALUE", "COURIER NEW");
                //    GenericSqlProvider.DatabaseUtils.AddParameter(command, "@USER_NAME", null);
                //    //var param = command.CreateParameter();
                //    //param.ParameterName = "NAME";
                //    //param.Value = "Bob";
                //    //command.Parameters.Add(param);
                //    command.ExecuteNonQuery();
                //}

                var settingNameList = new List<string>();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT SETTING_NAME FROM USER_SETTING WHERE USER_NAME=@USER_NAME";

                    //GenericSqlProvider.DatabaseUtils.AddParameter(command, "@USER_NAME", "Jill");

                    var param = command.CreateParameter();
                    param.ParameterName = "@USER_NAME";
                    param.Value = "Jill";
                    command.Parameters.Add(param);

                    using (IDataReader rdr = command.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            settingNameList.Add(rdr.GetString(0));
                        }
                    }
                }

                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT SETTING_NAME FROM USER_SETTING WHERE USER_NAME=@USER_NAME";

                    GenericSqlProvider.DatabaseUtils.AddParameter(command, "@USER_NAME", "Jill");

                    using (IDataReader rdr = command.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            settingNameList.Add(rdr.GetString(0));
                        }
                    }
                }
            }


            GuiConfiguration.DatabaseUserPassword = "";
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
            var str2 = connectionStringBuilder.GetConnectionString(GuiConfiguration.DatabaseProvider);
            return connectionStringBuilder.GetConnectionString(GuiConfiguration.DatabaseProvider.InvariantName);
        }

        private IDbProviderFactory GetSqlProviderFactory(string connectionString)
        {
            switch (GuiConfiguration.DatabaseProvider.IntegerValue)
            {
                case (int)DatabaseProviderType.Oracle:
                    return new GenericOracleProviderFactory(connectionString);
                case (int)DatabaseProviderType.SqlServer:
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

        private ConnectionConfiguration GetParametersForConnectionString()
        {
            var connectionStringParams = new ConnectionConfiguration()
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
