using OxyPlot;
using OxyPlot.Series;
using MbientLab.MetaWear;
using MbientLab.MetaWear.Core;
using MbientLab.MetaWear.Data;
using MbientLab.MetaWear.Sensor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Devices.Bluetooth;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization.DateTimeFormatting;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using OxyPlot.Axes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace RealTimeGraph {
    public class MainViewModel {
        public const int MAX_DATA_SAMPLES = 960;
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LineGraph : Page {

        private IMetaWearBoard metawear1;
        private IAccelerometer accelerometer1;
        
        private IMetaWearBoard metawear2;
        private IAccelerometer accelerometer2;

        public LineGraph() {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);

            var samples = 0;

            List<IMetaWearBoard> devices = new List<IMetaWearBoard>();

            devices = e.Parameter as List<IMetaWearBoard>;

            metawear1 = devices[0];
            metawear2 = devices[1];

            //metawear = MbientLab.MetaWear.Win10.Application.GetMetaWearBoard(devices[0] as BluetoothLEDevice); // We already have the board
            accelerometer1 = metawear1.GetModule<IAccelerometer>();
            accelerometer1.Configure(odr: 50f, range: 8f);

            await accelerometer1.PackedAcceleration.AddRouteAsync(source => source.Stream(async data => {
                var value1 = data.Value<Acceleration>();

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {

                    values1_X.Text = value1.X.ToString();
                    values1_Y.Text = value1.Y.ToString();
                    values1_Z.Text = value1.Z.ToString();

                });
            }));

            accelerometer2 = metawear2.GetModule<IAccelerometer>();
            accelerometer2.Configure(odr: 50f, range: 8f);

            await accelerometer2.PackedAcceleration.AddRouteAsync(source => source.Stream(async data => {
                var value2 = data.Value<Acceleration>();

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {

                    values2_X.Text = value2.X.ToString();
                    values2_Y.Text = value2.Y.ToString();
                    values2_Z.Text = value2.Z.ToString();

                });
            }));


        }

        private async void back_Click(object sender, RoutedEventArgs e) {
            if (!metawear1.InMetaBootMode) {
                metawear1.TearDown();
                await metawear1.GetModule<IDebug>().DisconnectAsync();
            }

            if (!metawear2.InMetaBootMode)
            {
                metawear2.TearDown();
                await metawear2.GetModule<IDebug>().DisconnectAsync();
            }

            Frame.GoBack();
        }

        private void streamSwitch_Toggled(object sender, RoutedEventArgs e) {
            if (streamSwitch.IsOn) {
                accelerometer1.Acceleration.Start();
                accelerometer1.Start();

                accelerometer2.Acceleration.Start();
                accelerometer2.Start();
            } else {
                accelerometer1.Stop();
                accelerometer1.Acceleration.Stop();

                accelerometer2.Stop();
                accelerometer2.Acceleration.Stop();
            }
        }
    }
}
