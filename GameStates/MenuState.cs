using SFML.Window;
using SFML.Graphics;
using SFML.System;
using Physics;

public class MenuState : IGameState
{
    private TextObject? titleText;
    private TextObject? instructionText;
    private RectangleShape? background;

    private MouseTrigger? mouseTrigger;
    private Button? startButton;
    private Button? quitButton;
    
    public void Initialize()
    {
        background = new RectangleShape(new Vector2f(1920, 1080))
        {
            FillColor = new Color(50, 50, 100)
        };

        titleText = new TextObject("TitleText");
        titleText.SetText("Game Menu");
        titleText.SetCharacterSize(48);
        titleText.SetColor(Color.White);
        titleText.Position = new Vector2f(960, 300);

        instructionText = new TextObject("InstructionText");
        instructionText.SetText("Press SPACE to start game\nPress ESC to quit");
        instructionText.SetCharacterSize(24);
        instructionText.SetColor(Color.White);
        instructionText.Position = new Vector2f(560, 500);
        
        // Create Start button
        startButton = new Button(new Vector2f(200, 60), "START", "MenuStartButton");
        startButton.NormalColor = new Color(0, 100, 200);
        startButton.HoverColor = new Color(0, 150, 255);
        startButton.ClickColor = new Color(0, 50, 150);
        startButton.OnClick += OnStartClicked;
        startButton.Position = new Vector2f(960, 450);
        
        // Create Quit button
        quitButton = new Button(new Vector2f(200, 60), "QUIT", "MenuQuitButton");
        quitButton.NormalColor = new Color(200, 0, 0);
        quitButton.HoverColor = new Color(255, 0, 0);
        quitButton.ClickColor = new Color(150, 0, 0);
        quitButton.OnClick += OnQuitClicked;
        quitButton.Position = new Vector2f(960, 550);

        // Create mouse trigger for button interactions
        mouseTrigger = new MouseTrigger(5f);
    }
    
    public void Update(float deltaTime)
    {
        // Update mouse trigger and buttons
        GameObjectManager.Instance.Update(deltaTime);
        PhysicsManager.Instance.CheckAndResolveCollisions();
    }
    
    public void Draw(RenderWindow window)
    {
        window.Draw(background);
        
        GameObjectManager.Instance.Draw(window);
        //PhysicsManager.Instance.DebugDraw(window); // Call to draw physics debug shapes
    }
    
    public void Shutdown()
    {
        startButton!.OnClick -= OnStartClicked;
        quitButton!.OnClick -= OnQuitClicked;

        PhysicsManager.Instance.ClearShapes();
        GameObjectManager.Instance.Clear();

        background?.Dispose();
    }
    
    public void HandleInput()
    {
        if (InputManager.Instance.GetKeyDown(Keyboard.Key.Space))
        {
            GameStateManager.Instance.SetState("game");
        }
        
        if (InputManager.Instance.GetKeyDown(Keyboard.Key.Escape))
        {
            GameStateManager.Instance.Shutdown();
            SystemManager.Instance.CloseGame();
        }
    }
    
    private void OnStartClicked()
    {
        GameStateManager.Instance.SetState("game");
    }
    
    private void OnQuitClicked()
    {
        GameStateManager.Instance.Shutdown();
        SystemManager.Instance.CloseGame();
    }
}
