using SFML.Audio;
using SFML.Graphics;

public class AssetManager
{
    private static AssetManager? instance;
    
    public static AssetManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AssetManager();
            }
            return instance;
        }
    }
    
    private readonly Dictionary<string, Texture> _textures = new();
    private readonly Dictionary<string, Font> _fonts = new();
    private readonly Dictionary<string, SoundBuffer> _soundBuffers = new();
    
    private AssetManager() { }
    
    public void LoadTexture(string key, string filePath)
    {
        if (!_textures.ContainsKey(key))
        {
            var texture = new Texture(filePath);
            _textures[key] = texture;
        }
    }
    
    public Texture? GetTexture(string key)
    {
        _textures.TryGetValue(key, out var texture);
        return texture;
    }
    
    public void LoadFont(string key, string filePath)
    {
        if (!_fonts.ContainsKey(key))
        {
            var font = new Font(filePath);
            _fonts[key] = font;
        }
    }
    
    public Font? GetFont(string key)
    {
        _fonts.TryGetValue(key, out var font);
        return font;
    }
    
    public void LoadSound(string key, string filePath)
    {
        if (!_soundBuffers.ContainsKey(key))
        {
            var soundBuffer = new SoundBuffer(filePath);
            _soundBuffers[key] = soundBuffer;
        }
    }
    
    public SoundBuffer? GetSoundBuffer(string key)
    {
        _soundBuffers.TryGetValue(key, out var soundBuffer);
        return soundBuffer;
    }
    
    public Sound? CreateSound(string key)
    {
        var soundBuffer = GetSoundBuffer(key);
        return soundBuffer != null ? new Sound(soundBuffer) : null;
    }
    
    public void Shutdown()
    {
        _textures.Clear();
        _fonts.Clear();
        _soundBuffers.Clear();
    }
}
