using SFML.Graphics;

public class SystemManager
{
    private static SystemManager? instance;
    
    public static SystemManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SystemManager();
            }
            return instance;
        }
    }
    
    private RenderWindow? window;
    private bool isRunning;
    
    private SystemManager()
    {
        window = null;
        isRunning = true;
    }
    
    public void Initialize(RenderWindow gameWindow)
    {
        window = gameWindow;
        isRunning = true;
    }
    
    public void CloseGame()
    {
        if (window != null)
        {
            window.Close();
        }
        isRunning = false;
    }
    
    public bool IsGameRunning()
    {
        return isRunning && (window?.IsOpen ?? false);
    }
}
