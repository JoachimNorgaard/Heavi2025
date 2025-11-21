using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace SerialReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SerialPort _weightSerialPort;
        private DispatcherTimer _timer;
        private DispatcherTimer _uiBatchTimer;
		private readonly ConcurrentQueue<string> _lineQueue = new ConcurrentQueue<string>();
		private readonly StringBuilder _incomingBuffer = new StringBuilder(4096);
		private readonly object _bufferLock = new object();

        public MainWindow()
        {
            Debug.WriteLine("Started serial reader");
            InitializeComponent();
            InitializeSerialPort();
            InitializeTimer();
        }

        private void InitializeSerialPort()
        {
            try
            {
                _weightSerialPort = new SerialPort("COM3", 9600);
                // Set other optional properties
                _weightSerialPort.Parity = Parity.None;
                _weightSerialPort.StopBits = StopBits.One;
                _weightSerialPort.DataBits = 8;
                _weightSerialPort.Handshake = Handshake.None;
                _weightSerialPort.RtsEnable = true;
                _weightSerialPort.DtrEnable = true;
                _weightSerialPort.NewLine = "\r\n"; // match device terminator
                _weightSerialPort.ReadBufferSize = 65536; // larger buffer to reduce overruns
                _weightSerialPort.WriteBufferSize = 16384;
                _weightSerialPort.ReadTimeout = -1; // non-blocking via DataReceived
                _weightSerialPort.Encoding = Encoding.ASCII;
                _weightSerialPort.ReceivedBytesThreshold = 1;

                _weightSerialPort.DataReceived += SerialPort_DataReceived;
                _weightSerialPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening COM port: {ex.Message}");
            }
        }

        private void InitializeTimer()
        {
            _timer = new DispatcherTimer();
            // Set the interval to 100 milliseconds.
            _timer.Interval = TimeSpan.FromMilliseconds(100);
            // Link the timer's Tick event to a method.
            _timer.Tick += timer_Tick;
        }

		private void InitializeUiBatchTimer()
		{
			_uiBatchTimer = new DispatcherTimer();
			_uiBatchTimer.Interval = TimeSpan.FromMilliseconds(50);
			_uiBatchTimer.Tick += (s, e) =>
			{
				var sb = new StringBuilder();
				int drained = 0;
				while (_lineQueue.TryDequeue(out var line))
				{
					sb.AppendLine(line);
					drained++;
					// Prevent huge UI updates in one tick
					if (drained >= 2000) break;
				}
				if (drained > 0)
				{
					OutputTextBox.AppendText(sb.ToString());
					OutputTextBox.ScrollToEnd();
					OutputTextBox2.Text = drained.ToString();
				}
			};
			_uiBatchTimer.Start();
		}

        int kg = 10;
        private void timer_Tick(object sender, EventArgs e)
        {            
            if (kg < 100)
                kg++;
            else
                kg = 10;

            string data = "US,GS,+000" + kg.ToString() + ".5kg\r\n";

            try
            {                
                _weightSerialPort.WriteLine(data);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error writing COM port: {ex.Message}");
            }
        }

        int numberOfReceives = 0;

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                // Drain all available bytes quickly into a local buffer
                while (_weightSerialPort.BytesToRead > 0)
                {
                    int bytesAvailable = _weightSerialPort.BytesToRead;
                    if (bytesAvailable <= 0) break;
                    byte[] buffer = new byte[bytesAvailable];
                    int bytesRead = _weightSerialPort.Read(buffer, 0, bytesAvailable);
                    if (bytesRead <= 0) break;

                    string chunk = _weightSerialPort.Encoding.GetString(buffer, 0, bytesRead);
                    lock (_bufferLock)
                    {
                        _incomingBuffer.Append(chunk);

                        // Extract complete lines using \n as delimiter, trimming optional \r
                        int newlineIndex;
                        while ((newlineIndex = _incomingBuffer.ToString().IndexOf('\n')) >= 0)
                        {
                            string line = _incomingBuffer.ToString(0, newlineIndex);
                            // Remove the extracted line and the delimiter
                            _incomingBuffer.Remove(0, newlineIndex + 1);
                            if (line.EndsWith("\r")) line = line[..^1];
                            _lineQueue.Enqueue(line);
                            numberOfReceives++;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Swallow serial read exceptions to keep stream alive; UI thread can show alerts if needed
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_weightSerialPort?.IsOpen == true)
            {
                _weightSerialPort.DataReceived -= SerialPort_DataReceived;
                _weightSerialPort.Close();
            }
            _uiBatchTimer?.Stop();
            base.OnClosed(e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Data to write
            string dataToWrite = InputTextBox.Text;

            // Write the data to the port
            _weightSerialPort.WriteLine(dataToWrite);

            Dispatcher.Invoke(() =>
            {
                OutputTextBox.AppendText(InputTextBox.Text);
                OutputTextBox.ScrollToEnd();
            });
        }

        private void MyButtonTimer_Click(object sender, RoutedEventArgs e)
        {
            if (_timer.IsEnabled)
            {
                _timer.Stop();
                MyButtonTimer.Content = "Start";                
            }
            else
            {
                _timer.Start();
                Debug.WriteLine("Started reader");
                MyButtonTimer.Content = "Stop";
            }
        }

		// Ensure UI batch timer starts when window is ready
		protected override void OnContentRendered(EventArgs e)
		{
			base.OnContentRendered(e);
			InitializeUiBatchTimer();
		}
    }
}