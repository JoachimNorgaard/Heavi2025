using System;
using System.Windows;
using System.IO.Ports;
using System.Text;
using System.Threading;
//using Serilog;
using Timer = System.Timers.Timer;

namespace WeightGameWPF.Helpers
{
    public class Rs232Helper
    {
        private readonly SerialPort _motorSerialPort;
        private readonly SerialPort _weightSerialPort;

        // N: open relay 3
        // n: open relay 4
        // M: close relay 3
        // m: close relay 4
        // J: open relay 1
        // j: open relay 2
        // K: close relay 1
        // k: close relay 2
        public string OpenRelay1 = "J";
        public string CloseRelay1 = "K";
        private const string OpenRelay2 = "j";
        private const string CloseRelay2 = "k";
        private const string OpenRelay3 = "N";
        private const string CloseRelay3 = "M";
        private const string OpenRelay4 = "n";
        private const string CloseRelay4 = "m";

        private string _readBufferWeight = string.Empty;
        private string _readBufferMotor = string.Empty;
        private readonly Timer _motorTimer;

        public class WeightDataChangedEventArgs : EventArgs
        {
            public double Weight { get; set; }
        }
        public delegate void ChangedEventHandler(object sender, WeightDataChangedEventArgs e);
        public event ChangedEventHandler WeightDataRecieved;

        public class MotorDataChangedEventArgs : EventArgs
        {
            public int Data { get; set; }
        }
        public delegate void MotorDataChangedEventHandler(object sender, MotorDataChangedEventArgs e);
        public event MotorDataChangedEventHandler MotorDataChanged;

        public Rs232Helper(string motorComPort, string weightComPort)
        {
            if (_motorSerialPort != null || _weightSerialPort != null)
            {
                throw new Exception("The serial port instances are not null - cannot initialize.");
            }
            _motorSerialPort = new SerialPort
            {
                PortName = motorComPort,
                BaudRate = 9600,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
                Handshake = Handshake.None,
                ReadTimeout = 2000,
                ReadBufferSize = 10000
            };
            //_weightSerialPort = new SerialPort
            //{
            //    PortName = weightComPort,
            //    BaudRate = 9600,
            //    Parity = Parity.None,
            //    DataBits = 8,
            //    StopBits = StopBits.One,
            //    Handshake = Handshake.None,
            //    ReadTimeout = 2000,
            //    ReadBufferSize = 10000
            //};
            _weightSerialPort = new SerialPort
            {
                PortName = weightComPort,
                BaudRate = 9600,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
                Handshake = Handshake.None,
                ReadTimeout = 500,
                ReadBufferSize = 10000
            };

            //_weightSerialPort = new SerialPort
            //{
            //    PortName = weightComPort,
            //    BaudRate = 4800,
            //    Parity = Parity.None,
            //    DataBits = 7,
            //    StopBits = StopBits.One,
            //    Handshake = Handshake.None,
            //    ReadTimeout = 2000,
            //    ReadBufferSize = 10000
            //};
            

            _motorSerialPort.DataReceived += MotorDataReceived;
            _weightSerialPort.DataReceived += WeightDataReceived;

            // ReSharper disable once UseObjectOrCollectionInitializer
            _motorTimer = new Timer();
            _motorTimer.Interval = 100;
            _motorTimer.Elapsed += (sender, args) =>
            {
                PollPort(_motorSerialPort);
            };
            _motorTimer.Enabled = true;
            _motorTimer.Start();
        }

        private void PollPort(SerialPort port)
        {
            WriteToPort(port, "R");
        }

        public void WriteToPort(SerialPort port, string value)
        {
            if (port != null && port.IsOpen)
            {
                port.Write(value);
            }
        }

        private void ReadData(SerialPort port, ref string readBuffer)
        {
            var buffer = new byte[port.BytesToRead];
            var bytesRead = port.Read(buffer, 0, buffer.Length);
            readBuffer += Encoding.ASCII.GetString(buffer, 0, bytesRead);
        }

        private void MotorDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte terminator = 0x3;
            ReadData(_motorSerialPort, ref _readBufferMotor);

            if (_readBufferMotor.IndexOf((char)terminator) > -1)
            {
                var workingString = _readBufferMotor.Substring(0, _readBufferMotor.IndexOf((char)terminator));
                if (workingString.Length == 4)
                {
                    var value = workingString.Substring(0, 4);
                    int beerAtBottom;
                    int.TryParse(value[1].ToString(), out beerAtBottom);
                    MotorDataChangedEventArgs ea = new Helpers.Rs232Helper.MotorDataChangedEventArgs();

                    if (beerAtBottom == 1)
                    {
                        ea.Data = 1;
                        MotorDataChanged(this, ea);
                    }

                    // Changed to index 0 instead of previous "reset game"
                    int startGame;
                    int.TryParse(value[0].ToString(), out startGame);
                    if (startGame == 1)
                    {
                        ea.Data = 2;
                        MotorDataChanged(this, ea);
                    }
                    int wagerAccept;
                    int.TryParse(value[2].ToString(), out wagerAccept);
                    if (wagerAccept == 1)
                    {
                        ea.Data = 3;
                        MotorDataChanged(this, ea);
                        Console.WriteLine(" ACCEPT HIT!!!");
                    }
                    //Console.WriteLine(DateTime.Now.ToUniversalTime()+"  "+value);
                }
                _readBufferMotor = string.Empty;
            }
        }
        private string ReturnLastValidEntry(string[] lines)
        {
            bool found = false;
            int idx = lines.Length - 1;
            while (!found && idx > 0)
            {
                string last = lines[idx];
                var idx1 = last.IndexOf("ST,GS");
                if (idx1 >= 0)
                {
                    var idx2 = last.IndexOf("kg", idx1);
                    if (idx2 > 0)
                        found = true;
                    else
                        idx--;
                }
                else
                    idx--;

            }
            return lines[idx];
        }
        private void WeightDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {  
                ReadData(_weightSerialPort, ref _readBufferWeight);

                if (_readBufferWeight.IndexOf("kg") > -1)
                {
                    var workingString = _readBufferWeight.Substring(0, _readBufferWeight.IndexOf("kg"));
                    //ST,GS,-00001.2kg
                    //Console.WriteLine(workingString);
                    var weight = "";
                    if (workingString.Length == 14)
                    {   
                        weight = workingString.Substring(7, 5); //must be 5???
                                                                //if (weight != "0000=")
                                                                //    Console.WriteLine(weight);
                                                                //weight = weight.TrimStart('0');
                    }
                 
                    // Parse string to int
                    int weightInteger;
                    var parsed = int.TryParse(weight, out weightInteger);
                    if (WeightDataRecieved != null && parsed)
                    {
                        WeightDataChangedEventArgs w = new WeightDataChangedEventArgs();
                        w.Weight = weightInteger;
                        WeightDataRecieved(this, w);
                    }

                    //_gameHelper.UpdateCurrentWeight(parsed ? weightInteger : 0);
                }
                _readBufferWeight = string.Empty;

            }
            catch (Exception ex)
            {
                _readBufferWeight = string.Empty;
                Console.WriteLine(" Weight COM port trouble" + ex.Message);
            }
        }

        public void RaiseBeer()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                for (int i = 0; i < 5; i++)
                {
                    WriteToPort(_motorSerialPort, OpenRelay2);
                    Thread.Sleep(Constants.Instance.MilliSecondsForMotorForEachDrop);
                }
                WriteToPort(_motorSerialPort, CloseRelay2);
            }).Start();
        }


        public void LowerBeerOneStep(bool isWinning)
        {
            Thread.BeginCriticalRegion();
            new Thread(() =>
            {

                Thread.CurrentThread.IsBackground = true;
                WriteToPort(_motorSerialPort, OpenRelay1);
                Thread.Sleep(Constants.Instance.MilliSecondsForMotorForEachDrop);
                if (!isWinning)
                {
                    WriteToPort(_motorSerialPort, CloseRelay1);
                }

            }).Start();
            Thread.EndCriticalRegion();
        }

        public void LowerBeerFromTopToStepBeforeDrop()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                //WriteToPort(_motorSerialPort, OpenRelay1);
                //Thread.Sleep(Constants.Instance.SecondsForMotorForEntireDrop * 1000);
                //WriteToPort(_motorSerialPort, CloseRelay1);
                for (var i = 0; i < 57; i++)
                {
                    WriteToPort(_motorSerialPort, OpenRelay1);
                    Thread.Sleep(1000);
                }
                WriteToPort(_motorSerialPort, CloseRelay1);
            }).Start();
        }


        public void LowerBeerEntirely()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                for (var i = 0; i < 30; i++)
                {
                    WriteToPort(_motorSerialPort, OpenRelay1);
                    Thread.Sleep(Constants.Instance.MilliSecondsForMotorForEachDrop);
                }
                WriteToPort(_motorSerialPort, CloseRelay1);
            }).Start();
        }
        public void DelpoyChips(int seconds)
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                for (int i = 0; i < seconds; i++)
                {

                    WriteToPort(_motorSerialPort, OpenRelay3);
                    Thread.Sleep(1000);
                }

                WriteToPort(_motorSerialPort, CloseRelay3);
            }).Start();
        }

        public void SoundRoundWonHorn()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                WriteToPort(_motorSerialPort, OpenRelay3);
                Thread.Sleep(300);
                WriteToPort(_motorSerialPort, CloseRelay3);
            }).Start();
        }

        public void SoundGameWonHorn()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                WriteToPort(_motorSerialPort, OpenRelay2);
                Thread.Sleep(Constants.Instance.GameWonHornTime * 1000);
                WriteToPort(_motorSerialPort, CloseRelay2);
            }).Start();
        }

        #region Port open/close

        public void OpenPorts()
        {
            try
            {
                _motorSerialPort.Open();
                _weightSerialPort.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message);
                throw e;
            }
        }

        public void ClosePorts()
        {
            try
            {
                if (_motorSerialPort != null)
                {
                    if (_motorSerialPort.IsOpen)
                    {
                        _motorSerialPort.Close();
                        _motorSerialPort.DataReceived -= MotorDataReceived;
                    }
                }
                if (_weightSerialPort != null)
                {
                    if (_weightSerialPort.IsOpen)
                    {
                        _weightSerialPort.Close();
                        _weightSerialPort.DataReceived -= WeightDataReceived;
                    }
                }
            }
            catch (Exception e)
            {
                //Log.Error(e, "Error while closing ports.");
            }
        }

        #endregion // Port open/close

        ~Rs232Helper()
        {
            _motorTimer.Stop();
            ClosePorts();
        }
    }
}
