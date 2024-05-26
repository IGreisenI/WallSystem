# Wall Build System in Unity

## Overview
This is a wall system for making walls based on a vr boundary. All that is needed is to call the CreateWallWithMeshes function in WallCreator with the VR boundary list of points. 

In the project there is also a drawing system to allow the user to draw walls in the scene. Scenes for mouse and vr drawing can be found under Assets->DrawingWalls->Sample.
Once a line is drawn, the system converts it into a wall using a list of Vector3 points, generating rectangular geometry between points with proper UV mappings.

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

### Wall Creator Component
Buttons in the inspector:
    - Create Random Wall From Points
      - Creates a circle of points dictated by 'numberOfPoint' and 'radius' fields
      - Creates walls without meshes but with colliders
    - Create Random Wall With Mesh From Points
      - Creates the same thing but with meshes
    - Add Geo
      - Adds geometry along the walls. Settings for this are under "Dynamic Geometry Settings" in the inspector.   
