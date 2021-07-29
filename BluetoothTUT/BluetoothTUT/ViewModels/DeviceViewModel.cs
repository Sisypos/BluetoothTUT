using Plugin.BLE.Abstractions.Contracts;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BluetoothTUT.ViewModels
{
    public class DeviceViewModel:INotifyPropertyChanged
    {
        private IDevice _nativeDevice;

        public event PropertyChangedEventHandler PropertyChanged;
        public IDevice NativeDevice
        {
            get
            {
                return _nativeDevice;
            }
            set 
            {
                _nativeDevice = value;
                RaisePropertyChanged();
            }
        }

        protected void RaisePropertyChanged([CallerMemberName] string caller = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(caller));
        }

    }
}
