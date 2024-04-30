import encoding.json as json
import encoding.hex
import .flespi-mqtt
import .bme
import .utils
import mqtt
import gpio
import .drv8825


main:
  //init utils
  Utils
  // setup MQTT client
  flespi-mqtt /Flespi-MQTT := Flespi-MQTT
  client /mqtt.Client := flespi-mqtt.get-client

  //TODO: pong device config back
  // get-config client
  
  
  driver := DRV8825 client
  
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

get-config client:
  // get device MAC address and publish to devices topic
  client.publish "$TOPIC-PREFIX/devices" (json.encode {"mac":MAC})

  config_ /string? := null
  //TODO: wait for response
  client.subscribe "$TOPIC-PREFIX/devices/$MAC/config" :: |topic/string payload /ByteArray|
    config_ = json.decode payload
    print "Received config message on topic: $topic with payload: $config_"

  while config_ == null:
    sleep --ms=1000