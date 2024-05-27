import encoding.hex
import gpio
import .secrets as secrets
import .secrets-dev as secrets-dev

DEV /bool := false
TOPIC-PREFIX /string := secrets.TOPIC-PREFIX
MAC /string := Utils.get-mac-string

class Utils:
  static get_mac_address -> ByteArray:
    #primitive.esp32.get_mac_address
  static get-mac-string -> string:
    return hex.encode get_mac_address
  
  constructor:
    // set development mode 
    // connect pin 13/D7 to GND to enable development mode
    dev-pin ::= gpio.Pin 13 --input --pull-up
    DEV = dev-pin.get == 0 ? true : false
    if DEV:
      print "Development mode"
      // onboard led is on if in dev mode
      dev-led ::= gpio.Pin 2 --output
      dev-led.set dev-pin.get
      TOPIC-PREFIX = secrets-dev.TOPIC-PREFIX
    else:
      print "Production mode"
