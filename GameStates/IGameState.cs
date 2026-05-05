using SFML.Graphics;

public interface IGameState
{
    void Initialize();
    void Update(float deltaTime);
    void Draw(RenderWindow window);
    void Shutdown();
    void HandleInput();
}
