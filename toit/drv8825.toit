import gpio
import .secrets-dev as secrets
import encoding.json
import .utils
import .config
import .flespi-mqtt

interface StepperDriver:
  step steps /int
  stepCW steps /int
  stepACW steps /int
  go-to-position pos /int
  send-position

class DRV8825 implements StepperDriver:
  reversed_ := false
  static DIR_PIN_ ::= 17
  static STEP_PIN_ ::= 16

  static DIR ::= gpio.Pin DIR_PIN_ --output
  static STEP ::= gpio.Pin STEP_PIN_ --output
  static DELAY ::= 1

  position := ?
  max-position := ? 

  MQTT-CLIENT_ ::= ?
  

  constructor:
    MQTT-CLIENT_ = Flespi-MQTT.get-instance.get-client

    config /Config := Config.origin
    reversed_ = config.MOTOR-REVERSED
    position = config.LAST-MOTOR-POSITION
    max-position = config.MAX-MOTOR-POSITION

    subscribe-to-mqtt_
  
  subscribe-to-mqtt_:
    MQTT-CLIENT_.subscribe "$TOPIC-PREFIX/devices/$MAC/motor/controls" :: |topic/string payload /ByteArray|
      catch --trace:
        message := json.decode payload
        // print "Received motor message on topic: $topic with payload: $message"
        p := message.get "position"
        if p != null:
          go-to-position p
        else:
          step message["steps"]

    MQTT-CLIENT_.subscribe "$TOPIC-PREFIX/devices/$MAC/commands/motor" :: |topic/string payload /ByteArray|    
      catch --trace:
        message := json.decode payload
        // print "Received command message on topic: $topic with payload: $message"
        reversed := message.get "reversed"
        max-pos := message.get "maxPosition"
        if reversed != null:
          reversed_ = reversed
          Config.MOTOR-REVERSED = reversed
          // print "Motor direction changed to: $reversed"
        if max-pos != null:
          max-position = max-pos
          Config.MAX-MOTOR-POSITION = max-pos
          // print "Motor max position changed to: $max-pos"
        if reversed == null and max-pos == null:
          print "Invalid command message on topic: $topic with payload: $message"

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
    steps.repeat:
      position -= 1
      STEP.set 1
      sleep --ms= DELAY
      STEP.set 0
      sleep --ms= DELAY
      
    send-position
    moving = false

  go-to-position pos /int:
    if pos > max-position:
      pos = max-position
    steps := pos - position
    step steps

  //send position to mqtt
  send-position:
    payload := json.encode {"position": position}
    MQTT-CLIENT_.publish "$TOPIC-PREFIX/devices/$MAC/motor/data" payload 


