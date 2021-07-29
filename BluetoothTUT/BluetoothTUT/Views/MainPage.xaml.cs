using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using BluetoothTUT.ViewModels;
using System.Collections.Generic;

namespace BluetoothTUT
{
    public partial class MainPage : ContentPage
    {
        IBluetoothLE ble;
        IAdapter adapter;
        ObservableCollection<IDevice> deviceList;
        IDevice device;
        
        
        public MainPage()
        {
            InitializeComponent();
            ble = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;
            deviceList = new ObservableCollection<IDevice>();
            lv.ItemsSource = deviceList;
        }

        private void btnStatus_Clicked(object sender, EventArgs e)
        {
            var state = ble.State;
            this.DisplayAlert("Notice",state.ToString(),"OK !");
            if(state==BluetoothState.Off)
            {
                //this.DisplayAlert("Notice","please switch on Bluetooth","OK !");
            }
        }

        private async void btnConnect_Clicked(object sender, EventArgs e)
        {
            try 
            {
                if (device != null)
                {
                    await adapter.ConnectToDeviceAsync(device);
                } else
                {
                    DisplayAlert("Notice","No device selected","OK !");
                }

            }
            catch(DeviceConnectionException ex)
            {
                DisplayAlert("Notice", ex.Message.ToString(), "Error !");
            }
        }

        private async void btnScan_Clicked(object sender, EventArgs e)
        {   try
            {
                deviceList.Clear();
                adapter.DeviceDiscovered += (s, a) =>
                {
                    deviceList.Add(a.Device);
                };

                if (!ble.Adapter.IsScanning)
                {
                    await adapter.StartScanningForDevicesAsync();
                }
            }
            catch(Exception ex) 
            {
                DisplayAlert("Notice", ex.Message.ToString(), "Error !");
            }
        }

        private async void btnKnowConnect_Clicked(object sender, EventArgs e)
        {
            try 
            {
                await adapter.ConnectToKnownDeviceAsync(new Guid("guid"));
            }
            catch (DeviceConnectionException ex)
            {
                DisplayAlert("Notice", ex.Message.ToString(), "Error !");
            }
        }

        IList<IService> Servisces;
        IService Service;
        private async void btnGetServises_Clicked(object sender, EventArgs e)
        {
            Servisces = (IList<IService>)await device.GetServicesAsync();
            Service = await device.GetServiceAsync(device.Id);
        }

        IList<ICharacteristic> Characteristics;
        ICharacteristic Characteristic;
        private async void btnGetCharacters_Clicked(object sender, EventArgs e)
        {
            var characteristicts = await Service.GetCharacteristicsAsync();
            Guid idGuid = Guid.Parse("guid");
            Characteristic = await Service.GetCharacteristicAsync(idGuid);
        }

        IList<IDescriptor> descriptors;
        IDescriptor descriptor;

        private async void btnDiscriptor_Clicked(object sender, EventArgs e)
        {
            descriptors = (IList<IDescriptor>)await Characteristic.GetDescriptorsAsync();
            descriptor = await Characteristic.GetDescriptorAsync(Guid.Parse("guid"));
        }

        private async void btnDescRW_Clicked(object sender, EventArgs e)
        {
            var bytes = await descriptor.ReadAsync();
            await descriptor.WriteAsync(bytes);
        }

        private async void btnGetRW_Clicked(object sender, EventArgs e)
        {
            var bytes = await Characteristic.ReadAsync();
            await Characteristic.WriteAsync(bytes);
        }

        private async void btnUpdate_Clicked(object sender, EventArgs e)
        {
            Characteristic.ValueUpdated += (o, args) =>
            {
                var bytes = args.Characteristic.Value;
            };
            await Characteristic.StartUpdatesAsync();
        }
        private void lv_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if(lv.SelectedItem == null)
            {
                return;
            }
            device = lv.SelectedItem as IDevice;
        }
    }
}
