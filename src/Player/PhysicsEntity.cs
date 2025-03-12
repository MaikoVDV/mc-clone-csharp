
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using static System.Reflection.Metadata.BlobBuilder;

namespace mc_clone
{
    internal class PhysicsEntity
    {
        protected float maxSpeed;
        protected World world;
        private readonly Vector3 scale = new Vector3(0.8f, 1.8f, 0.8f);

        protected Vector3 dragVector;
        protected Vector3 gravityVector;

        private Vector3 velocity;
        protected Vector3 position;
        private Vector3 storedAccel; // Clears after every physics tick.

        public Vector3 Position { get { return position; } }
        protected bool OnGround { get { return onGround; } }
        protected bool onGround;

        public PhysicsEntity(World world)
        {
            this.world = world;
        }

        public void UpdatePhysics(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Add drag
            storedAccel += -dragVector * velocity;

            // Add gravity
            storedAccel += gravityVector;

            // Adjust desired acceleration to not exceed max futureSpeed, then apply.
            float futureSpeed = (velocity + storedAccel * deltaTime).Length();
            Vector3 trueAccel = storedAccel * MathF.Min(1, (maxSpeed - futureSpeed) / (storedAccel.Length() * deltaTime));
            velocity += trueAccel * deltaTime;

            Vector3[] axes = { Globals.AXIS_VECTOR_X, Globals.AXIS_VECTOR_Y, Globals.AXIS_VECTOR_Z };
            foreach (Vector3 axis in axes)
            {
                MoveAxis(velocity, axis, out bool collided);
                if (axis == Globals.AXIS_VECTOR_Y) onGround = collided && velocity.Y < 0;
                if (!collided) position += velocity * axis;
                else velocity *= axis.InvertUnitVector();
            }

            storedAccel = Vector3.Zero;
        }

        private void MoveAxis(Vector3 movement, Vector3 axis, out bool collided)
        {
            collided = false;
            movement *= axis;

            BlockCoordinates[] testBlocks = world.GetBlocksInAABB(position + movement - new Vector3(3), position + movement + new Vector3(3));

            AABBCollider testAABB = new AABBCollider(position + movement, scale, new Vector3(0, 1, 0));
            Vector3 finalMtv = Vector3.Zero;

            // First check final position and determine order of axis colission checking.
            if (testAABB.IntersectsBlocksMTV(testBlocks) is Vector3 mtv)
            {
                foreach (BlockCoordinates coords in testBlocks)
                {
                    if (testAABB.IntersectsDepth(coords.ToAABB()) is Vector3 depth)
                    {
                        collided = true;
                    }
                }
            }
        }

        public void ApplyForce(Vector3 forceVector)
        {
            storedAccel += forceVector;
        }
    }
}
