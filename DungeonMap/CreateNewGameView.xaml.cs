﻿using DungeonMap.Common;
using DungeonMap.Helpers;
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
    public sealed partial class CreateNewGameView : Page
    {
        private ResourceLoader resources;
        private HttpClient client;
        private AppUser appUser;
        private JsonHelper<GameModel> gameModelJsonHelper;
        private JsonHelper<CharacterModel> charJsonHelper;
        private GameModel model;

        public IEnumerable<IdNamePair> FriendsCharacters { get; set; }

        public CreateNewGameView()
        {
            model = new GameModel();
            resources = ResourceLoader.GetForCurrentView();
            appUser = AppUser.Instance;
            charJsonHelper = new JsonHelper<CharacterModel>();
            gameModelJsonHelper = new JsonHelper<GameModel>();

            this.InitializeComponent();
        }

        private void DdGameType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var gameType = (ComboBox)sender;
            var selectedItem = (ComboBoxItem)gameType.SelectedItem;

            switch (selectedItem.Name)
            {
                case "dnd5e":
                    model.GameType = GameType.DungeonsAndDragons5e;
                    break;
                case "dnd4e":
                    model.GameType = GameType.DungeonsAndDragons4e;
                    break;
                case "dnd3_5e":
                    model.GameType = GameType.DungeonsAndDragons3_5e;
                    break;
                case "dnd3e":
                    model.GameType = GameType.DungeonsAndDragons3e;
                    break;
                case "adnd2e":
                    model.GameType = GameType.AdvancedDungeonsAndDragons2e;
                    break;
                case "adnd1e":
                    model.GameType = GameType.AdvancedDungeonsAndDragons1e;
                    break;
                case "dnd1e":
                    model.GameType = GameType.DungeonsAndDragons1e;
                    break;
                case "pathfinder":
                    model.GameType = GameType.Pathfinder;
                    break;
            }
        }

        private async void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(tbGameName.Text))
            {
                throw new Exception("Game Name is required");
            }
            if (cbGameType.SelectedItem == null)
            {
                throw new Exception("Game Type is required");
            }

            model.Name = tbGameName.Text;
            model.IsActive = true;
            model.Characters.Add(new CharacterModel()
            {
                CharacterName = "DM",
                RoleType = RoleType.GameMaster,
                UserId = appUser.UserId.Value
            });

            //Set up http client to send new game.
            client = client ?? new HttpClient();

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(resources.GetString("BaseUri") + $"api/games");
            request.Method = HttpMethod.Post;
            request.Headers.Add("Authorization", $"Bearer {appUser.Token}");

            request.Content = new StringContent(gameModelJsonHelper.ConvertToJson(model));

            var response = await client.SendAsync(request);

            var responseBody = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error: {responseBody}");
            }

            Frame.Navigate(typeof(GamesListView));
        }

        private async void CbPlayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            model.Characters = model.Characters ?? new List<CharacterModel>();

            client = client ?? new HttpClient();

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(resources.GetString("BaseUri") + $"api/characters/users/{appUser.UserId}");
            request.Headers.Add("Authorization", $"Bearer {appUser.Token}");

            var response = await client.SendAsync(request);

            var responseBody = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception($"Error: {responseBody}");
            }

            model.Characters.Add(charJsonHelper.ConvertToModel(responseBody));
        }

        private void BtnRemove_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var btn = sender as SymbolIcon;

            var character = model.Characters.Single(ch => ch.Id == (int)btn.DataContext);
            model.Characters.Remove(character);
        }
    }
}