import gpio
import i2c
import bme280
import mqtt
import encoding.json


CLIENT-ID ::= "patrikvalentiny"
HOST ::= "mqtt.flespi.io"
MY-USERNAME ::= "ZhxoTupoNsM1Ynkuxm4yv0vQtujXVYSK5fZMxc1buB0hMazakuwGkRmC9tnUh6ta"
TOPIC ::= "test/1"

main:
  delay-s := 5

  bme ::= BME

  client := mqtt.Client --host=HOST
  options := mqtt.SessionOptions
      --client-id=CLIENT-ID
      --username=MY-USERNAME

  client.start --options=options

  while true:
    temp := bme.get-temp-c
    pressure := bme.get-pressure-pa
    humidity := bme.get-humidity-percent

    payload := json.encode {
      "temperatureC": temp, 
      "pressure": pressure, 
      "humidity": humidity
    }

    // client.publish TOPIC payload

    print "{temperatureC: $temp, pressure: $pressure, humidity: $humidity}"
    
    
    sleep --ms=delay-s * 1000
  
  client.close




class Flespi-MQTT extends mqtt.Client:
  client_ /mqtt.Client := ?

  constructor:
    client_ = super.Client --host="mqtt.flespi.io"
    options := mqtt.SessionOptions
      --client-id=CLIENT-ID
      --username=MY-USERNAME
    client_.start --options=options



class BME:
  driver_ /bme280.Driver := ?

  constructor alt-address = false:
    bus := i2c.Bus
      --sda=gpio.Pin 21
      --scl=gpio.Pin 22
    // The BME280 can be configured to have one of two different addresses.
    // - bme280.I2C_ADDRESS, equal to 0x76
    // - bme280.I2C_ADDRESS_ALT, equal to 0x77
    // The address is generally chosen by the break-out board.
    // If the example fails with I2C_READ_FAILED verify that you are using the correct address.
    address := alt-address ? bme280.I2C_ADDRESS_ALT : bme280.I2C_ADDRESS
    device := bus.device address
    driver_ = bme280.Driver device

  get-temp-c:
    return driver_.read_temperature  
  get-temp-f:
    return (driver_.read_temperature * 1.8) + 32
  get-pressure-pa:
    return driver_.read_pressure
  get-humidity-percent:
    return driver_.read_humidity 
  