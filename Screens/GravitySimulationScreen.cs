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

        public int ScreenWidth => GraphicsDevice.Viewport.Width;
        public int ScreenHeight => GraphicsDevice.Viewport.Height;

        public GravitySimulationScreen(Game game) : base(game) 
        {
            _collisionComponent = new CollisionComponent(new MonoGame.Extended.RectangleF(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));
            _objects = new List<MassiveObject>();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Game.Exit();

            foreach(MassiveObject obj in _objects)
            {
                obj.Update(gameTime);
                ConstrainObject(obj);
            }

            var mouseState = MouseExtended.GetState();

            if (mouseState.WasButtonJustDown(MouseButton.Left))
            {
                var newObj = new MassiveObject(new Vector2(mouseState.X, mouseState.Y), new Random().Next(1, 400), new Vector2(new Random().Next(1, 130), new Random().Next(1, 130)));
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

            _spriteBatch.End();
        }

        private void ConstrainObject(MassiveObject obj)
        {

            var widthHalved = ((RectangleF)obj.Bounds).Width / 2;
            var heightHalved = ((RectangleF)obj.Bounds).Height / 2;
            if (obj.Bounds.Position.X - widthHalved < 0)
            {
                obj.Bounds.Position = new Vector2(widthHalved, obj.Bounds.Position.Y);
                obj.Velocity = new Vector2(obj.Velocity.X * -1, obj.Velocity.Y);
            }

            if (obj.Bounds.Position.X + widthHalved > ScreenWidth)
            {
                obj.Bounds.Position = new Vector2(ScreenWidth - widthHalved, obj.Bounds.Position.Y);
                obj.Velocity = new Vector2(obj.Velocity.X * -1, obj.Velocity.Y);
            }

            if (obj.Bounds.Position.Y - heightHalved < 0)
            {
                obj.Bounds.Position = new Vector2(obj.Bounds.Position.X, heightHalved);
                obj.Velocity = new Vector2(obj.Velocity.X, obj.Velocity.Y * -1);
            }

            if (obj.Bounds.Position.Y + heightHalved > ScreenHeight)
            {
                obj.Bounds.Position = new Vector2(obj.Bounds.Position.X, ScreenHeight - heightHalved);
                obj.Velocity = new Vector2(obj.Velocity.X, obj.Velocity.Y * -1);
            }
        }

    }
}
