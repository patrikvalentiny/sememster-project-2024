# ClimateCtrl

Realtime data visualization of temperature and humidity data from a ESP32 in a dashboard.

### [Configuration](toit/config.toit)
- Configuration of the device
- Sends the device mac to the MQTT broker to receive the configuration from the server
- Blocks the execution until the configuration is received from the MQTT broker

### [MQTT Driver](toit/flespi-mqtt.toit)
- Client for the Flespi MQTT broker
- Used as singleton in the project

### [BME280 driver](toit/bme.toit)
- Utilizes I2C protocol for the BME280 sensor
- Reads temperature, humidity and air pressure data from the sensor
- Sends data periodically to the MQTT broker in specific intervals
- Able to start and stop the realtime communication
  - Listening to `$TOPIC-PREFIX/devices/$MAC/commands/bmertc` for `start` and `stop` commands

### [DRV8825](toit/drv8825.toit)https://climate-ctrl.web.app/
- Driver for the DRV8825 stepper motor driver
- Subscribes MQTT topics:
  - `$TOPIC-PREFIX/devices/$MAC/commands/motor` for max position and direction change commands
  - `$TOPIC-PREFIX/devices/$MAC/motor/controls` for moving the motor to a specific position or moving a specific amount of steps
- Sends the current position to the MQTT broker after each movement `$TOPIC-PREFIX/devices/$MAC/motor/data`
- Interfaces for:
  - Go to a specific position
  - Move a specific amount of steps CW or CCW
  - Send the current position

