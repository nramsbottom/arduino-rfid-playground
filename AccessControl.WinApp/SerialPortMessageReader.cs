using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace AccessControl.WinApp
{
    internal class SerialPortMessageReader
    {
        private SerialPort _serialPort;
        private StreamReader _reader;
        private Thread _readThread;

        public event EventHandler<string> MessageReceived;

        private readonly string _portName;
        private readonly int _baudRate;

        public SerialPortMessageReader(string portName, int baudRate)
        {
            _portName = portName;
            _baudRate = baudRate;
        }

        public void Start()
        {
            _serialPort = new SerialPort();
            _serialPort.PortName = _portName;
            _serialPort.BaudRate = _baudRate;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.Open();

            // start reader thread
            _reader = new StreamReader(_serialPort.BaseStream, Encoding.ASCII);
            _readThread = new Thread(new ParameterizedThreadStart(ReadThreadProc));
            _readThread.Start(this);
        }

        private static void ReadThreadProc(object state)
        {
            var reader = state as SerialPortMessageReader;

            while (true)
            {
                try
                {
                    var message = reader._reader.ReadLine();
                    if (message == null)
                    {
                        return;
                    }

                    reader.MessageReceived?.Invoke(reader, message);
                }
                catch (IOException ex)
                {
                    if (ex.HResult != -2147023901)
                    {
                        throw ex;
                    }

                    return;
                }
                catch (ObjectDisposedException)
                {
                    return; // closing port
                }
            }
        }

        public void Stop()
        {
            if (_serialPort == null)
            {
                return;
            }

            _serialPort.Close();
            _serialPort.Dispose();
        }
    }

}
