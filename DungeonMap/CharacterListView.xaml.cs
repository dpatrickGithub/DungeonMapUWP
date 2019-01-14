﻿using DungeonMap.Helpers;
using DungeonMap.Models;
using DungeonMap.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class CharacterListView : Page
    {
        private JsonHelper<List<CharacterModel>> jsonHelper;
        public ResourceLoader _resources;
        private AppUser _appUser;
        private HttpClient _httpClient;

        public List<CharacterModel> Characters { get; set; }

        public CharacterListView()
        {
            _appUser = AppUser.Instance;
            jsonHelper = new JsonHelper<List<CharacterModel>>();

            _resources = ResourceLoader.GetForCurrentView("AppResources");

            // TODO: Wrap httpclient in a helper and use an HttpClientFactory for generation.

            // get active games list from api on view initialization. 
            _httpClient = new HttpClient();

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(_resources.GetString("BaseUri") + $"api/characters/users/{_appUser.UserId}");
            request.Headers.Add("Authorization", $"Bearer {_appUser.Token}");

            var response = _httpClient.SendAsync(request).Result; // cant make constructor async :(

            var responseBody = response.Content.ReadAsStringAsync().Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error loading Characters: " + responseBody);
            }

            Characters = jsonHelper.ConvertToModel(responseBody);

            this.InitializeComponent();
        }

        private void BtnCreateNewCharacter_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CreateNewCharacterView));
        }

        private void NvNavigation_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var selectedItem = sender.SelectedItem as NavigationViewItem;
            switch (selectedItem.Tag.ToString())
            {
                case "Characters":
                    Frame.Navigate(typeof(CharacterListView));
                    break;
                case "Games":
                    Frame.Navigate(typeof(GamesListView));
                    break;
                case "Settings":
                    Frame.Navigate(typeof(UserSettingsView));
                    break;
            }
        }
    }
}
