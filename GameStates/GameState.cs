using SFML.Graphics;
using SFML.System;
using SFML.Window;

using Physics;

public class GameState : IGameState
{
    private RectangleShape? background;
    
    public void Initialize()
    {
        background = new RectangleShape(new Vector2f(1920, 1080));
        background.FillColor = new Color(100, 50, 50);

        TextObject textObject = new TextObject("GameStateText");
        textObject?.Position = new Vector2f(200, 50);
        textObject?.SetText("Game State - Use WASD to move\nPress ESC to return to menu");
        textObject?.SetColor(Color.White);
        textObject?.SetCharacterSize(24);
        
        Player player = new Player("Player1");
        player?.Position = new Vector2f(960, 540);

        Circle circleCollider = new Circle(25);
        circleCollider.Position = new Vector2f(1160, 540);
        circleCollider.IsStatic = true;
    }
    
    public void Update(float deltaTime)
    {
        HandleInput();
        GameObjectManager.Instance.Update(deltaTime);
        PhysicsManager.Instance.CheckAndResolveCollisions();
    }
    
    public void Draw(RenderWindow window)
    {
        window.Draw(background);
        GameObjectManager.Instance.Draw(window);
        PhysicsManager.Instance.DebugDraw(window); // Call to draw physics debug shapes
    }
    
    public void Shutdown()
    {
        PhysicsManager.Instance.ClearShapes();
        GameObjectManager.Instance.Clear();
        background?.Dispose();
    }
    
    public void HandleInput()
    {
        if (InputManager.Instance.GetKeyDown(Keyboard.Key.Escape))
        {
            GameStateManager.Instance.SetState("menu");
        }
    }
}
