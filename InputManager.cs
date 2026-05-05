
using SFML.Window;
using SFML.System;

public class InputManager
{
    private static InputManager? instance;

    public static InputManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new InputManager();
            }
            return instance;
        }
    }
    
    private InputManager()
    {
        
    }

    private Dictionary<Keyboard.Key, bool> isKeyPressed = new Dictionary<Keyboard.Key, bool>();
    private Dictionary<Keyboard.Key, bool> isKeyDown = new Dictionary<Keyboard.Key, bool>();
    private Dictionary<Keyboard.Key, bool> isKeyUp = new Dictionary<Keyboard.Key, bool>();
    private Dictionary<Mouse.Button, bool> isMouseButtonPressed = new Dictionary<Mouse.Button, bool>();
    private Dictionary<Mouse.Button, bool> isMouseButtonDown = new Dictionary<Mouse.Button, bool>();
    private Dictionary<Mouse.Button, bool> isMouseButtonUp = new Dictionary<Mouse.Button, bool>();
    
    private Vector2i mousePosition;
    
    public void Init(Window window)
    {
        window.SetKeyRepeatEnabled(false);
        
        isKeyPressed.Add(Keyboard.Key.W, false);
        isKeyPressed.Add(Keyboard.Key.A, false);
        isKeyPressed.Add(Keyboard.Key.S, false);
        isKeyPressed.Add(Keyboard.Key.D, false);
        isKeyPressed.Add(Keyboard.Key.R, false);
        isKeyPressed.Add(Keyboard.Key.G, false);
        isKeyPressed.Add(Keyboard.Key.Q, false);
        isKeyPressed.Add(Keyboard.Key.Space, false);
        isMouseButtonPressed.Add(Mouse.Button.Left, false);

        isKeyDown.Add(Keyboard.Key.W, false);
        isKeyDown.Add(Keyboard.Key.A, false);
        isKeyDown.Add(Keyboard.Key.S, false);
        isKeyDown.Add(Keyboard.Key.D, false);
        isKeyDown.Add(Keyboard.Key.R, false);
        isKeyDown.Add(Keyboard.Key.G, false);
        isKeyDown.Add(Keyboard.Key.Q, false);
        isKeyDown.Add(Keyboard.Key.Space, false);
        isMouseButtonDown.Add(Mouse.Button.Left, false);
        
        isKeyUp.Add(Keyboard.Key.W, false);
        isKeyUp.Add(Keyboard.Key.A, false);
        isKeyUp.Add(Keyboard.Key.S, false);
        isKeyUp.Add(Keyboard.Key.D, false);
        isKeyUp.Add(Keyboard.Key.R, false);
        isKeyUp.Add(Keyboard.Key.G, false);
        isKeyUp.Add(Keyboard.Key.Q, false);
        isKeyUp.Add(Keyboard.Key.Space, false);
        isMouseButtonUp.Add(Mouse.Button.Left, false);

        window.KeyPressed += OnKeyPressed;
        window.KeyReleased += OnKeyReleased;

        window.MouseButtonPressed += OnMouseButtonPressed;
        window.MouseButtonReleased += OnMouseButtonReleased;
        window.MouseMoved += OnMouseMoved;
    }

    public void Update()
    {
        foreach(KeyValuePair<Keyboard.Key, bool> Kvp in isKeyDown)
        {
            isKeyDown[Kvp.Key] = false;
        }
        foreach(KeyValuePair<Keyboard.Key, bool> Kvp in isKeyUp)
        {
            isKeyUp[Kvp.Key] = false;
        }

        foreach(KeyValuePair<Mouse.Button, bool> Kvp in isMouseButtonDown)
        {
            isMouseButtonDown[Kvp.Key] = false;
        }
        foreach(KeyValuePair<Mouse.Button, bool> Kvp in isMouseButtonUp)
        {
            isMouseButtonUp[Kvp.Key] = false;
        }
    }

    private void CloseGameEscape(object? sender, KeyEventArgs e)
    {
        if(sender == null) return;

        Window window = (Window)sender;
        if (e.Code == Keyboard.Key.Escape)
        {
            window.Close();
        }
    }

    private void OnKeyPressed(object? sender, KeyEventArgs e)
    {
        isKeyDown[e.Code] = true;

        isKeyPressed[e.Code] = true;
    }
    private void OnKeyReleased(object? sender, KeyEventArgs e)
    {
        isKeyPressed[e.Code] = false;
        
        isKeyUp[e.Code] = true;
    }

    private void OnMouseButtonPressed(object? sender, MouseButtonEventArgs e)
    {
        isMouseButtonDown[e.Button] = true;

        isMouseButtonPressed[e.Button] = true;
    }
    private void OnMouseButtonReleased(object? sender, MouseButtonEventArgs e)
    {
        isMouseButtonPressed[e.Button] = false;
        
        isMouseButtonUp[e.Button] = true;
    }
    
    private void OnMouseMoved(object? sender, MouseMoveEventArgs e)
    {
        mousePosition = new Vector2i((int)e.Position.X, (int)e.Position.Y);
    }

    public bool GetKeyPressed(Keyboard.Key key)
    {
        if(isKeyPressed.ContainsKey(key) && isKeyPressed[key] == true)
        {
            return true;
        }
        return false;
    }
    public bool GetKeyDown(Keyboard.Key key)
    {
        if(isKeyDown.ContainsKey(key) && isKeyDown[key] == true)
        {
            return true;
        }
        return false;
    }
    public bool GetKeyUp(Keyboard.Key key)
    {
        if(isKeyUp.ContainsKey(key) && isKeyUp[key] == true)
        {
            return true;
        }
        return false;
    }

    public bool GetMouseButtonPressed(Mouse.Button key)
    {
        if(isMouseButtonPressed.ContainsKey(key) && isMouseButtonPressed[key] == true)
        {
            return true;
        }
        return false;
    }
    public bool GetMouseButtonDown(Mouse.Button key)
    {
        if(isMouseButtonDown.ContainsKey(key) && isMouseButtonDown[key] == true)
        {
            return true;
        }
        return false;
    }
    public bool GetMouseButtonUp(Mouse.Button key)
    {
        if(isMouseButtonUp.ContainsKey(key) && isMouseButtonUp[key] == true)
        {
            return true;
        }
        return false;
    }
    
    public Vector2i GetMousePosition()
    {
        return mousePosition;
    }
}