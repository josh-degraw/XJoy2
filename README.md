# XJoy2

This is essentially a fork of [XJoy](https://github.com/sam0x17/XJoy), but written in C# and intended to have multiplayer support. I loved the project, but it only supported one player and I wanted to play with my wife, but I'm not familiar enough with C++ to modify that source code, so I decided to implement it in C# instead.

XJoy2 allows you to use a pair of Nintendo Joy-Cons as a virtual Xbox 360 controller
on Windows. XJoy is made possible by [ViGEm](https://vigem.org/) and
[hidapi](https://github.com/signal11/hidapi).

## Installation

1. [Install the ViGEm Bus Driver](https://docs.vigem.org/#!vigem-bus-driver-installation.md)
2. Install the [Visual C++ Redistributable for Visual Studio 2017](https://go.microsoft.com/fwlink/?LinkId=746572)
2. Download the latest zip from the [releases page](https://github.com/josh-degraw/XJoy2/releases) and extract it somewhere permanent like your
Documents folder
3. That's it!

## Usage

1. Pair each of your Joy-Cons with Windows (hold down the button on the side to put into
   pairing mode, then go to add bluetooth device in Windows)
2. Ensure that both Joy-Cons show as "Connected" in your bluetooth devices page
3. Run XJoy2.exe
4. Start playing games with your Joy-Cons. A virtual xbox controller should
   show up as soon as XJoy2.exe starts running (you will hear the USB device inserted sound).
5. To confirm that it is working, try pressing some buttons on your Joy-Cons. You should
   see the names of the buttons currently being pressed printed in the terminal.
6. To exit, simply close the window.