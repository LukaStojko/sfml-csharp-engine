using SFML.Graphics;
using SFML.System;

public abstract class GameObject
{
    protected Vector2f _position;
    protected float _rotation;
    protected Vector2f _scale;
    
    public virtual Vector2f Position
    {
        get => _position;
        set => _position = value;
    }

    public virtual float Rotation
    {
        get => _rotation;
        set => _rotation = value;
    }

    public virtual Vector2f Scale
    {
        get => _scale;
        set => _scale = value;
    }

    // The unique name identifier for this game object.
    // Can be used for debugging or finding specific objects.
    public virtual string Name { get; set; }

    // Determines if the game object should receive Update calls.
    // When false, the object's Update method will be skipped.
    public virtual bool IsActive { get; set; } = true;
    
    // Determines if the game object should be rendered.
    // When false, the object's Draw method will be skipped.
    public virtual bool IsVisible { get; set; } = true;

    public GameObject(string name = "GameObject")
    {
        Name = name;
    }

    public virtual void Initialize()
    {
        GameObjectManager.Instance.Add(this);
        _position = new Vector2f(0, 0);
        _rotation = 0f;
        _scale = new Vector2f(1f, 1f);
    }

    public virtual void Update(float deltaTime)
    {
        if(!IsActive) return;

        // Must be implemented by derived classes
    }
    public virtual void Draw(RenderWindow window)
    {
        if(!IsVisible) return;

        // Must be implemented by derived classes
    }

    public virtual void Destroy()
    {
        GameObjectManager.Instance.Remove(this);
    }
}
