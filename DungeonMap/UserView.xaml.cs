using DungeonMap.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input.Inking;
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
    public sealed partial class UserView : Page
    {
        private IDictionary<uint, InkStrokeModel> _inkStrokes;
        private HubConnection _hubConnection;

        public UserView()
        {
            this.InitializeComponent();

            var inkStrokeBuilder = new InkStrokeBuilder();

            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:51135/map")
                .Build();

            _hubConnection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await _hubConnection.StartAsync();
            };

            // event triggered when the Map hub broadcasts out a new inkstroke from the DM View.
            _hubConnection.On<IDictionary<uint, InkStrokeModel>>("StrokesAdded", async (inkStrokes) =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    _inkStrokes = _inkStrokes ?? new Dictionary<uint, InkStrokeModel>();
                    _inkStrokes.Concat(inkStrokes);

                    foreach (var stroke in inkStrokes)
                    {
                        AddStrokeToInkCanvas(inkStrokeBuilder, stroke.Value);

                        _inkStrokes.Add(stroke);
                    }
                });
            });

            // event triggered when the Map hub broadcasts out a deleted inkstroke from the DM View.
            _hubConnection.On<uint>("StrokesRemoved", async (strokeKey) =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var stroke = _inkStrokes[strokeKey];

                    RemoveStrokeFromInkCanvas(stroke.Id);

                    _inkStrokes.Remove(strokeKey);
                });
            });


            _hubConnection.StartAsync();

            inkCanvas.InkPresenter.IsInputEnabled = false;

        }

        private void RemoveStrokeFromInkCanvas(uint strokeId)
        {
            var stroke = inkCanvas.InkPresenter.StrokeContainer.GetStrokeById(strokeId);
            
            stroke.Selected = true;
            inkCanvas.InkPresenter.StrokeContainer.DeleteSelected();
        }

        private void AddStrokeToInkCanvas(InkStrokeBuilder inkStrokeBuilder, InkStrokeModel stroke)
        {
            var inkPoints = new List<InkPoint>();
            foreach (var inkPoint in stroke.InkPoints)
            {
                var position = new Point()
                {
                    X = inkPoint.Position.X,
                    Y = inkPoint.Position.Y
                };

                var newInkPoint = new InkPoint(position, inkPoint.Pressure, inkPoint.TiltX, inkPoint.TiltY, 0);

                inkPoints.Add(newInkPoint);
            }

            var inkStroke = inkStrokeBuilder.CreateStrokeFromInkPoints(inkPoints, Matrix3x2.Identity);
            stroke.Id = inkStroke.Id;

            inkCanvas.InkPresenter.StrokeContainer.AddStroke(inkStroke);
        }

    }
}
