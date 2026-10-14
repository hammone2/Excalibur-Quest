const int potPin = A0;
const int hallPin = A1;

// Wheel circumference in meters
const float wheelCircumference = 2.1;

// Speed measurement
unsigned long lastPulseTime = 0;
unsigned long pulseInterval = 0;

float rpm = 0.0;
float speedMPH = 0.0;

// Adjust these to match your potentiometer's usable range
const int potMin = 0;
const int potMax = 1023;

// Button placeholder: 0 = not pressed
const int buttonPressed = 0;

void setup()
{
    Serial.begin(115200);

    pinMode(hallPin, INPUT_PULLUP);
}

void loop()
{
    // Read potentiometer
    int potValue = analogRead(potPin);

    // Convert to steering range from -1.0 to +1.0
    float steering = ((float)potValue - potMin)
                     / (potMax - potMin) * 2.0 - 1.0;

    steering = constrain(steering, -1.0, 1.0);

    // Read Hall sensor
    static int previousHallState = HIGH;
    int hallState = digitalRead(hallPin);

    // Detect HIGH -> LOW transition
    if (previousHallState == HIGH && hallState == LOW)
    {
        unsigned long currentTime = millis();

        // Debounce / duplicate pulse protection
        if (lastPulseTime == 0 ||
            currentTime - lastPulseTime > 50)
        {
            if (lastPulseTime > 0)
            {
                pulseInterval = currentTime - lastPulseTime;

                if (pulseInterval > 0)
                {
                    rpm = 60000.0 / pulseInterval;

                    float speedMPS =
                        (rpm / 60.0) * wheelCircumference;

                    speedMPH = speedMPS * 2.23694;
                }
            }

            lastPulseTime = currentTime;
        }
    }

    previousHallState = hallState;

    // Stop reporting speed after 1 second without a pulse
    if (lastPulseTime > 0 &&
        millis() - lastPulseTime >= 1000)
    {
        rpm = 0.0;
        speedMPH = 0.0;
    }

    // Unity format: steering,speed,button
    Serial.print(steering, 4);
    Serial.print(",");
    Serial.print(speedMPH, 2);
    Serial.print(",");
    Serial.println(buttonPressed);

    delay(10);
}