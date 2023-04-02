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
        public IShapeF Bounds { get; set; }
        public float Mass { get; set; }

        private float consumeAmount = 0f;

        public MassiveObject ConsumedObject { get; set; }

        public MassiveObject(Vector2 position, float mass, Vector2 startingVelocity)
        {
            Mass = mass;
            Bounds = new RectangleF(position.X, position.Y, (float)Math.Sqrt(Mass / Math.PI), (float)Math.Sqrt(Mass / Math.PI));//new CustomEllipseF(position, (float)Math.Sqrt(Mass / Math.PI), (float)Math.Sqrt(Mass / Math.PI));
            Velocity = startingVelocity;
        }

        public void Update(GameTime gameTime)
        {
            if (ConsumedObject != null && ConsumedObject.Mass > 0)
            {
                var amount = (float)(consumeAmount * gameTime.ElapsedGameTime.TotalSeconds);
                ConsumedObject.Velocity = Velocity;
                Mass += amount;
                ConsumedObject.Mass -= amount;

                Bounds = new RectangleF(Bounds.Position.X, Bounds.Position.Y, (float)Math.Sqrt(Mass / Math.PI), (float)Math.Sqrt(Mass / Math.PI));
                ConsumedObject.Bounds = new RectangleF(ConsumedObject.Bounds.Position.X, ConsumedObject.Bounds.Position.Y, (float)Math.Sqrt(ConsumedObject.Mass / Math.PI), (float)Math.Sqrt(ConsumedObject.Mass / Math.PI));

            }
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
            if (otherObject != ConsumedObject)
            {
                if (Mass > otherObject.Mass)
                {
                    ConsumedObject = otherObject;
                    consumeAmount = ConsumedObject.Mass / 2;
                }
            }
            

        }
    }
}
