import encoding.json
import encoding.hex
import .secrets
import .flespi-mqtt
import .bme
import .utils
import .enums
import mqtt


main:
  // setup MQTT client
  flespi-mqtt /Flespi-MQTT := Flespi-MQTT
  client /mqtt.Client := flespi-mqtt.get-client

  // get device MAC address and publish to devices topic
  mac /string := hex.encode Utils.get_mac_address
  payload /ByteArray := json.encode {"mac":mac}
  client.publish "$TOPIC-PREFIX/devices" payload

  //TODO: wait for response
  client.subscribe "$TOPIC-PREFIX/devices/$mac/config" :: |topic/string payload /ByteArray|
    print "Received config message on topic: $topic with payload: $payload.to-string"
  
  // create BME280 sensor instance
  // bme ::= BME


  // client.close
  // while true:
  //   temp := bme.get-temp-c
  //   pressure := bme.get-pressure-pa
  //   humidity := bme.get-humidity-percent

  //   payload := json.encode {
  //     "temperatureC": temp, 
  //     "pressure": pressure, 
  //     "humidity": humidity
  //   }

  //   // client.publish TOPIC payload

  //   print "{temperatureC: $temp, pressure: $pressure, humidity: $humidity}"
    
    
  //   sleep --ms=delay-s * 1000
