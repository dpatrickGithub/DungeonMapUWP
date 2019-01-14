using DungeonMap.Helpers;
using DungeonMap.Models.Auth;
using DungeonMap.Validators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DungeonMap
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RegisterView : Page
    {
        private ResourceLoader resources;
        private JsonHelper<RegisterUserModel> requestHelper;

        public RegisterView()
        {
            this.InitializeComponent();
            resources = ResourceLoader.GetForCurrentView("AppResources");
            requestHelper = new JsonHelper<RegisterUserModel>();
        }

        private async void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate())
            {
                throw new Exception("Something went wrong.");
            }

            var requestBody = new RegisterUserModel()
            {
                UserName = tbUsername.Text,
                Password = pbPassword.Password,
                Biography = tbBiography.Text,
                Email = tbEmail.Text,
                FirstName = tbFirstName.Text,
                LastName = tbLastName.Text,
                PhoneNumber = tbPhone.Text,
                PreferredCharacterName = tbCharacterName.Text,
                SkypeHandle = tbSkypeHandle.Text,
                SubscriptionType = Common.SubscriptionType.Free
            };

            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(resources.GetString("BaseUri") + "api/auth/register");
                request.Method = HttpMethod.Post;
                request.Content = new StringContent(requestHelper.ConvertToJson(requestBody), Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);
                
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to create new user.");
                }

                Frame.Navigate(typeof(Login));
            }
        }

        private bool Validate()
        {
            return ValidateUserName() 
                && ValidatePasswordsMatch() 
                && ValidatePasswordStrength()
                && ValidateEmail()
                && !String.IsNullOrEmpty(tbFirstName.Text)
                && !String.IsNullOrEmpty(tbLastName.Text);
        }

        private bool ValidateUserName()
        {
            var isValid = !String.IsNullOrWhiteSpace(tbUsername.Text) && tbUsername.Text.Length > 6;

            if (!isValid)
            {
                //throw new Exception("Username must be 6 characters in length and contain no white spaces");
            }

            return isValid;
        }

        private bool ValidatePasswordsMatch()
        {
            var isValid = pbConfirmPassword.Password == pbPassword.Password;

            if (!isValid)
            {
                //throw new Exception("Passwords do not match");
            }

            return isValid;
        }

        private bool ValidatePasswordStrength()
        {
            const int MIN_LENGTH = 8;
            const int MAX_LENGTH = 15;

            var password = pbPassword.Password;

            if (String.IsNullOrWhiteSpace(password))
            {
                //throw new ArgumentNullException();
                return false;
            }


            bool meetsLengthRequirements = password.Length >= MIN_LENGTH && password.Length <= MAX_LENGTH;
            bool hasUpperCaseLetter = false;
            bool hasLowerCaseLetter = false;
            bool hasDecimalDigit = false;

            if (meetsLengthRequirements)
            {
                foreach (char c in password)
                {
                    if (char.IsUpper(c)) hasUpperCaseLetter = true;
                    else if (char.IsLower(c)) hasLowerCaseLetter = true;
                    else if (char.IsDigit(c)) hasDecimalDigit = true;
                }
            }

            bool isValid = meetsLengthRequirements
                        && hasUpperCaseLetter
                        && hasLowerCaseLetter
                        && hasDecimalDigit;

            if (!isValid)
            {
                throw new Exception("Password does not meet strength requirements.");
            }

            return isValid;

        }

        private bool ValidateEmail()
        {
            var isValid = !String.IsNullOrWhiteSpace(tbEmail.Text) && tbEmail.Text.Contains('@');

            if (!isValid)
            {
                //throw new Exception("Email is not properly formatted");
            }

            return isValid;
        }
    }
}
