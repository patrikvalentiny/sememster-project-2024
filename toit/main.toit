import encoding.json as json
import encoding.hex
import .flespi-mqtt
import .bme
import .utils
import mqtt
import gpio
import .drv8825
import .config


main:
  //init utils
  Utils
  driver := DRV8825
  // setup MQTT client
  client /mqtt.Client := Flespi-MQTT.get-instance.get-client
  
  
  client.subscribe "$TOPIC-PREFIX/devices/$MAC/motor/controls" :: |topic/string payload /ByteArray|
    message := json.decode payload
    print "Received motor message on topic: $topic with payload: $message"
    catch --trace:
      driver.step message["steps"]

  bme ::= BME
  // start BME280 data sending task
  bme.send-bme-data-periodically client 5  

  client.subscribe "$TOPIC-PREFIX/devices/$MAC/commands/bmertc" :: |topic/string payload /ByteArray|
    message := json.decode payload
    print "Received command message on topic: $topic with payload: $message"
    catch --trace:
      if message["command"] == "start":
        bme.start-rtc client
      else if message["command"] == "stop":
        bme.stop-rtc

