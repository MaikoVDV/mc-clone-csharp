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
- [ ] Support more complex block behaviors (e.g. fluids, a redstone-like system)
  - [ ] Store more complex data about blocks (powered, transparent, etc.)
  - [ ] Implement fluids using Minecraft's fluid behavior rules (fluid graphics are a challenge for later)
  - [ ] Implement basic redstone circuits: levers, wire and torches
- [ ] Lighting
