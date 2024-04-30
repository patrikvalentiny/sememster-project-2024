import encoding.json as json
import encoding.hex
import .flespi-mqtt
import .bme
import .utils
import mqtt
import gpio
import .drv8825


main:
  Utils
  mac_ /string ::= Utils.MAC
  topic-prefix_ /string := Utils.TOPIC-PREFIX
  // setup MQTT client
  flespi-mqtt /Flespi-MQTT := Flespi-MQTT
  client /mqtt.Client := flespi-mqtt.get-client

  // get device MAC address and publish to devices topic
  client.publish "$Utils.TOPIC-PREFIX/devices" (json.encode {"mac":mac_})

  config_ /string := ?
  //TODO: wait for response
  client.subscribe "$Utils.TOPIC-PREFIX/devices/$mac_/config" :: |topic/string payload /ByteArray|
    config_ = json.decode payload
    print "Received config message on topic: $topic with payload: $config_"
  

  driver := DRV8825 client
  
  client.subscribe "$Utils.TOPIC-PREFIX/devices/$mac_/motor/controls" :: |topic/string payload /ByteArray|
    message := json.decode payload
    print "Received motor message on topic: $topic with payload: $message"
    catch --trace:
      driver.step message["steps"]

  bme ::= BME
  // start BME280 data sending task
  bme.send-bme-data-periodically client 5  

  client.subscribe "$topic-prefix_/devices/$mac_/commands/bmertc" :: |topic/string payload /ByteArray|
    message := json.decode payload
    print "Received command message on topic: $topic with payload: $message"
    catch --trace:
      if message["command"] == "start":
        bme.start-rtc client
      else if message["command"] == "stop":
        bme.stop-rtc