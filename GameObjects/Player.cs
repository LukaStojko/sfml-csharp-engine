using Physics;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class Player : GameObject
{
    public Vector2f _velocity;

    private CircleShape? _playerVisual;

    private Circle? _circleCollider;

    public Player(string name = "Player") : base(name)
    {
        Initialize();
    }
    
    public override Vector2f Position
    {
        get => _position;
        set
        {
            _position = value;
            _circleCollider!.Position = value;
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        _playerVisual = new CircleShape(25);
        _playerVisual?.Origin = new Vector2f(25, 25);
        _playerVisual?.Scale = new Vector2f(1, 1);
        _playerVisual?.FillColor = Color.Yellow;

        _circleCollider = new Circle(25);
        _circleCollider!.Owner = this;
        _circleCollider!.Position = _position;
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        _velocity = new Vector2f(0, 0);
        float speed = 300f;
        
        if (InputManager.Instance.GetKeyPressed(Keyboard.Key.W))
        {
            _velocity.Y -= speed;
        }
        if (InputManager.Instance.GetKeyPressed(Keyboard.Key.S))
        {
            _velocity.Y += speed;
        }
        if (InputManager.Instance.GetKeyPressed(Keyboard.Key.A))
        {
            _velocity.X -= speed;
        }
        if (InputManager.Instance.GetKeyPressed(Keyboard.Key.D))
        {
            _velocity.X += speed;
        }
        // Normalize diagonal movement to prevent speed boost
        if (_velocity.X != 0 && _velocity.Y != 0)
        {
            float length = (float)Math.Sqrt(
                _velocity.X * _velocity.X +
                _velocity.Y * _velocity.Y
            );
            _velocity.X = (_velocity.X / length) * speed;
            _velocity.Y = (_velocity.Y / length) * speed;
        }
        
        _position += _velocity * deltaTime;
        
        Vector2f pos = _position;

        // Keep player within window bounds (assuming 1920x1080 and player radius of 25)
        // In reality, you'd have actual physics collision with e.g. walls instead of hard clamping
        if (pos.X < 25) pos.X = 25;
        if (pos.X > 1920 - 25) pos.X = 1920 - 25;
        if (pos.Y < 25) pos.Y = 25;
        if (pos.Y > 1080 - 25) pos.Y = 1080 - 25;
        _position = pos;
        
        // IMPORTANT: Update collider position with player movement
        _circleCollider?.Position = _position;
        
        // Example: Check if player is colliding with objects named "Danger" (physical collision)
        if (PhysicsManager.Instance.IsCollidingWithGameObject(_circleCollider!, "Danger"))
        {
            // Handle collision with danger object (e.g., take damage, respawn, etc.)
            // For now, let's just change the player color to red
            _playerVisual!.FillColor = Color.Red;
        }
        // Example: Check if player is colliding with trigger objects named "Checkpoint" (trigger collision)
        else if (PhysicsManager.Instance.IsTriggerCollidingWithGameObject(_circleCollider!, "Checkpoint"))
        {
            // Handle trigger collision (e.g., save game, play sound, etc.)
            // For now, let's change the player color to green
            _playerVisual!.FillColor = Color.Green;
        }
        else
        {
            // Reset color when not colliding
            _playerVisual!.FillColor = Color.Yellow;
        }
    }

    public override void Draw(RenderWindow window)
    {
        base.Draw(window);

        // IMPORTANT: Sync visual position from collider (after physics has potentially modified it)
        _position = _circleCollider!.Position;

        // Draw the player sprite
        _playerVisual?.Position = _position;
        _playerVisual?.Rotation = _rotation;
        window.Draw(_playerVisual);
    }

    public override void Destroy()
    {
        base.Destroy();
        _playerVisual?.Dispose();
    }
}