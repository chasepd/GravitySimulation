using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GravitySimulation.Objects
{
    internal class MassiveObject : ICollisionActor
    {
        public Vector2 Velocity { get; set; }
        public IShapeF Bounds { get; }
        public float Mass { get; set; }
        private float collisionTransferEfficency = 0.99f;
        protected MassiveObject _mostRecentCollision;

        public MassiveObject(Vector2 position, float mass, Vector2 startingVelocity)
        {
            _mostRecentCollision = null;
            Mass = mass;
            Bounds = new RectangleF(position.X, position.Y, (float)Math.Sqrt(Mass / Math.PI), (float)Math.Sqrt(Mass / Math.PI));//new CustomEllipseF(position, (float)Math.Sqrt(Mass / Math.PI), (float)Math.Sqrt(Mass / Math.PI));
            Velocity = startingVelocity;
        }

        public void Update(GameTime gameTime)
        {
            Bounds.Position += Velocity * gameTime.GetElapsedSeconds();
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawCircle(Bounds.Position, 
                (float)Math.Sqrt(Mass / Math.PI),
                100,
                Color.DimGray,
                thickness: (float)Math.Sqrt(Mass / Math.PI));
        }
        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            var collisionObject = this;
            var otherObject = (MassiveObject)collisionInfo.Other;

            if (otherObject._mostRecentCollision != this)
            {

                var otherVelocity = otherObject.Velocity;
                var thisVelocity = collisionObject.Velocity;

                var thisVelocityDelta = otherVelocity * (otherObject.Mass / collisionObject.Mass);
                var otherVelocityDelta = thisVelocity * (collisionObject.Mass / collisionObject.Mass);
                collisionObject.Velocity += thisVelocityDelta * collisionTransferEfficency;
                collisionObject.Velocity -= otherVelocityDelta;
                otherObject.Velocity += otherVelocityDelta * collisionTransferEfficency;
                otherObject.Velocity -= thisVelocityDelta;

                _mostRecentCollision = otherObject;
            }
            else
            {
                otherObject._mostRecentCollision = null;
            }
        }
    }
}
