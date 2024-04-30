import gpio
import .secrets-dev as secrets
import encoding.json
import .utils

class DRV8825:
  reversed_ ::= false
  static DIR_PIN_ ::= 17
  static STEP_PIN_ ::= 16

  static DIR ::= gpio.Pin DIR_PIN_ --output
  static STEP ::= gpio.Pin STEP_PIN_ --output
  static DELAY ::= 1

  MQTT-CLIENT ::= ?

  constructor client reversed /bool = false:
    
    reversed_ = reversed
    MQTT-CLIENT = client

  position := 0

  step steps /int:
    // pos +
    if steps > 0:
      stepCW steps
    
    // pos -
    else:
      if position + steps < 0:
        steps = -position
      stepACW -steps

  stepCW steps /int:
    DIR.set (reversed_ ? 0 : 1)
    print "CW"
    position += steps
    steps.repeat:
      STEP.set 1
      sleep --ms= DELAY
      STEP.set 0
      sleep --ms= DELAY
    
    send-position


  stepACW steps /int:
    DIR.set (reversed_ ? 1 : 0)
    print "ACW"
    position -= steps
    steps.repeat:
      STEP.set 1
      sleep --ms= DELAY
      STEP.set 0
      sleep --ms= DELAY
    send-position

  reset-start:
    position = 0

  //send position to mqtt
  send-position:
    payload := json.encode {"position": position}
    // print "Pos: $payload.to-string"
    // print "Topic: $topic-prefix/devices/$MAC/motor"
    MQTT-CLIENT.publish "$TOPIC-PREFIX/devices/$MAC/motor/data" payload 

 
      
