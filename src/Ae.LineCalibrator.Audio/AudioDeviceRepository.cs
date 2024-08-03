using NAudio.CoreAudioApi;
using System.Collections.Generic;
using System.Linq;

namespace Ae.LineCalibrator.Audio
{
    public sealed class AudioDeviceRepository
    {
        public IEnumerable<AudioDevice> GetCaptureDevices()
        {
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            foreach (MMDevice device in enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).OrderByDescending(x => x.State == DeviceState.Active))
            {
                yield return new AudioDevice(device);
            }
        }
    }
}
