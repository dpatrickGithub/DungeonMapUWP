using DungeonMap.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DungeonMap
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DMView : Page
    {
        private IDictionary<uint, InkStrokeModel> inkStrokes;
        private InkStrokeModel inkStroke;
        private ColorModel color;
        private PenSizeModel penSize;
        private HubConnection hubConnection;

        public DMView()
        {
            this.InitializeComponent();

            hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:51135/map")
                .Build();

            hubConnection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await hubConnection.StartAsync();
            };

            hubConnection.StartAsync();

            inkCanvas.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse | Windows.UI.Core.CoreInputDeviceTypes.Pen;

            InkDrawingAttributes drawingAttributes = new InkDrawingAttributes();
            drawingAttributes.Color = Windows.UI.Colors.Black;
            drawingAttributes.IgnorePressure = false;
            drawingAttributes.FitToCurve = true;
            inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);

            color = new ColorModel
            {
                A = drawingAttributes.Color.A,
                R = drawingAttributes.Color.R,
                G = drawingAttributes.Color.G,
                B = drawingAttributes.Color.B
            };

            penSize = new PenSizeModel
            {
                Height = drawingAttributes.Size.Height,
                Width = drawingAttributes.Size.Width
            };

            // By default, the InkPresenter processes input modified by
            // a secondary affordance (pen barrel button, right mouse
            // button, or similar) as ink.
            // To pass through modified input to the app for custom processing
            // on the app UI thread instead of the background ink thread, set
            // InputProcessingConfiguration.RightDragAction to LeaveUnprocessed.
            inkCanvas.InkPresenter.InputProcessingConfiguration.RightDragAction =
                InkInputRightDragAction.LeaveUnprocessed;

            // Listen for new ink or erase strokes to clean up selection UI.
            inkCanvas.InkPresenter.StrokesErased +=
                InkPresenter_StrokesErased;
            inkCanvas.InkPresenter.StrokesCollected +=
                InkPresenter_StrokesCollected;

            btnUserView.Click += BtnUserView_Click;

        }

        private async void InkPresenter_StrokesCollected(InkPresenter sender, InkStrokesCollectedEventArgs args)
        {
            var newStrokes = AddNewStrokes(inkCanvas.InkPresenter.StrokeContainer.GetStrokes());

            await hubConnection.SendAsync("AddStrokes", newStrokes);
        }

        private void BtnUserView_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(UserView));
        }

        private async void InkPresenter_StrokesErased(InkPresenter sender, InkStrokesErasedEventArgs args)
        {
            var strokeId = args.Strokes.First().Id;
            inkStrokes.Remove(strokeId);

            await hubConnection.SendAsync("DeleteStrokes", strokeId);
        }

        private void AddPoint(InkPoint point)
        {
            var inkPoint = new InkPointModel()
            {
                Pressure = point.Pressure,
                TiltX = point.TiltX,
                TiltY = point.TiltY,
                Position = new PointModel
                {
                    X = point.Position.X,
                    Y = point.Position.Y
                },
            };
            inkStroke = inkStroke ?? new InkStrokeModel();

            inkStroke.InkPoints.Add(inkPoint);
        }

        /// <summary>
        /// Adds new strokes to the collection of strokes already cached and returns the new inkstroke models. 
        /// </summary>
        /// <param name="strokes">Full list of ink strokes in the view</param>
        /// <returns></returns>
        private IDictionary<uint, InkStrokeModel> AddNewStrokes(IEnumerable<InkStroke> strokes)
        {
            inkStrokes = inkStrokes ?? new Dictionary<uint, InkStrokeModel>();
            var newStrokes = new Dictionary<uint, InkStrokeModel>();

            foreach (var stroke in strokes)
            {
                if (!inkStrokes.Keys.Contains(stroke.Id))
                {
                    inkStroke = new InkStrokeModel()
                    {
                        Color = new ColorModel
                        {
                            A = stroke.DrawingAttributes.Color.A,
                            R = stroke.DrawingAttributes.Color.R,
                            G = stroke.DrawingAttributes.Color.G,
                            B = stroke.DrawingAttributes.Color.B
                        }
                    };

                    foreach (var point in stroke.GetInkPoints())
                    {
                        AddPoint(point);
                    }

                    inkStrokes.Add(stroke.Id, inkStroke);
                    newStrokes.Add(stroke.Id, inkStroke);
                }
            }

            return newStrokes;
        }
    }
}
