


<a id="readme-top"></a>
<div align="center">
<h1 align="center">LibCast Engine</h1>

  <p align="center">
    A from-scratch tile-based raycaster game-engine utilizing <a href="https://github.com/raylib-cs/raylib-cs">raylib-cs</a>.
  </p>

  <p align="center">
<img /src="https://nthorn.com/images/libcast/libcast-short-demo.webp" width="500">
<h6>Example LibCast game</h6>
</p>
</div>

<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about">About</a>
      <ul>
        <li><a href="#game-flow">Game flow</a></li>
        <li><a href="#graphics">Graphics</a></li>
        <li><a href="#ui">UI</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation--usage">Installation & Usage</a></li>
      </ul>
    </li>
    <li><a href="#ai-disclaimer">AI Disclaimer</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
  </ol>
</details>



<!-- ABOUT -->
## About

LibCast is a tile-based raycaster game engine that utilizes a C# implementation of RayLib ([raylib-cs](https://github.com/raylib-cs/raylib-cs)). This game engine makes heavy use of object-oriented programming techniques, with developer work-flows and backend logic akin to [GameMaker Studio](https://gamemaker.io/en).

Developing games with LibCast currently is a UX-free affair, with tools like map editors and project managers in progress. VSCode is recommended for developing with LibCast.



<p align="right">(<a href="#readme-top">back to top</a>)</p>


### Game Flow

LibCast utilizes both entity and room-based game loops. The easiest way to understand the game flow is start in [_Program.cs](/src/utility/_Program.cs) and follow the loops into [_Game.cs](/src/_Game.cs), and then the respective rooms.

The developer work-flow for LibCast is very similar to [GameMaker Studio](https://gamemaker.io/en) as it relates to map and entity (or "object") construction, step loops, and interrupts. All maps have entities and a main-loop, the main-loop then loops over all of it's entities' loops (see [_Room.cs](/src/rooms/_Room.cs)).

<p align="right">(<a href="#readme-top">back to top</a>)</p>


### Graphics

See [_Screen.cs](/src/screen/_Screen.cs) for all low-level drawing functions and the basics of the graphics functionality and [_Raycaster.cs](/src/entities/raycastable/_Raycaster.cs) for all raycasting/sprite drawing logic.

Walls/environments are rendered using [DDA raycasting techniques](https://lodev.org/cgtutor/raycasting.html), with support for variable-height, transparency, fixed-height floors/ceilings, and 2D scrolling sky-boxes. Entities are rendered as sprites.

All custom graphics functions are ran CPU-side, making use of a screen buffer that gets rendered at a larger scale by RayLib. This allows for easy rendering of both low-resolution and high-resolution images (i.e. text over sprites) without the need for multiple variable-resolution buffers to render individual images to. This requires that games made in LibCast run at a low-resolution, something that is both an aesthetic decision of LibCast as well as a real technical limitation.

There is work-in-progress support for terminal-esque emulation for the purpose of developing ascii-based games that are not limited by actually having to run within a terminal (see [_Terminal.cs](/src/screen/_Terminal.cs)).

<p align="right">(<a href="#readme-top">back to top</a>)</p>


### UI

LibCast allows for the easy creation of buttons and UI interactions with use of the MouseCollision class.

1. Instantiate and start MouseCollision family
2. Draw all relevant items that should react according to the family.
3. End MouseCollision family.

This causes all pixels drawn within the start/end of the family, like a button, to detect hovers/clicks and react appropriately depending on their assigned family (see [_UI.cs](/src/screen/_Screen.cs), [PauseUIButtons.cs](/src/screen/UI/PauseUI/PauseUIButtons.cs) and [_Screen.cs](/src/screen/_Screen.cs)).

Menus are handled largely using state-machines (see [_UI.cs](/src/screen/_Screen.cs)).

<p align="right">(<a href="#readme-top">back to top</a>)</p>


<!-- INSTALLATION -->
## Getting started

RayLib's default project is a demo of a procedurally-generated open world. A default building or "structure" can be dynamically placed in this demo by pressing `b`.

See [About](#about) for technical information to begin game development. Proper and thorough documentation is coming in the future.

### Prerequisites

Microsoft's [.NET](https://dotnet.microsoft.com/en-us/download) is required.

### Installation & Usage

1. Clone/download the repo
   ```sh
   git clone https://github.com/nTh0rn/libcast-engine.git
   ```
2. Use `dotnet build` and `dotnet run` respectively to run LibCast.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## AI Disclaimer
LibCast was developed with little-to-no support from Gen-AI. It is a personal goal to keep as much Gen-AI out of LibCast's source code as possible for the sake of learning. The overwhelming majority of LibCast, including architectural/technical decisions, as well as almost every single line of code, were determined and typed by-hand without any assistance from Gen-AI.

Notable exceptions exist in the following area(s):
* JSON Structure Parsing (unfinished)

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- LICENSE -->
## License

Distributed under the MIT License and is subject to the licenses of both raylib and raylib-cs. See [LICENSE.txt](/LICENSE.txt) and [THIRD_PARTY_NOTICES.txt](/THIRD_PARTY_NOTICES.txt) for more information.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- CONTACT -->
## Contact

Nikolas Thornton - [nthorn.com](https://nthorn.com)

<p align="right">(<a href="#readme-top">back to top</a>)</p>

