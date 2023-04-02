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

        protected float _radius;

        private const float consumeAmount = 800f;

        public List<MassiveObject> ObjectsConsuming { get; set; }

        public MassiveObject(Vector2 position, float mass, Vector2 startingVelocity)
        {
            Mass = mass;
            _radius = (float)Math.Sqrt(Mass / Math.PI);
            Bounds = new CircleF(new Point2(position.X, position.Y), _radius);
            Velocity = startingVelocity;
            ObjectsConsuming = new List<MassiveObject>();
        }

        public void Update(GameTime gameTime)
        {
            if (ObjectsConsuming.Count > 0)
            {
                foreach (var consumedObject in ObjectsConsuming)
                {
                    var amount = (float)(consumeAmount * gameTime.ElapsedGameTime.TotalSeconds);
                    consumedObject.Velocity = Velocity;
                    Mass += amount;
                    consumedObject.Mass -= amount;
                    _radius = (float)Math.Sqrt(Mass / Math.PI);
                    Bounds = new CircleF(new Point2(Bounds.Position.X, Bounds.Position.Y), _radius);
                    consumedObject._radius = (float)Math.Sqrt(consumedObject.Mass / Math.PI);
                    consumedObject.Bounds = new CircleF(new Point2(consumedObject.Bounds.Position.X, consumedObject.Bounds.Position.Y), consumedObject._radius);
                }               
            }

            Bounds.Position += Velocity * gameTime.GetElapsedSeconds();
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawCircle((CircleF)Bounds, 100, Color.DarkSlateGray, thickness:_radius);
        }
        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            var otherObject = (MassiveObject)collisionInfo.Other;
            if (!ObjectsConsuming.Contains(otherObject))
            {
                if (Mass > otherObject.Mass)
                {
                    ObjectsConsuming.Add(otherObject);
                }
            }
        }
    }
}
