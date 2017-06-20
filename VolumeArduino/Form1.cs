using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VolumeArduino
{
    public partial class Form1 : Form
    {
        const string SETTINGS_SECTION = "main";
        const string SETTINGS_DEVICE_NAME = "device";
        const string SETTINGS_SERIAL_NAME = "serial";

        public Form1()
        {
            InitializeComponent();
            Application.ApplicationExit += Application_ApplicationExit;
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {

                serialPort1.Close();
            }
        }

        XmlSettings.Settings setts = new XmlSettings.Settings("settings.xml");
        private void Form1_Load(object sender, EventArgs e)
        {


            



            var serialName = setts.GetValue(SETTINGS_SECTION, SETTINGS_SERIAL_NAME);

            var serials = SerialPort.GetPortNames();
            comboBox1.DataSource = serials;
            if (serials.Contains(serialName))
            {
                comboBox1.SelectedItem = serialName;
            }

            var devName = setts.GetValue(SETTINGS_SECTION, SETTINGS_DEVICE_NAME);



            try
            {
                NAudio.CoreAudioApi.MMDeviceEnumerator MMDE = new NAudio.CoreAudioApi.MMDeviceEnumerator();
                NAudio.CoreAudioApi.MMDeviceCollection DevCol = MMDE.EnumerateAudioEndPoints(NAudio.CoreAudioApi.DataFlow.All, NAudio.CoreAudioApi.DeviceState.All);
                foreach (NAudio.CoreAudioApi.MMDevice dev in DevCol)
                {
                    comboBox2.Items.Add(dev.FriendlyName);
                }

                if (!string.IsNullOrWhiteSpace(devName))
                {
                    comboBox2.SelectedItem = devName;
                    comboBox2_SelectionChangeCommitted(comboBox2, new EventArgs());


                }

            }
            catch { }

            try
            {
                if (foundDevice == null)
                {
                    setDevice();
                }

                serialPort1.PortName = comboBox1.SelectedItem.ToString();
                serialPort1.Open();
                timer1.Enabled = true;


            }
            catch
            {
                Console.WriteLine("Exception");
            }

            SendWithDelay();

        }

        async Task SendWithDelay()
        {
            await Task.Delay(100);
            Hide();
            WindowState = FormWindowState.Normal;
        }

        void setDevice()
        {
            NAudio.CoreAudioApi.MMDeviceEnumerator MMDE = new NAudio.CoreAudioApi.MMDeviceEnumerator();
            NAudio.CoreAudioApi.MMDeviceCollection DevCol = MMDE.EnumerateAudioEndPoints(NAudio.CoreAudioApi.DataFlow.All, NAudio.CoreAudioApi.DeviceState.All);
            foreach (NAudio.CoreAudioApi.MMDevice dev in DevCol)
            {
                Console.WriteLine(" {0}     {1}", dev.FriendlyName.ToLowerInvariant(), deviceName.ToLowerInvariant());
                if (dev.FriendlyName.ToLowerInvariant().Contains(deviceName.ToLowerInvariant()))
                {
                    if (dev.State == NAudio.CoreAudioApi.DeviceState.Active)
                    {
                        //Console.WriteLine(dev);
                        foundDevice = dev.AudioEndpointVolume;
                        break;
                        //Console.WriteLine((int)(100 * dev.AudioEndpointVolume.MasterVolumeLevelScalar));

                    }
                }

            }

        }

        string deviceName = "boxe";

        private void button1_Click(object sender, EventArgs e)
        {
            //serialPort1.PortName = comboBox1.SelectedItem.ToString();
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var data = serialPort1.ReadExisting();
            if (data.Length > 0)
            {
                var val = (byte)data.Last();
                //Console.WriteLine(">> {0}", val);
                setVol(deviceName, val);
            }

        }

        private void serialPort1_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {

        }

        private void serialPort1_PinChanged(object sender, SerialPinChangedEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                //serialPort1.Write(new byte[1] { (byte)getVol(deviceName) }, 0, 1);
            }

        }


        int getVol(string idString)
        {
            try
            {

                if (foundDevice != null)
                {
                    return (int)(100 * foundDevice.MasterVolumeLevelScalar);
                }

            }
            catch { }
            return 0;
        }

        NAudio.CoreAudioApi.AudioEndpointVolume foundDevice = null;

        void setVol(string idString, int level)
        {

            try
            {
                if (foundDevice != null)
                {
                    float newVolume = (float)Math.Max(Math.Min(level, 100), 0) / (float)100;
                    foundDevice.MasterVolumeLevelScalar = newVolume;
                }

            }
            catch { }
        }

        private void comboBox2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                deviceName = comboBox2.SelectedItem.ToString();
                setDevice();
                setts.SetValue(SETTINGS_SECTION, SETTINGS_DEVICE_NAME, deviceName);
            }
            catch { }
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)
                {
                    serialPort1.Close();
                }
                serialPort1.PortName = comboBox1.SelectedItem.ToString();
                setts.SetValue(SETTINGS_SECTION, SETTINGS_SERIAL_NAME, serialPort1.PortName);
                serialPort1.Open();
            }
            catch
            {
                Console.WriteLine("Exception serial");
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!quitApp)
            {
                e.Cancel = true;
                Hide();
            }
        }

        bool quitApp = false;
        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            quitApp = true;
            Application.Exit();
        }

        private void quitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            quitApp = true;
            Application.Exit();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            Show();
        }
    }
}
