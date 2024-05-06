import gpio
import .secrets-dev as secrets
import encoding.json
import .utils
import .config
import .flespi-mqtt

class DRV8825:
  reversed_ ::= false
  static DIR_PIN_ ::= 17
  static STEP_PIN_ ::= 16

  static DIR ::= gpio.Pin DIR_PIN_ --output
  static STEP ::= gpio.Pin STEP_PIN_ --output
  static DELAY ::= 1

  position := ?
  max-position := ? 

  MQTT-CLIENT ::= ?
  

  constructor reversed /bool = false:
    reversed_ = reversed
    MQTT-CLIENT = Flespi-MQTT.get-instance.get-client

    config /Config := Config.origin
    position = config.LAST-MOTOR-POSITION
    max-position = config.MAX-MOTOR-POSITION
  

  step steps /int:
    // pos +
    if steps > 0:
      if position + steps > max-position:
        steps = max-position - position
      stepCW steps
    // pos -
    else:
      if position + steps < 0:
        steps = -position
      stepACW -steps

  stepCW steps /int:
    moving:= true
    DIR.set (reversed_ ? 0 : 1)
    print "CW"
    steps.repeat:
      position += 1
      STEP.set 1
      sleep --ms= DELAY
      STEP.set 0
      sleep --ms= DELAY
      
    send-position
    moving = false


  stepACW steps /int:
    moving:= true
    DIR.set (reversed_ ? 1 : 0)
    print "ACW"
    steps.repeat:
      position -= 1
      STEP.set 1
      sleep --ms= DELAY
      STEP.set 0
      sleep --ms= DELAY
      
    send-position
    moving = false


  go-to-position pos /int:
    steps := pos - position
    step steps

  reset-start:
    position = 0

  //send position to mqtt
  send-position:
    payload := json.encode {"position": position}
    // print position
    // print "Pos: $payload.to-string"
    // print "Topic: $topic-prefix/devices/$MAC/motor"
    MQTT-CLIENT.publish "$TOPIC-PREFIX/devices/$MAC/motor/data" payload 

 
      
