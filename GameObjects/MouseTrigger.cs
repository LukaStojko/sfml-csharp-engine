using Physics;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class MouseTrigger : GameObject
{
    private Circle? _mouseCollider;
    private float _radius;

    public MouseTrigger(float radius = 5f, string name = "MouseTrigger") : base(name)
    {
        _radius = radius;
        Initialize();
    }

    public override void Initialize()
    {
        base.Initialize();
        
        _mouseCollider = new Circle(_radius);
        _mouseCollider.IsTrigger = true; // Mouse trigger is always a trigger
        _mouseCollider.Owner = this;
        
        // Initialize position at center
        Vector2i mousePos = InputManager.Instance.GetMousePosition();
        _position = new Vector2f(mousePos.X, mousePos.Y);
        _mouseCollider.Position = _position;
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        
        // Update position to follow mouse
        Vector2i mousePos = InputManager.Instance.GetMousePosition();
        _position = new Vector2f(mousePos.X, mousePos.Y);
        
        // Update collider position
        if (_mouseCollider != null)
        {
            _mouseCollider.Position = _position;
        }
    }

    public override void Draw(RenderWindow window)
    {
        base.Draw(window);
        
        // Optional: Draw a small visual indicator for debugging
        // Usually mouse trigger would be invisible
        if (_mouseCollider != null)
        {
            _mouseCollider.DebugDraw(window, new Color(255, 255, 0, 128)); // Semi-transparent yellow
        }
    }

    public override void Destroy()
    {
        base.Destroy();
        // Circle collider is cleaned up by PhysicsManager
    }
    
    // Helper method to check if mouse is hovering over a specific game object
    public bool IsHoveringOverGameObject(string gameObjectName)
    {
        if (_mouseCollider != null)
        {
            return PhysicsManager.Instance.IsTriggerCollidingWithGameObject(_mouseCollider, gameObjectName);
        }
        return false;
    }
    
    // Helper method to check if mouse was clicked on a specific game object
    public bool IsClickedOnGameObject(string gameObjectName)
    {
        return IsHoveringOverGameObject(gameObjectName) && InputManager.Instance.GetMouseButtonDown(Mouse.Button.Left);
    }
}
