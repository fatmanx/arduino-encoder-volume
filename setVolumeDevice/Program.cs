using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace setVolumeDevice
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length == 1)
            {
                var deviceName = args[0];
                getVol(deviceName);
                return;

            }
            if (args.Length == 2)
            {
                var deviceName = args[0];
                var level = Int32.Parse(args[1]);

                setVol(deviceName, level);
            }

        }


        static void getVol(string idString)
        {
            try
            {
                NAudio.CoreAudioApi.MMDeviceEnumerator MMDE = new NAudio.CoreAudioApi.MMDeviceEnumerator();
                NAudio.CoreAudioApi.MMDeviceCollection DevCol = MMDE.EnumerateAudioEndPoints(NAudio.CoreAudioApi.DataFlow.All, NAudio.CoreAudioApi.DeviceState.All);

                foreach (NAudio.CoreAudioApi.MMDevice dev in DevCol)
                {
                    if (dev.FriendlyName.ToLowerInvariant().Contains(idString.ToLowerInvariant()))
                    {
                        if (dev.State == NAudio.CoreAudioApi.DeviceState.Active)
                        {
                            Console.WriteLine((int)(100 * dev.AudioEndpointVolume.MasterVolumeLevelScalar));
                        }
                    }
                }
            }
            catch { }
        }


        static void setVol(string idString, int level)
        {

            try
            {
                NAudio.CoreAudioApi.MMDeviceEnumerator MMDE = new NAudio.CoreAudioApi.MMDeviceEnumerator();
                NAudio.CoreAudioApi.MMDeviceCollection DevCol = MMDE.EnumerateAudioEndPoints(NAudio.CoreAudioApi.DataFlow.All, NAudio.CoreAudioApi.DeviceState.All);

                foreach (NAudio.CoreAudioApi.MMDevice dev in DevCol)
                {
                    if (dev.FriendlyName.ToLowerInvariant().Contains(idString.ToLowerInvariant()))
                    {
                        if (dev.State == NAudio.CoreAudioApi.DeviceState.Active)
                        {
                            var newVolume = (float)Math.Max(Math.Min(level, 100), 0) / (float)100;

                            //Set at maximum volume
                            dev.AudioEndpointVolume.MasterVolumeLevelScalar = newVolume;

                        }

                    }
                }
            }
            catch { }
        }
    }
}
