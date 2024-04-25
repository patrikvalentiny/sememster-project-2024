import encoding.json as json
import encoding.hex
// TODO: change to production secrets
import .secrets-dev as secrets
import .flespi-mqtt
import .bme
import .utils
import mqtt
import gpio


topic-prefix /string := secrets.TOPIC-PREFIX
MAC /string ::= hex.encode Utils.get_mac_address
main:
  
  
  // setup MQTT client
  flespi-mqtt /Flespi-MQTT := Flespi-MQTT
  client /mqtt.Client := flespi-mqtt.get-client

  // get device MAC address and publish to devices topic
  payload /ByteArray := json.encode {"mac":MAC}
  client.publish "$topic-prefix/devices" payload

  config_ /string := ?
  //TODO: wait for response
  client.subscribe "$topic-prefix/devices/$MAC/config" :: |topic/string payload /ByteArray|
    config_ = json.decode payload
    print "Received config message on topic: $topic with payload: $config_"



  task:: send-bme-data client
  
send-bme-data client:
  delay-s /int := 5
  // create BME280 sensor instance
  bme ::= BME

  while true:
    temp := bme.get-temp-c
    pressure := bme.get-pressure-pa
    humidity := bme.get-humidity-percent

    payload := json.encode {
      "temperatureC": temp, 
      "pressure": pressure, 
      "humidity": humidity
    }

    print payload.to-string
    
    // send BME280 data to MQTT broker
    client.publish "$topic-prefix/data/$MAC/bmedata" payload

    sleep --ms=delay-s * 1000
   