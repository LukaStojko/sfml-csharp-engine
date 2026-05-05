using SFML.Graphics;

public class GameObjectManager
{
    private static GameObjectManager? instance;

    public static GameObjectManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObjectManager();
            }
            return instance;
        }
    }

    private readonly Dictionary<string, GameObject> gameObjects;

    private GameObjectManager()
    {
        gameObjects = new Dictionary<string, GameObject>();
    }

    public void Add(GameObject gameObject)
    {
        if (gameObject != null && !gameObjects.ContainsKey(gameObject.Name))
        {
            gameObjects[gameObject.Name] = gameObject;
        }
    }

    public void Remove(GameObject gameObject)
    {
        if (gameObject != null)
        {
            gameObjects.Remove(gameObject.Name);
        }
    }

    public void RemoveByName(string name)
    {
        gameObjects.Remove(name);
    }

    public GameObject? FindObjectByName(string name)
    {
        gameObjects.TryGetValue(name, out GameObject? gameObject);
        return gameObject;
    }

    public void Update(float deltaTime)
    {
        var gameObjectsCopy = new List<GameObject>(gameObjects.Values);
        foreach (var gameObject in gameObjectsCopy)
        {
            gameObject.Update(deltaTime);
        }
    }

    public void Draw(RenderWindow window)
    {
        var gameObjectsCopy = new List<GameObject>(gameObjects.Values);
        foreach (var gameObject in gameObjectsCopy)
        {
            gameObject.Draw(window);
        }
    }

    public void Clear()
    {
        var gameObjectsCopy = new List<GameObject>(gameObjects.Values);
        foreach (var gameObject in gameObjectsCopy)
        {
            gameObject.Destroy();
        }
        gameObjects.Clear();
    }

    public int GetGameObjectCount()
    {
        return gameObjects.Count;
    }
}
