import mqtt
import .secrets as secrets
import .secrets-dev as secrets-dev
import .utils


class Flespi-MQTT:
  static INSTANCE /Flespi-MQTT? := null
  static CLIENT-ID /string ::= Utils.get-mac-string
  client_ /mqtt.Client := mqtt.Client --host=secrets.MQTT-HOST

  constructor:
    options := mqtt.SessionOptions
      --client-id=CLIENT-ID
      --username=DEV ? secrets-dev.FLESPI-USERNAME : secrets.FLESPI-USERNAME
      --password=DEV ? secrets-dev.FLESPI-PASSWORD : secrets.FLESPI-PASSWORD
    client_.start --options=options
  
  static get-instance -> Flespi-MQTT:
    if INSTANCE == null:
      INSTANCE = Flespi-MQTT
    return INSTANCE

  get-client -> mqtt.Client:
    return client_

  close:
    client_.close

