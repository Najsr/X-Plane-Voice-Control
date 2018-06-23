# X-Plane Voice Control

Simple to use Voice control made for Zibo's 738X & 739 Ultimate

Latest build: [![Build status](https://ci.appveyor.com/api/projects/status/ejvcv44bm65t9814?svg=true)](https://ci.appveyor.com/project/Najsr/x-plane-voice-control/build/artifacts)

## Getting Started


### Prerequisites

You need ExtPlane Plugin installed ([Download](https://github.com/vranki/ExtPlane/releases))


### Installing

* Get the latest release (or [build](https://ci.appveyor.com/project/Najsr/x-plane-voice-control/build/artifacts) for latest features) from [here](https://github.com/Najsr/X-Plane-Voice-Control/releases)
* Extract the ZIP and run the .exe file
* Load into Boeing airplane and press __Connect__
* Enjoy and report any bugs encountered

### Command Line Option

* Add __--console__ to launch parameter and CMD window will show with info about program

### Available Commands
* Parking brake control
  * __[set the parking brake / engage the brakes / release the brakes / disengage the brakes]__
* Com set and swap
  *  __[tune / set] [com1 / com2] to XXX [decimal / point] X__ XX
  *  __swap [com1 / com2]__
* Nav set and swap
  *  __[tune / set] [nav1 / nav2] to XXX [decimal / point] X__ X
  *  __swap [nav1 / nav2]__
* Flaps control
  * set __flaps [up / down / zero / one / two / five / ten / fifteen / twentyfive / thirty / forty]__
  * set __flaps [up / down] a notch__
* Gear control
  * __set the [gear up / gear down / raise the landing gear / extend the landing gear]__
* Landing lights control
  *  set __[landing lights on / landing lights off]__
* Taxi lights control
  *  set __[taxi lights on / taxi lights off]__
* Engine control
  *  __[start / light up / kill / shut down] the engine number [one / two]__
  *  __introduce fuel [into / to] number engine [one / two]__
* APU control
  *  __[start / light up / stop / shutdown] APU__
  *  __APU [on / off]__ please
* Probe heat
  *  set __probe heat [on/off]__
* Window heat
  *  set __window heat [on/off]__
* No Smoking / Seatbelts
  *  set __[no smoking / seatbelts] [off / auto / on]__
* Packs
  *  set __[left / right] pack to [off / auto / high]__
* Bleed air
  *  set __[apu / engine one / engine two] bleed air__ to __[off / on]__
* Power bus
  *  __[connect / disconnect] [left / right] [APU / engine] generator__
* Ground power
  *  __[connect / disconnect] the ground power__
* Lights
  *  set __[logo / position / beacon / wing / wheel] lights to [off / on]__
* Fuel control
  *  set __[left / right / center / all] fuel pumps__ to __[off / on]__
  *  set __crossfeed__ to __[on / off]__
* Transponder
  *  [tune / set] __[transponder / squawk]__ code to __TTTT__
  *  set __transponder__ mode to __[off / on]__


__Cheat sheet:__
* __Bold text is Mandatory__ 
* Normal text is Optional
* __X => [zero / one / two / three / four / fiver / six / seven / eight / niner]__
* __T => [zero / one / two / three / four / fiver / six / seven]__

## Built With

* [.NET Framework](https://www.microsoft.com/net/download/windows/)
* [ExtPlane Net library](https://github.com/Najsr/ExtPlaneNet) - modified

## Contributing

TBD

## Authors

* **David** - *Initial work* - [Nicer](https://github.com/Najsr)

See also the list of [contributors](https://github.com/Najsr/X-Plane-Voice-Control/graphs/contributors) who participated in this project.

## License

This project is licensed under the GPLv3 License - see the [LICENSE](LICENSE) file for details

## Acknowledgments

* [jrunestone](https://github.com/jrunestone) - For his work on ExtPlane NET library
* [vranki](https://github.com/vranki) - For the ExtPlane library
