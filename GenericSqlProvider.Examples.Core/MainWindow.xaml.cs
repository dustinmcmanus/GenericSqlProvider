using GenericSqlProvider.Configuration;
using GenericSqlProvider.Examples.Common;
using GenericSqlProvider.Oracle;
using GenericSqlProvider.SqlServer;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;

namespace GenericSqlProvider.Examples.Core
{
    public partial class MainWindow : Window
    {
        private const string UserInputFileName = "ConfigurationRecord.xml";
        private ReadOnlyObservableCollection<GenericSqlProvider.Configuration.DatabaseProviderInfo> databaseProviderOptions;
        private readonly ConfigurationRecord guiConfiguration;
        private readonly ConfigurationLoader configManager = new ConfigurationLoader(UserInputFileName);

        public ObservableCollection<DatabaseProviderInfo> Providers { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;
            guiConfiguration = configManager.GetConfigurationRecord();
            this.DataContext = guiConfiguration;
            PrepareComboBoxOptions();
#if DEBUG
            pwdDatabaseUserPassword.Password = "db";
#endif
        }

        private void PrepareComboBoxOptions()
        {
            databaseProviderOptions = DatabaseProviders.GetSupportedProviders();
            this.cboDatabaseProvider.ItemsSource = databaseProviderOptions;
            if (guiConfiguration.DatabaseProvider is null)
            {
                guiConfiguration.DatabaseProvider = Providers[0];
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            guiConfiguration.DatabaseUserPassword = pwdDatabaseUserPassword.Password;
            ValidateForm();
            SaveConfiguration();
            string connectionString = GetConnectionString();
            var sqlProviderFactory = GetSqlProviderFactory(connectionString);

            using (IDbConnection connection = sqlProviderFactory.CreateConnection())
            {
                connection.Open();

                var settingNameList = Demonstrations.GetSettingsWithMicrosoftDocumentationAddParameter(connection);
                var settingNameList2 = Demonstrations.GetSettingsWithDatabaseUtilsAddParameter(connection);
            }


            guiConfiguration.DatabaseUserPassword = "";
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
                configManager.SaveConfiguration(guiConfiguration);
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
            return connectionStringBuilder.GetConnectionString(guiConfiguration.DatabaseProvider.InvariantName);
        }

        private IDbProviderFactory GetSqlProviderFactory(string connectionString)
        {
            switch (guiConfiguration.DatabaseProvider.IntegerValue)
            {
                case (int)DatabaseProviderType.Oracle:
                    return new GenericOracleProviderFactory(connectionString);
                case (int)DatabaseProviderType.SqlServer:
                    return new GenericSqlServerProviderFactory(connectionString);
                default:
                    throw new NotImplementedException($"Generic SQL Providers for {guiConfiguration.DatabaseProvider.InvariantName} are not supported yet.");
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
                HostName = guiConfiguration.DatabaseHostName,
                Port = guiConfiguration.DatabasePort,
                Name = guiConfiguration.DatabaseName,
                UserName = guiConfiguration.DatabaseUserName,
                UserPassword = guiConfiguration.DatabaseUserPassword
            };
            return connectionStringParams;
        }
    }
}
