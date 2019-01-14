using DungeonMap.Common;
using DungeonMap.Helpers;
using DungeonMap.Models;
using DungeonMap.Providers;
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
    public sealed partial class CreateNewCharacterView : Page
    {
        private CharacterModel model;
        private ResourceLoader resources;
        private HttpClient client;
        private AppUser appUser;
        private JsonHelper<CharacterModel> charJsonHelper;
        private JsonHelper<List<GameUserModel>> friendsGamesJsonHelper;

        public List<GameUserModel> FriendsGames { get; set; }

        public CreateNewCharacterView()
        {
            model = new CharacterModel();
            resources = ResourceLoader.GetForCurrentView("AppResources");
            client = new HttpClient();
            appUser = AppUser.Instance;
            charJsonHelper = new JsonHelper<CharacterModel>();
            friendsGamesJsonHelper = new JsonHelper<List<GameUserModel>>();

            // populate friends' games.
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(resources.GetString("BaseUri") + $"api/games/users/{appUser.UserId}/friends");
            request.Headers.Add("Authorization", $"Bearer {appUser.Token}");

            var response = client.SendAsync(request).Result;

            var responseBody = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception($"Error: {responseBody}");
            }

            FriendsGames = friendsGamesJsonHelper.ConvertToModel(responseBody);

            this.InitializeComponent();
        }

        private async void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(tbCharacterName.Text))
            {
                return;
            }

            model.CharacterName = tbCharacterName.Text;
            model.RoleType = RoleType.Player;
            model.UserId = appUser.UserId.Value;

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(resources.GetString("BaseUri") + $"api/characters");
            request.Method = HttpMethod.Post;
            request.Headers.Add("Authorization", $"Bearer {appUser.Token}");
            request.Content = new StringContent(charJsonHelper.ConvertToJson(model), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);

            var responseBody = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception($"Error: {responseBody}");
            }

            Frame.Navigate(typeof(CharacterListView));
        }

        private void CbFriendsGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (StackPanel)sender;
            if (selectedItem.DataContext == null)
            {
                model.GameId = default(int);
            }

            model.GameId = Convert.ToInt32(selectedItem.DataContext);
        }
    }
}
