using UnityEngine;
using System;
using System.Globalization;
using System.IO.Ports;
using System.Threading;
using System.IO;

public class ArduinoInput : MonoBehaviour
{
    [SerializeField] private string portName = "COM3";
    [SerializeField] private int baudRate = 115200;

    private SerialPort serialPort;
    private Thread serialThread;
    private volatile bool running;

    private readonly object dataLock = new object();

    private float steering;
    private float speed;
    private bool buttonPressed;

    public float Steering { get; private set; }
    public float Speed { get; private set; }
    public bool ButtonPressed { get; private set; }

    public float steeringMultiplier;

    private void Start()
    {
        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.ReadTimeout = 100;
            serialPort.NewLine = "\n";

            serialPort.Open();

            running = true;
            serialThread = new Thread(ReadSerial);
            serialThread.IsBackground = true;
            serialThread.Start();

            Debug.Log("Arduino connected on " + portName);
        }
        catch (Exception e)
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
                string line = serialPort.ReadLine().Trim();
                string[] values = line.Split(',');

                if (values.Length != 3)
                    continue;

                if (float.TryParse(values[0],
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out float newSteering) &&
                    float.TryParse(values[1],
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out float newSpeed) &&
                    int.TryParse(values[2], out int newButton))
                {
                    lock (dataLock)
                    {
                        steering = Mathf.Clamp(newSteering, -1f, 1f);
                        speed = Mathf.Max(0f, newSpeed);
                        buttonPressed = newButton == 1;
                    }
                }
            }
            catch (TimeoutException)
            {
                // No complete line arrived in time.
            }
            catch (InvalidOperationException)
            {
                break;
            }
            catch (IOException)
            {
                if (running)
                    Debug.LogWarning("Arduino serial connection interrupted.");
                break;
            }
        }
    }

    private void Update()
    {
        lock (dataLock)
        {
            Steering = steering;
            Speed = speed;
            ButtonPressed = buttonPressed;
        }
    }

    private void OnDestroy()
    {
        running = false;

        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                serialPort.Close();
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error closing serial port: " + e.Message);
            }
        }

        if (serialThread != null && serialThread.IsAlive)
            serialThread.Join(500);
    }
}