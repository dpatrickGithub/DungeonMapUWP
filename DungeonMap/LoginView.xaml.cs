using DungeonMap.Helpers;
using DungeonMap.Models.Auth;
using DungeonMap.Providers;
using DungeonMap.Validators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Json;
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
    public sealed partial class Login : Page
    {
        private LoginValidator validator;
        private ResourceLoader resources;
        private JsonHelper<LoginModel> requestJsonHelper;
        private JsonHelper<TokenModel> responseJsonHelper;

        public Login()
        {
            this.InitializeComponent();
            resources = ResourceLoader.GetForCurrentView("AppResources");
            validator = new LoginValidator();
            requestJsonHelper = new JsonHelper<LoginModel>();
            responseJsonHelper = new JsonHelper<TokenModel>();
        }
        
        private async void BtnSignIn_Click(object sender, RoutedEventArgs e)
        {
            if (!validator.ValidateUserName(tbUserName.Text) || validator.ValidatePassword(pbPassword.Password))
            {
                throw new Exception("Username or password does not meet length requirements.");
            }

            var response = new HttpResponseMessage();

            using (var client = new HttpClient())
            {
                var requestMessage = new HttpRequestMessage();
                requestMessage.RequestUri = new Uri(resources.GetString("BaseUri") + "token");
                requestMessage.Method = HttpMethod.Post;

                var model = new LoginModel()
                {
                    UserName = tbUserName.Text,
                    Password = pbPassword.Password
                };

                requestMessage.Content = new StringContent(requestJsonHelper.ConvertToJson(model));

                response = await client.SendAsync(requestMessage);
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error logging in: " + responseBody);
            }

            var userToken = responseJsonHelper.ConvertToModel(responseBody);

            var appUser = AppUser.Instance;

            appUser.Token = userToken.Token;
            appUser.UserId = userToken.UserId;

            if (appUser.UserId.HasValue)
            {
                Frame.Navigate(typeof(GamesListView));
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
