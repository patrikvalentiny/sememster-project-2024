import gpio
import i2c
import bme280
import encoding.json
import .utils
import .flespi-mqtt
import mqtt

interface BME:
  send-bme-data-periodically minutes /int

class BME280 implements BME:
  driver_ /bme280.Driver := ?
  MQTT-CLIENT_ /mqtt.Client := ?

  constructor alt-address = false:
    MQTT-CLIENT_ = Flespi-MQTT.get-instance.get-client
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

    subscribe-rtc


  get-temp-c -> float:
    return driver_.read_temperature  
  get-temp-f -> float:
    return (driver_.read_temperature * 1.8) + 32
  get-pressure-pa -> float:
    return driver_.read_pressure
  get-humidity-percent -> float:
    return driver_.read_humidity 
  
  get-json -> ByteArray:
    return json.encode{
        "temperatureC": get-temp-c,
        "pressure": get-pressure-pa,
        "humidity": get-humidity-percent
    }

  send-bme-data-periodically minutes /int:
    delay-s /int := (minutes * 60).to-int
    task::
      while true: 
        // send BME280 data to MQTT broker
        MQTT-CLIENT_.publish "$TOPIC-PREFIX/devices/$MAC/bmedata" get-json
        sleep --ms=delay-s * 1000


  live_ /bool := false
  start-rtc --s=1000:
    if live_: return
    live_ = true
    task::
      while live_:     
        // send BME280 data to MQTT broker
        MQTT-CLIENT_.publish "$TOPIC-PREFIX/devices/$MAC/bmedata/rtc" get-json --qos=0
        sleep --ms=s * 1000

  stop-rtc:
    live_ = false

  subscribe-rtc:
    MQTT-CLIENT_.subscribe "$TOPIC-PREFIX/devices/$MAC/commands/bmertc" :: |topic/string payload /ByteArray|    
      catch --trace:
        message := json.decode payload
        print "Received command message on topic: $topic with payload: $message"
        if message["command"] == "start":
          start-rtc --s=5
        else if message["command"] == "stop":
          stop-rtc


class BME-SIM implements BME:
  MQTT-CLIENT_ /mqtt.Client := ?
  constructor alt-address = false:
    MQTT-CLIENT_ = Flespi-MQTT.get-instance.get-client
    subscribe-rtc

  temp /float := 25.0
  press := 101325.0
  humidity := 50.0
  get-temp-c -> float:
    temp += ((random 100) - 50) / 100.0
    return temp
  get-pressure-pa -> float:
    press += ((random 100) - 50) * 100.0
    return press
  get-humidity-percent -> float:
    humidity += ((random 100) - 50) / 100.0
    return humidity
  
  get-json -> ByteArray:
    return json.encode{
        "temperatureC": get-temp-c,
        "pressure": get-pressure-pa,
        "humidity": get-humidity-percent
    }

  send-bme-data-periodically minutes /int:
    delay-s /int := (minutes * 60).to-int
    task::
      while true: 
        // send BME280 data to MQTT broker
        MQTT-CLIENT_.publish "$TOPIC-PREFIX/devices/$MAC/bmedata" get-json
        sleep --ms=delay-s * 1000


  live_ /bool := false
  start-rtc --s=1000:
    if live_: return
    live_ = true
    task::
      while live_:     
        // send BME280 data to MQTT broker
        MQTT-CLIENT_.publish "$TOPIC-PREFIX/devices/$MAC/bmedata/rtc" get-json --qos=0
        sleep --ms=s * 1000

  stop-rtc:
    live_ = false

  subscribe-rtc:
    MQTT-CLIENT_.subscribe "$TOPIC-PREFIX/devices/$MAC/commands/bmertc" :: |topic/string payload /ByteArray|    
      catch --trace:
        message := json.decode payload
        print "Received command message on topic: $topic with payload: $message"
        if message["command"] == "start":
          start-rtc --s=5
        else if message["command"] == "stop":
          stop-rtc