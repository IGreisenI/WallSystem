# Wall Build System in Unity

## Overview
This is a wall system for making walls based on a vr boundary. All that is needed is to call the CreateWallWithMeshes function in WallCreator with the VR boundary list of points. 

In the project there is also a drawing system to allow the user to draw walls in the scene. Scenes for mouse and vr drawing can be found under Assets->DrawingWalls->Sample.
Once a line is drawn, the system converts it into a wall using a list of Vector3 points, generating rectangular geometry between points with proper UV mappings.

In this project for models I am using a free https://assetstore.unity.com/packages/3d/environments/historic/polylised-medieval-desert-city-94557 asset.\
I am using Naughty Attributes for the buttons in the inspectors https://github.com/dbrizov/NaughtyAttributes \
For Debugging in VR I am using DebugXR, by Tobiesen https://assetstore.unity.com/packages/tools/gui/in-game-debug-log-for-ar-and-vr-devices-174780 \ 

## Features

- **Wall Generation**: Create walls using a list of Vector3 points.
- **Geometry Creation**: Generate rectangular geometry between points with customizable height.
- **UV Mapping**: Automatically set UV mappings for the wall textures.
- **Drawing System**: 
  - Raycasts from the camera to create lines.
  - Customize color, thickness, and point distances.
  - Uses Unity's new input system for drawing controls.

## Installation

1. Clone the repository:
    ```sh
    git clone https://github.com/IGreisenI/WallSystem.git
    ```
2. Open the project in Unity.

## Usage

### Drawing Walls
1. **Customize Drawing**:
    - **Color**: Change the color of the line through the provided UI or settings script.
    - **Thickness**: Adjust the thickness of the line.
    - **New Point Distance**: Set the minimum distance between points for a new segment to be added.
    - **Continue Drawing Distance**: Define the distance to continue drawing from the last point.
2. **Finish Drawing**: Release the mouse button to finalize the line.
3. **Start Drawing**: Click and hold the mouse button to start drawing a line.
4. **Generate Wall**: The system will automatically generate a wall based on the drawn line's points.

![Unity_cKrpunmu1n](https://github.com/IGreisenI/WallSystem/assets/58489283/0977633b-f264-44ad-8408-15a4c88af996)
![Unity_VNOIo0CrLW](https://github.com/IGreisenI/WallSystem/assets/58489283/f3724755-86d6-42c2-a6a2-e3999d5cb51f)

### Wall Creator Component
Buttons in the inspector:
    - Create Random Wall From Points
      - Creates a circle of points dictated by 'numberOfPoint' and 'radius' fields
      - Creates walls without meshes but with colliders
    - Create Random Wall With Mesh From Points
      - Creates the same thing but with meshes
    - Add Geo
      - Adds geometry along the walls. Settings for this are under "Dynamic Geometry Settings" in the inspector.   
![Unity_DOybRehuuP](https://github.com/IGreisenI/WallSystem/assets/58489283/4607e0d6-053a-4dac-af2d-b07566db0014)

### Editing the corner pieces
The corner pieces have a clickable dot in their origins. Once they are clicked an editor window pops up with models within the project. Selecting one and pressing "Place Object" will replace the corner piece with the selected model.
![Unity_BDq0fPyh66](https://github.com/IGreisenI/WallSystem/assets/58489283/06bcd779-01f0-4769-b08b-a132939b7770)

