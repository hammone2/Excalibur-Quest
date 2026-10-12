using UnityEngine;
//using System.IO.Ports;
//using System.Threading;

public class ArduinoInput : MonoBehaviour
{
    /*[SerializeField] private string portName = "COM3";
    [SerializeField] private int baudRate = 115200;

    private SerialPort serialPort;

    public float Steering { get; private set; }
    public float Speed { get; private set; }
    public bool ButtonPressed { get; private set; }

    private Thread serialThread;
    private bool running;

    private void Start()
    {
        serialPort = new SerialPort(portName, baudRate);
        serialPort.ReadTimeout = 100;

        try
        {
            serialPort.Open();

            running = true;
            serialThread = new Thread(ReadSerial);
            serialThread.Start();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Could not open Arduino: " + e.Message);
        }
    }

    private void ReadSerial()
    {
        while (running)
        {
            try
            {
                string line = serialPort.ReadLine();

                string[] values = line.Split(',');

                if (values.Length >= 3)
                {
                    float steering = float.Parse(values[0]);
                    float speed = float.Parse(values[1]);
                    bool button = values[2] == "1";

                    Steering = steering;
                    Speed = speed;
                    ButtonPressed = button;
                }
            }
            catch
            {
                // Ignore incomplete/invalid serial messages
            }
        }
    }

    private void OnDestroy()
    {
        running = false;

        if (serialThread != null)
            serialThread.Join();

        if (serialPort != null && serialPort.IsOpen)
            serialPort.Close();
    }*/
}