import .utils
import encoding.json
import .flespi-mqtt

class Config:
  static INSTANCE /Config? := null
  LAST_MOTOR_POSITION /int? := 0
  MAX_MOTOR_POSITION /int? := 1000

  constructor:
    get-config

  constructor.origin:
    if INSTANCE == null:
      Config
    return INSTANCE

  constructor.from_json json /Map:
    catch:
      LAST-MOTOR-POSITION = json["lastMotorPosition"]
      MAX-MOTOR-POSITION = json["maxMotorPosition"]
      print "Config loaded; lastMotorPosition: $LAST-MOTOR-POSITION, maxMotorPosition: $MAX-MOTOR-POSITION"
      
  get-config:
    client := Flespi-MQTT.get-instance.get-client
    //TODO: wait for response
    client.subscribe "$TOPIC-PREFIX/devices/$MAC/config" :: |topic /string payload /ByteArray|
      json := json.decode payload
      print "Received config message on topic: $topic with payload: $json"
      INSTANCE = Config.from-json json

    while INSTANCE == null:
      // get device MAC address and publish to devices topic
      client.publish "$TOPIC-PREFIX/devices" (json.encode {"mac":MAC}) --retain=true
      sleep --ms=10_000
    