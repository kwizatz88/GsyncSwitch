using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Power;
using Windows.Gaming.Input;
using Windows.Gaming.UI;

namespace GsyncSwitch
{
    internal class Peripherals
    {
        private Gamepad _gamepad;
        const string fileNameDAHTwaveFormat = "dolbyAtmosDefaultWaveFormat.dat";

        private void Gamepad_GamepadAdded(object sender, Gamepad e)
        {
            var controller = Gamepad.Gamepads?.First();
            var reading = controller.GetCurrentReading();
        }
        public int GetControllerBatteryLevel()
        {
            Gamepad.GamepadAdded += Gamepad_GamepadAdded;
            var gamepads = Gamepad.Gamepads;
            if (gamepads.Count == 0)
            {
                return -1;
            }

            Gamepad gamepad = gamepads.First();

            if (gamepad != null)
            {
                BatteryReport batteryReport = gamepad.TryGetBatteryReport();
                if (batteryReport != null)
                {
                    int? remainingCapacity = batteryReport.RemainingCapacityInMilliwattHours;
                    int? fullCapacity = batteryReport.FullChargeCapacityInMilliwattHours;

                    if (fullCapacity > 0)
                    {
                        double batteryLevel = (double)remainingCapacity / (double)fullCapacity;
                        int batteryLevelPercent = (int)Math.Round(batteryLevel * 100);
                        return batteryLevelPercent;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }

        }

        public int GetBatteryLevel(string peripheralName)
        {
            var query = new SelectQuery("SELECT * FROM Win32_Battery");
            var searcher = new ManagementObjectSearcher(query);

            foreach (var battery in searcher.Get())
            {
                string batteryName = battery["Name"].ToString();
                if (batteryName.Contains(peripheralName))
                {
                    int estimatedChargeRemaining = Convert.ToInt32(battery["EstimatedChargeRemaining"]);
                    return estimatedChargeRemaining;
                }
            }

            // peripheralName not found or battery information not available
            return -1;
        }


        public void debugSpeakerStatus()
        {
            var deviceEnum = new MMDeviceEnumerator();
            var devices = deviceEnum.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
            foreach (MMDevice device in devices)
            {
                System.Diagnostics.Debug.WriteLine("device.FriendlyName: " + device.FriendlyName.ToString());
                System.Diagnostics.Debug.WriteLine("device.State: " + device.State.ToString());
                System.Diagnostics.Debug.WriteLine("device.AudioClient: " + device.AudioClient.ToString());


                if (device.FriendlyName.Contains("EP-HDMI"))
                {
                    if (device.Properties.Contains(PropertyKeys.PKEY_AudioEndpoint_Association))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_AudioEndpoint_Association];
                        System.Diagnostics.Debug.WriteLine("PKEY_AudioEndpoint_Association: " + value.Value.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_AudioEndpoint_ControlPanelPageProvider))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_AudioEndpoint_ControlPanelPageProvider];
                        System.Diagnostics.Debug.WriteLine("PKEY_AudioEndpoint_ControlPanelPageProvider: " + value.Value.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_AudioEndpoint_Disable_SysFx))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_AudioEndpoint_Disable_SysFx];
                        System.Diagnostics.Debug.WriteLine("PKEY_AudioEndpoint_Disable_SysFx: " + value.Value.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_AudioEndpoint_FormFactor))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_AudioEndpoint_FormFactor];
                        System.Diagnostics.Debug.WriteLine("PKEY_AudioEndpoint_FormFactor: " + value.Value.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_AudioEndpoint_FullRangeSpeakers))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_AudioEndpoint_FullRangeSpeakers];
                        System.Diagnostics.Debug.WriteLine("PKEY_AudioEndpoint_FullRangeSpeakers: " + value.Value.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_AudioEndpoint_GUID))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_AudioEndpoint_GUID];
                        System.Diagnostics.Debug.WriteLine("PKEY_AudioEndpoint_GUID: " + value.Value.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_AudioEndpoint_JackSubType))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_AudioEndpoint_JackSubType];
                        System.Diagnostics.Debug.WriteLine("PKEY_AudioEndpoint_JackSubType: " + value.Value.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_AudioEndpoint_PhysicalSpeakers))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_AudioEndpoint_PhysicalSpeakers];
                        System.Diagnostics.Debug.WriteLine("PKEY_AudioEndpoint_PhysicalSpeakers: " + value.Value.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_AudioEndpoint_Supports_EventDriven_Mode))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_AudioEndpoint_Supports_EventDriven_Mode];
                        System.Diagnostics.Debug.WriteLine("PKEY_AudioEndpoint_Supports_EventDriven_Mode: " + value.Value.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_AudioEngine_DeviceFormat))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_AudioEngine_DeviceFormat].Value as byte[];

                        IntPtr unmanagedPointer = Marshal.AllocHGlobal(value.Length);
                        Marshal.Copy(value, 0, unmanagedPointer, value.Length);
                        Marshal.FreeHGlobal(unmanagedPointer);
                        var waveFormat = WaveFormat.MarshalFromPtr(unmanagedPointer);

                        System.Diagnostics.Debug.WriteLine("PKEY_AudioEngine_DeviceFormat: " + waveFormat.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_AudioEngine_OEMFormat))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_AudioEngine_OEMFormat].Value as byte[];

                        IntPtr unmanagedPointer = Marshal.AllocHGlobal(value.Length);
                        Marshal.Copy(value, 0, unmanagedPointer, value.Length);
                        Marshal.FreeHGlobal(unmanagedPointer);
                        var waveFormat = WaveFormat.MarshalFromPtr(unmanagedPointer);

                        System.Diagnostics.Debug.WriteLine("PKEY_AudioEngine_OEMFormat: " + waveFormat.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_DeviceInterface_FriendlyName))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_DeviceInterface_FriendlyName];
                        System.Diagnostics.Debug.WriteLine("PKEY_DeviceInterface_FriendlyName: " + value.Value.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_Device_ControllerDeviceId))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_Device_ControllerDeviceId];
                        System.Diagnostics.Debug.WriteLine("PKEY_Device_ControllerDeviceId: " + value.Value.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_Device_DeviceDesc))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_Device_DeviceDesc];
                        System.Diagnostics.Debug.WriteLine("PKEY_Device_DeviceDesc: " + value.Value.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_Device_FriendlyName))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_Device_FriendlyName];
                        System.Diagnostics.Debug.WriteLine("PKEY_Device_FriendlyName: " + value.Value.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_Device_IconPath))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_Device_IconPath];
                        System.Diagnostics.Debug.WriteLine("PKEY_Device_IconPath: " + value.Value.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_Device_InstanceId))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_Device_InstanceId];
                        System.Diagnostics.Debug.WriteLine("PKEY_Device_InstanceId: " + value.Value.ToString());
                    }
                    if (device.Properties.Contains(PropertyKeys.PKEY_Device_InterfaceKey))
                    {
                        var value = device.Properties[PropertyKeys.PKEY_Device_InterfaceKey];
                        System.Diagnostics.Debug.WriteLine("PKEY_Device_InterfaceKey: " + value.Value.ToString());
                    }

                }

            }

        }

        public void setFormatToDolbyAtmosForHomeTheater(MMDevice currentDevice)
        {
            WaveFormat waveFormat = null;

            using (var stream = File.Open(fileNameDAHTwaveFormat, FileMode.Open))
            {
                if (stream != null)
                {
                    using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                    {
                        waveFormat = new WaveFormat(reader);
                    }
                }
            }

            if (waveFormat != null)
            {
                PropVariant p = new PropVariant();

                IntPtr formatPointer = Marshal.AllocHGlobal(Marshal.SizeOf(waveFormat));
                Marshal.StructureToPtr(waveFormat, formatPointer, false);
                p.pointerValue = formatPointer;

                currentDevice.GetPropertyInformation(StorageAccessMode.ReadWrite);
                currentDevice.Properties.SetValue(PropertyKeys.PKEY_AudioEngine_DeviceFormat, p);
            }
        }

    }
}
