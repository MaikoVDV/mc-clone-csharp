# A simple voxel engine written in C#
This project is currently only a simple analog to Minecraft Alpha, but that's not the objective for the project. In the future, I hope to use this engine to explore 3D voxel world generation, especially using the wave function collapse algorithm.

## Objectives
- [x] Basics
  - [x] Rendering voxels efficiently using chunking and removing covered mesh faces
  - [x] Player movement
  - [x] Block placing and breaking with raycasting
- [x] Physics
  - [x] Collision detection using AABBs
  - [x] Handle collisions when performing physics move
  - [ ] Sweeping collision detection / raycasting to find the collision point more accurately.
- [x] Block update system
  - [x] Store more complex data about blocks (powered, transparent, etc.)
  - [x] Implement fluids using Minecraft's fluid behavior rules
  - [x] Build more streamlined block and chunk update logic
  - [x] Implement transparency with shaders and blockface rendering order
- [ ] Lighting
- [ ] Interaction
  - [ ] Block picking with UI
  - [ ] Levers and buttons (non-functional)
  - [ ] Redstone update system with wire and lights