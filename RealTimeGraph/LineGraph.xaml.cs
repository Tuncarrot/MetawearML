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
using Windows.System.Threading;
using Windows;

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

        public DispatcherTimer dispatcherTimer;

        public SensorData RW;
        public SensorData LW;

        public LineGraph() {
            InitializeComponent();
            Logger.StartLogger("SensorDataRecord");

            dispatcherTimer = new DispatcherTimer();

            RW = new SensorData();
            LW = new SensorData();
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
                 
                    values1_X.Text = "X" + value1.X.ToString();
                    values1_Y.Text = "Y" + value1.Y.ToString();
                    values1_Z.Text = "Z" + value1.Z.ToString();

                    if (RW.RecordData)
                    {
                        Logger.LogMsg(value1.X.ToString());
                       RW.X_Value.Add(value1.X);
                       RW.Y_Value.Add(value1.Y);
                       RW.Z_Value.Add(value1.Z);
                    }

                });
            }));

            accelerometer2 = metawear2.GetModule<IAccelerometer>();
            accelerometer2.Configure(odr: 50f, range: 8f);

            await accelerometer2.PackedAcceleration.AddRouteAsync(source => source.Stream(async data => {
                var value2 = data.Value<Acceleration>();

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {

                    values2_X.Text = "X" + value2.X.ToString();
                    values2_Y.Text = "Y" + value2.Y.ToString();
                    values2_Z.Text = "Z" + value2.Z.ToString();

                    if (LW.RecordData)
                    {
                        Logger.LogMsg(value2.X.ToString());
                        LW.X_Value.Add(value2.X);   //Record Data //make class
                        LW.Y_Value.Add(value2.Y);
                        LW.Z_Value.Add(value2.Z);
                    }

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

        private void Button_Click(object sender, RoutedEventArgs e) // Start
        {
            dispatcherTimer.Tick += Timer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 3);
            dispatcherTimer.Start();
            RW.RecordData = true;
            LW.RecordData = true;
            Counter.Text = "RECORDING";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) // Stop
        {
            RW.RecordData = false;
            LW.RecordData = false;
            Counter.Text = "STOP";
            dispatcherTimer.Stop();
        }

        private void Timer_Tick(object sender, object e)
        {
            RW.RecordData = false;  // Stop recording
            LW.RecordData = false;
            Counter.Text = "STOP";

            string RW_X = "";
            string RW_Y = "";
            string RW_Z = "";

            string LW_X = "";
            string LW_Y = "";
            string LW_Z = "";

            foreach (float value in RW.X_Value)
            {
                RW_X += value + " ";
            }

            foreach (float value in RW.Y_Value)
            {
                RW_Y += value + " ";
            }

            foreach (float value in RW.Z_Value)
            {
                RW_Z += value + " ";
            }

            foreach (float value in LW.X_Value)
            {
                LW_X += value + " ";
            }

            foreach (float value in LW.Y_Value)
            {
                LW_Y += value + " ";
            }

            foreach (float value in LW.Z_Value)
            {
                LW_Z += value + " ";
            }

            Logger.LogMsg(RW_X + "," + RW_Y + "," + RW_Z + "," + LW_X + "," + LW_Y + "," + LW_Z + "," + exerciseName.Text);
        }
    }
}
