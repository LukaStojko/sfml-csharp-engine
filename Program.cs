using System;
using SFML.Graphics;
using SFML.Window;
using SFML.System;

class Program
{
    static void Main()
    {
        // Create a window
        VideoMode mode = new VideoMode(new Vector2u(1920, 1080));
        RenderWindow window = new RenderWindow(mode, "Tutorium Game Dev");
        window.Closed += (sender, e) => window.Close();

        // Load assets
        AssetManager.Instance.LoadFont("arial", "arial.ttf");

        // Initialize Managers
        SystemManager.Instance.Initialize(window);
        InputManager.Instance.Init(window);
        
        GameStateManager.Instance.RegisterState("menu", new MenuState());
        GameStateManager.Instance.RegisterState("game", new GameState());
        GameStateManager.Instance.SetState("menu");

        // Setup clock for delta time
        Clock clock = new Clock();

        // Main loop
        while (SystemManager.Instance.IsGameRunning())
        {
            // Calculate delta time
            float deltaTime = clock.Restart().AsSeconds();

            // Process events
            window.DispatchEvents();

            // Handle input
            GameStateManager.Instance.HandleInput();
            // Update game state
            GameStateManager.Instance.Update(deltaTime);
            // Draw everything
            GameStateManager.Instance.Draw(window);

            // Update input manager
            InputManager.Instance.Update();

            // Display everything
            window.Display();
        }
    }
}