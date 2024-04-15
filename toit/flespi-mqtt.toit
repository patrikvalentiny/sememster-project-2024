import mqtt
import .secrets
import .enums


class Flespi-MQTT:

  static CLIENT-ID /string ::= "patrikvalentiny"
  client_ /mqtt.Client := mqtt.Client --host=MQTT-HOST

  constructor:
    options := mqtt.SessionOptions
      --client-id=CLIENT-ID
      --username=FLESPI-USERNAME
    client_.start --options=options

  get-client -> mqtt.Client:
    return client_

  close:
    client_.close

