# calibrationOSC
# microcontroller esp32 NODEMCU
# # calibration for breathing sensors made by stretch conductive fabrics.

the circuit is a voltage divider circuit, uses an analog input (pin 35) and a touch capacitor input (pin 4). 
It sends data via OSC to Unity using a Wifi network (check your computer's credentials and IP).
Unity receives raw data during the calibration time (40 sec) and after that receives 1 and 2. 
This triggers a 'bang' to change the values in a PureData patch (already integrated in Unity, no need to install PD). The output is the change of frequencies of a synthesizer.



