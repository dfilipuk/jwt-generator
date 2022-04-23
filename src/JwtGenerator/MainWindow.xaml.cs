using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JwtGenerator.Config;
using JwtGenerator.Extensions;
using JwtGenerator.Jwt;
using Microsoft.Win32;

namespace JwtGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<string, TokenLifetime> _tokenLifetime = new Dictionary<string, TokenLifetime>
        {
            { "Rb1h", TokenLifetime.OneHour },
            { "Rb24h", TokenLifetime.TwentyFourHours },
            { "Rb1m", TokenLifetime.OneMonth },
            { "Rb1y", TokenLifetime.OneYear },
        };
        private readonly Dictionary<TokenLifetime, RadioButton> _radioButtons;
        private readonly OpenFileDialog _openFileDialog;
        private readonly SaveFileDialog _saveFileDialog;

        private TokenLifetime _lifetime = TokenLifetime.OneHour;

        public MainWindow()
        {
            InitializeComponent();
            CcClaims.NewItemTypes = new List<Type> { typeof(JwtClaim) };
            _radioButtons = new Dictionary<TokenLifetime, RadioButton>
            {
                { TokenLifetime.OneHour, Rb1h },
                { TokenLifetime.TwentyFourHours, Rb24h },
                { TokenLifetime.OneMonth, Rb1m },
                { TokenLifetime.OneYear, Rb1y },
            };
            _openFileDialog = new OpenFileDialog();
            _openFileDialog.CheckPathExists = true;
            _openFileDialog.CheckFileExists = true;
            _openFileDialog.RestoreDirectory = true;
            _openFileDialog.Filter = "JSON files (*.json)|*.json";
            _saveFileDialog = new SaveFileDialog();
            _saveFileDialog.CheckPathExists = true;
            _saveFileDialog.CheckFileExists = false;
            _saveFileDialog.RestoreDirectory = true;
            _saveFileDialog.Filter = "JSON files (*.json)|*.json";
        }

        private void SetLifetime(object sender, RoutedEventArgs e)
        {
            _lifetime = _tokenLifetime[(sender as RadioButton).Name];
        }

        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
            {
                return;
            }

            try
            {
                var config = GetTokenConfig();
                string token = JwtService.CreateToken(config);
                TbToken.Text = token;
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(TbToken.Text);
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            TbToken.Text = String.Empty;
        }

        private void OpenCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (_openFileDialog.ShowDialog() != true)
            {
                return;
            }

            try
            {
                TokenConfig config = ConfigService.LoadConfigFromFile(_openFileDialog.FileName);
                SetTokenConfig(config);
                ValidateInput();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void SaveAsCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (!ValidateInput())
            {
                return;
            }

            if (_saveFileDialog.ShowDialog() != true)
            {
                return;
            }

            try
            {
                TokenConfig config = GetTokenConfig();
                ConfigService.SaveConfigIntoFile(_saveFileDialog.FileName, config);
                ShowInformation("Configuration saved!");
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void ShowError(string text)
        {
            MessageBox.Show(text, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ShowInformation(string text)
        {
            MessageBox.Show(text, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool ValidateInput()
        {
            var isInputValid = true;

            isInputValid &= TbAudience.ValidateText();
            isInputValid &= TbIssuer.ValidateText();
            isInputValid &= TbKey.ValidateText();
            isInputValid &= TbRole.ValidateText();

            return isInputValid;
        }

        private TokenConfig GetTokenConfig()
        {
            return new TokenConfig
            {
                Key = TbKey.Text,
                Audience = TbAudience.Text,
                Issuer = TbIssuer.Text,
                Lifetime = _lifetime,
                Role = TbRole.Text,
                Claims = CcClaims.Items
                    .Select(o => (JwtClaim)o)
                    .ToArray()
            };
        }

        private void SetTokenConfig(TokenConfig config)
        {
            TbKey.Text = config.Key;
            TbAudience.Text = config.Audience;
            TbIssuer.Text = config.Issuer;
            _lifetime = config.Lifetime;
            TbRole.Text = config.Role;

            foreach (KeyValuePair<TokenLifetime, RadioButton> keyValuePair in _radioButtons)
            {
                keyValuePair.Value.IsChecked = false;
            }

            _radioButtons[config.Lifetime].IsChecked = true;

            CcClaims.Items.Clear();

            if (config.Claims != null)
            {
                foreach (JwtClaim claim in config.Claims)
                {
                    CcClaims.Items.Add(claim);
                }
            }
        }
    }
}
