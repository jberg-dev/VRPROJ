# VRPROJ
> This is a university level short project. 
> The code has been developed in about 120 total man hours from the state of not knowing anything about Unity at all.
> From this, you can understand that it isn't a great project, but it's decent for what it is representing.

> Do with it what you wish.

## Install
Due to storage limitations on github, I have not commited the SteamVR Dev Kit or the Oculus Dev Kit, thus the first thing you have to do when you get into Unity is:
1) Download the SteamVR kit from the unity asset store and import it.
2) Download the Oculus dev kit from the unity asset store and import it. (OPTIONAL)

## Usage

This project were developed for the VIVE PRO VR on a Windows machine, so any bindings have only been set up for it.
It shouldn't be difficult to change things around with another set, though, as the VR base is built with SteamVR 2.2, and it very rebindable by design.

The Oculus kit is used for its mouse-and-keyboard VR simulation experience. It's not great, but it works.

To use the simulator mode, 
1) Make sure that the boolean value Simulator in RayCasting Manager is set to true.
2) Make sure that the MENUS canvas is set to Screen Overlay.
3) Press H to open menus.
4) Press L to delete all nodes on the screen.

To use VR mode (VIVE)
1) Make sure that the boolean value Simulator in RayCasting Manager is set to FALSE.
2) The menu button above the trackpad calls the menus.
3) One trigger triggers a laser pointer which is used to select everything. Everything includes nodes and menu options.
4) On the opposite controller from the laser pointer, the trackpad will rotate the spawned nodes.

## Acknowledgements

Newtonsoft JSON
Copyright (c) 2007 James Newton-King

SteamVR Unity Plugin - v2.4.5
Copyright (c) Valve Corporation, All rights reserved.

Oculus for Unity
© 2019 Oculus VR, LLC. All Rights Reserved.

Unity
© 2019 Unity Technologies

Microsoft Visual Studio
© Microsoft 2019


## Contributing

This project will not be updated further. If you wish to, though, feel free to fork away!

## License

MIT