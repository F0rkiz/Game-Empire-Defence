using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Empire_Defence
{
    public class Player : IDamageable
    {
        public Vector2 Position { get; set; }
        private Texture2D _texture;
        private Vector2 _velocity;
        private float _speed = 150f;
        private float _jumpForce = -350f;

        private bool _isGrounded;
        private float _gravity = 1000f;
        private float _groundLevel = 800f;

        public int HP { get; set; } = 100;
        public int MaxHP { get; set; } = 100;
        public bool IsAlive => HP > 0;

        public void TakeDamage(int amount)
        {
            HP -= amount;
        }

        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("player_idle");
        }
        public void Respawn()
        {
            HP = MaxHP;
            isDead = false;
            Position = new Vector2(700, _groundLevel - _texture.Height);
        }

        public void Update(GameTime gameTime, int mapWidth)
        {
            if (!IsAlive)
            {
                respawnTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (respawnTimer <= 0)
                {
                    Respawn();
                }

                return;
            }

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

            Position = new Vector2(
                MathHelper.Clamp(Position.X, 0, mapWidth - _texture.Width),
                Position.Y
            );

            if (Position.Y >= _groundLevel - _texture.Height)
            {
                Position = new Vector2(Position.X, _groundLevel - _texture.Height);
                _velocity.Y = 0;
                _isGrounded = true;
            }
        }
        private float respawnTimer = 0f;
        private bool isDead = false;

        public void Kill()
        {
            isDead = true;
            respawnTimer = 10f;
        }

        public Texture2D Texture => _texture;

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsAlive)
                spriteBatch.Draw(_texture, Position, Color.White);
        }
    }
}
