﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Ae.LineCalibrator.Audio;
using Avalonia.Media;

namespace Ae.LineCalibrator.Interface
{
    public sealed class LineCalibratorViewModel : INotifyPropertyChanged, IDisposable
    {
        private AudioDeviceRepository _audioDeviceRepository = new AudioDeviceRepository();

        public LineCalibratorViewModel()
        {
            MeasureClippingCommand = new SimpleCommand(() => MeasureClipping());
            InputDevices = _audioDeviceRepository.GetCaptureDevices().ToArray();
            if (InputDevices.Any())
            {
                SelectedInputDevice = InputDevices.FirstOrDefault();
            }
        }

        public ICommand MeasureClippingCommand { get; }

        private async void MeasureClipping()
        {
            var startVolume = DeviceVolume;

            IsMeasuringClipping = true;
            RaisePropertyChanged(nameof(IsEnabled));
            RaisePropertyChanged(nameof(IsMeasuringClipping));
            RaisePropertyChanged(nameof(MeasureClippingText));

            await Task.Delay(TimeSpan.FromSeconds(5));

            if (startVolume != DeviceVolume)
            {
                DeviceVolume += 1;
            }

            IsMeasuringClipping = false;
            RaisePropertyChanged(nameof(IsEnabled));
            RaisePropertyChanged(nameof(IsMeasuringClipping));
            RaisePropertyChanged(nameof(MeasureClippingText));
        }

        public bool IsEnabled => !IsMeasuringClipping;
        public bool IsMeasuringClipping { get; private set; }

        public float AudioVolume => SelectedInputDevice?.AudioVolume ?? 0f;
        public IBrush SliderColor
        {
            get
            {
                if (AudioVolume > 95)
                {
                    return Brushes.Red;
                }
                else if (AudioVolume > 80)
                {
                    return Brushes.Orange;
                }

                return Brushes.Green;
            }
        }

        public string MeasureClippingText
        {
            get
            {
                if (IsMeasuringClipping)
                {
                    return "Measuring Clipping - Please Generate a Loud Signal";
                }

                return "Reduce Volume to Safe Levels";
            }
        }

        public int DeviceMaxVolume => SelectedInputDevice?.DeviceMaxVolume ?? 0;
        public int DeviceMinVolume => SelectedInputDevice?.DeviceMinVolume ?? 0;

        public int DeviceVolume
        {
            get => SelectedInputDevice?.DeviceVolume ?? 0;
            set
            {
                if (SelectedInputDevice != null)
                {
                    SelectedInputDevice.DeviceVolume = value;
                }
                RaisePropertyChanged(nameof(DeviceVolume));
            }
        }

        private bool _isLocked;
        public bool IsLocked
        {
            get => _isLocked;
            set => IsLockedChanged(value);
        }

        private void IsLockedChanged(bool newIsLocked)
        {
            _isLocked = newIsLocked;
            if (_isLocked)
            {
                SelectedInputDevice.LockVolume();
            }
            else
            {
                SelectedInputDevice.UnlockVolume();
            }
            RaisePropertyChanged(nameof(IsLocked));
            RaisePropertyChanged(nameof(IsNotLocked));
        }

        public bool IsNotLocked => !IsLocked;

        private AudioDevice _selectedInputDevice;
        public AudioDevice SelectedInputDevice
        {
            get => _selectedInputDevice;
            set => AudioDeviceChanged(value);
        }

        private void AudioDeviceChanged(AudioDevice newDevice)
        {
            if (newDevice == null)
            {
                return;
            }

            SelectedInputDevice?.StopSamplingDeviceVolume();
            SelectedInputDevice?.StopSamplingAudioVolume();
            _selectedInputDevice = newDevice;

            SelectedInputDevice.AudioVolumeSampled += () =>
            {
                RaisePropertyChanged(nameof(SliderColor));
                RaisePropertyChanged(nameof(AudioVolume));
                CheckClipping();
            };

            SelectedInputDevice.DeviceVolumeChanged += () =>
            {
                RaisePropertyChanged(nameof(DeviceVolume));
            };

            RaisePropertyChanged(nameof(DeviceMinVolume));
            RaisePropertyChanged(nameof(DeviceMaxVolume));
            SelectedInputDevice.StartSamplingDeviceVolume();
            SelectedInputDevice.StartSamplingAudioVolume();
        }

        private void CheckClipping()
        {
            if (IsMeasuringClipping && MathF.Round(AudioVolume) == 100)
            {
                DeviceVolume -= 1;
                RaisePropertyChanged(nameof(DeviceVolume));
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            foreach (var device in InputDevices)
            {
                device.Dispose();
            }
        }

        public AudioDevice[] InputDevices { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
