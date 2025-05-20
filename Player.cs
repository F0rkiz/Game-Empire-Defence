using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Empire_Defence
{
    public class Player
    {
        public Vector2 Position;
        private Texture2D _texture;
        private Vector2 _velocity;
        private float _speed = 150f;
        private float _jumpForce = -350f;

        private bool _isGrounded;
        private float _gravity = 1000f;
        private float _groundLevel = 350f; 

        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("player_idle"); 
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.A))
                _velocity.X = -_speed;
            else if (ks.IsKeyDown(Keys.D))
                _velocity.X = _speed;
            else
                _velocity.X = 0;

            if (ks.IsKeyDown(Keys.W) && _isGrounded)
            {
                _velocity.Y = _jumpForce;
                _isGrounded = false;
            }
            _velocity.Y += _gravity * dt;
            Position += _velocity * dt;


            if (Position.Y >= _groundLevel - _texture.Height)
            {
                Position.Y = _groundLevel - _texture.Height;
                _velocity.Y = 0;
                _isGrounded = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, Color.White);
        }
    }
}
