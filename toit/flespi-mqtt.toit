import mqtt
import .secrets as secrets
import .secrets-dev as secrets-dev
import .utils


class Flespi-MQTT:

  static CLIENT-ID /string ::= "patrik"
  client_ /mqtt.Client := mqtt.Client --host=secrets.MQTT-HOST

  constructor:
    options := mqtt.SessionOptions
      --client-id=CLIENT-ID
      --username=DEV ? secrets-dev.FLESPI-USERNAME : secrets.FLESPI-USERNAME
    client_.start --options=options

  get-client -> mqtt.Client:
    return client_

  close:
    client_.close

