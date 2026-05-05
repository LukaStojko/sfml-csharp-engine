using Physics;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class Button : GameObject
{
    // Visual representation of the button
    private RectangleShape? _buttonVisual;
    private Text? _buttonText;

    // Collider for detecting mouse interactions
    private AABB? _buttonCollider;
    
    // Customizable properties
    public Color NormalColor { get; set; } = new Color(100, 100, 100);
    public Color HoverColor { get; set; } = new Color(150, 150, 150);
    public Color ClickColor { get; set; } = new Color(80, 80, 80);
    public Color TextColor { get; set; } = Color.White;
    public string ButtonText { get; set; } = "Button";
    public uint FontSize { get; set; } = 24;
    
    // Actions that get invoked on button events, can be subscribed to from outside
    // This is the C# version of the Observer pattern
    public event Action? OnClick;
    public event Action? OnHover;
    public event Action? OnHoverExit;

    public Button(Vector2f size, string text = "Button", string name = "Button") : base(name)
    {
        ButtonText = text;
        Initialize(size);
    }
    public Button(Vector2f position, Vector2f size, string text = "Button", string name = "Button") : base(name)
    {
        _position = position;
        ButtonText = text;
        Initialize(size);
    }

    private void Initialize(Vector2f size)
    {
        base.Initialize();
        
        // Create button visual
        _buttonVisual = new RectangleShape(size);
        _buttonVisual.Origin = size / 2f;
        _buttonVisual.FillColor = NormalColor;
        _buttonVisual.OutlineColor = Color.White;
        _buttonVisual.OutlineThickness = 2.0f;

        // Create button collider (trigger)
        _buttonCollider = new AABB(size);
        _buttonCollider.Position = _position;
        _buttonCollider.IsTrigger = true; // Buttons are triggers
        _buttonCollider.Owner = this;

        // Load font and create text
        Font? font = AssetManager.Instance.GetFont("arial");
        _buttonText = new Text(font, ButtonText)
        {
            CharacterSize = (uint)FontSize
        };
        _buttonText.FillColor = TextColor;
        
        // Center text in button
        FloatRect textBounds = _buttonText.GetLocalBounds();
        _buttonText.Origin = new Vector2f(textBounds.Left + textBounds.Width / 2f, 
                                         textBounds.Top + textBounds.Height / 2f);
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        
        // Update collider position
        if (_buttonCollider != null)
        {
            _buttonCollider.Position = _position;
        }
        
        // Check for mouse interactions
        CheckMouseInteractions();
    }

    private void CheckMouseInteractions()
    {
        if (_buttonCollider == null) return;
        
        // Check if mouse is hovering over button
        bool isHovering = PhysicsManager.Instance.IsTriggerCollidingWithGameObject(_buttonCollider, "MouseTrigger");
        
        if (isHovering)
        {
            // Mouse is hovering
            if (_buttonVisual != null)
            {
                _buttonVisual.FillColor = InputManager.Instance.GetMouseButtonPressed(Mouse.Button.Left) ? ClickColor : HoverColor;
            }
            
            // Check for click
            if (InputManager.Instance.GetMouseButtonDown(Mouse.Button.Left))
            {
                OnClick?.Invoke();
            }
            
            // Hover event (only trigger once)
            if (_buttonVisual != null && _buttonVisual.FillColor != ClickColor)
            {
                OnHover?.Invoke();
            }
        }
        else
        {
            // Mouse is not hovering
            if (_buttonVisual != null && _buttonVisual.FillColor != NormalColor)
            {
                _buttonVisual.FillColor = NormalColor;
                OnHoverExit?.Invoke();
            }
        }
    }

    public override void Draw(RenderWindow window)
    {
        base.Draw(window);

        // Draw button background
        if (_buttonVisual != null)
        {
            _buttonVisual.Position = _position;
            _buttonVisual.Rotation = _rotation;
            window.Draw(_buttonVisual);
        }
        
        // Draw button text
        if (_buttonText != null)
        {
            _buttonText.Position = _position;
            _buttonText.Rotation = _rotation;
            window.Draw(_buttonText);
        }
    }

    public override void Destroy()
    {
        base.Destroy();
    }
    
    public void SetText(string newText)
    {
        ButtonText = newText;
        if (_buttonText != null)
        {
            _buttonText.DisplayedString = newText;
            // Re-center text
            FloatRect textBounds = _buttonText.GetLocalBounds();
            _buttonText.Origin = new Vector2f(textBounds.Left + textBounds.Width / 2f, 
                                             textBounds.Top + textBounds.Height / 2f);
        }
    }
}
