import gpio
import gpio.adc

main:
  pin := gpio.Pin 34
  adc := adc.Adc pin
  256.repeat:
    print adc.get
    sleep --ms=500
