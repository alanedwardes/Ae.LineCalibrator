using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ae.Mixer
{
    public sealed class AudioDevice : IDisposable
    {
        private float? _lockedVolume;
        private MMDevice _audioDevice;
        private IWaveIn _waveIn;
        private CancellationTokenSource _sampleToken;
        private readonly string _name;

        public delegate void OnDeviceVolumeChanged();
        public event OnDeviceVolumeChanged DeviceVolumeChanged;

        public delegate void OnAudioVolumeSampled();
        public event OnAudioVolumeSampled AudioVolumeSampled;

        public AudioDevice(MMDevice audioDevice)
        {
            _audioDevice = audioDevice;
            _name = $"{_audioDevice.FriendlyName} ({_audioDevice.State})";
        }

        public override string ToString() => _name;

        private void AudioEndpointVolumeChanged(AudioVolumeNotificationData data)
        {
            if (_lockedVolume.HasValue)
            {
                _audioDevice.AudioEndpointVolume.MasterVolumeLevelScalar = _lockedVolume.Value;
            }

            DeviceVolumeChanged?.Invoke();
        }

        public void Dispose()
        {
            StopSamplingAudioVolume();
            StopSamplingDeviceVolume();
        }

        public float AudioVolume => _audioVolume * 100f;

        private float _audioVolume;

        public int DeviceMaxVolume => (int)_audioDevice.AudioEndpointVolume.VolumeRange.MaxDecibels;
        public int DeviceMinVolume => (int)_audioDevice.AudioEndpointVolume.VolumeRange.MinDecibels;

        public int DeviceVolume
        {
            get => (int)MathF.Round(_audioDevice.AudioEndpointVolume.MasterVolumeLevel);
            set => _audioDevice.AudioEndpointVolume.MasterVolumeLevel = value;
        }

        public void LockVolume()
        {
            _lockedVolume = _audioDevice.AudioEndpointVolume.MasterVolumeLevelScalar;
        }

        public void UnlockVolume()
        {
            _lockedVolume = null;
        }

        public void StartSamplingDeviceVolume()
        {
            _audioDevice.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolumeChanged;
            DeviceVolumeChanged?.Invoke();
        }

        public async void StartSamplingAudioVolume()
        {
            StopSamplingAudioVolume();

            AudioVolumeSampled?.Invoke();

            if (_audioDevice.State != DeviceState.Active)
            {
                return;
            }

            _waveIn = new WasapiCapture(_audioDevice)
            {
                WaveFormat = new WaveFormat(_audioDevice.AudioClient.MixFormat.SampleRate, _audioDevice.AudioClient.MixFormat.Channels)
            };
            _waveIn.DataAvailable += WaveInDataAvailable;
            _waveIn.StartRecording();

            _sampleToken = new CancellationTokenSource();

            try
            {
                await Task.Delay(-1, _sampleToken.Token);
            }
            catch (TaskCanceledException)
            {
                // Expected when the wait is cancelled
            }
        }

        public void StopSamplingDeviceVolume()
        {
            _audioDevice.AudioEndpointVolume.OnVolumeNotification -= AudioEndpointVolumeChanged;
        }

        public void StopSamplingAudioVolume()
        {
            if (_waveIn != null)
            {
                _sampleToken.Cancel();
                _waveIn.DataAvailable -= WaveInDataAvailable;
                _waveIn.StopRecording();
                _waveIn.Dispose();
            }
        }

        private void WaveInDataAvailable(object sender, WaveInEventArgs args)
        {
            float max = 0;
            // interpret as 16 bit audio
            for (int index = 0; index < args.BytesRecorded; index += 2)
            {
                short sample = (short)((args.Buffer[index + 1] << 8) |
                                        args.Buffer[index + 0]);
                // to floating point
                var sample32 = sample / 32768f;
                // absolute value 
                if (sample32 < 0) sample32 = -sample32;
                // is this the max value?
                if (sample32 > max) max = sample32;
            }

            _audioVolume = max;
            AudioVolumeSampled?.Invoke();
        }
    }
}
