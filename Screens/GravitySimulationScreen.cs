using GravitySimulation.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GravitySimulation.Screens
{
    internal class GravitySimulationScreen : GameScreen
    {
        private List<MassiveObject> _objects;
        private readonly CollisionComponent _collisionComponent;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private List<MassiveObject> _objectsToRemove;
        private const float _gravitationalConstant = 5f;

        public int ScreenWidth => GraphicsDevice.Viewport.Width;
        public int ScreenHeight => GraphicsDevice.Viewport.Height;

        public GravitySimulationScreen(Game game) : base(game) 
        {
            _collisionComponent = new CollisionComponent(new MonoGame.Extended.RectangleF(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));
            _objects = new List<MassiveObject>();
            _objectsToRemove = new List<MassiveObject>();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("Arial");

            var newObj = new MassiveObject(new Vector2(ScreenWidth / 2, ScreenHeight / 2), 2000, new Vector2(0, 0));
            _objects.Add(newObj);
            _collisionComponent.Insert(newObj);

            newObj = new MassiveObject(new Vector2(newObj.Bounds.Position.X - 100, newObj.Bounds.Position.Y - 100), 100, new Vector2(0, 5));
            _objects.Add(newObj);
            _collisionComponent.Insert(newObj);
        }

        public override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Game.Exit();

            foreach(MassiveObject obj in _objects)
            {
                obj.Update(gameTime);
                if(obj.ConsumedObject != null)
                {
                    if (obj.ConsumedObject.Mass <= 0)
                    {
                        _objectsToRemove.Add(obj.ConsumedObject);
                        obj.ConsumedObject = null;
                    }
                }               
                CalculateGravitationalPulls(obj, gameTime.ElapsedGameTime.TotalSeconds);
                CheckForOffScreen(obj);
            }

            foreach(var obj in _objectsToRemove)
            {
                _objects.Remove(obj);
                _collisionComponent.Remove(obj);
            }

            var mouseState = MouseExtended.GetState();

            if (mouseState.WasButtonJustDown(MouseButton.Left))
            {
                var newObj = new MassiveObject(new Vector2(mouseState.X, mouseState.Y), new Random().Next(1, 4000), new Vector2(new Random().Next(-10, 10), new Random().Next(-10, 10)));
                _objects.Add(newObj);
                _collisionComponent.Insert(newObj);
            }

            _collisionComponent.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            foreach(var obj in _objects)
            {
                obj.Draw(_spriteBatch);
            }

            _spriteBatch.DrawString(_font, "Click Anywhere to Add an Object", new Vector2(20, 20), Color.WhiteSmoke);

            _spriteBatch.End();
        }

        private void CheckForOffScreen(MassiveObject obj)
        {

            var widthDistanceEliminationTarget = ScreenWidth * 2;
            var heightDistanceEliminationTarget = ScreenHeight * 2;
            if (obj.Bounds.Position.X + widthDistanceEliminationTarget < 0 ||            
                obj.Bounds.Position.X - widthDistanceEliminationTarget > ScreenWidth ||
                obj.Bounds.Position.Y + heightDistanceEliminationTarget < 0 ||
                obj.Bounds.Position.Y - heightDistanceEliminationTarget > ScreenHeight)
            {
                _objectsToRemove.Add(obj);
            }
        }

        private void CalculateGravitationalPulls(MassiveObject obj, double elapsedTime)
        {
            foreach (var otherObj in _objects)
            {
                if (otherObj == obj) continue;
                var gravityAcceleration = (float)(otherObj.Mass * _gravitationalConstant / (Math.Pow(Math.Abs(Vector2.Distance(obj.Bounds.Position, otherObj.Bounds.Position)), 2)));

                Vector2 accelerationVector = new Vector2(obj.Bounds.Position.X < otherObj.Bounds.Position.X? 1:-1, obj.Bounds.Position.Y < otherObj.Bounds.Position.Y ? 1 : -1);
                accelerationVector *= gravityAcceleration;

                obj.Velocity += accelerationVector * (float)elapsedTime;
            }
        }

    }
}
