using System;
using System.Collections.Generic;
using SFML.Graphics;

public class GameStateManager
{
    private static GameStateManager? _instance;
    
    public static GameStateManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameStateManager();
            }
            return _instance;
        }
    }
    
    private Dictionary<string, IGameState> _states;
    private IGameState? _currentState;
    
    private GameStateManager()
    {
        _states = new Dictionary<string, IGameState>();
        _currentState = null;
    }
    
    public void RegisterState(string name, IGameState state)
    {
        if (_states.ContainsKey(name))
        {
            Console.WriteLine($"State '{name}' already registered. Overwriting.");
        }
        _states[name] = state;
    }
    
    public void SetState(string name)
    {
        if (!_states.ContainsKey(name))
        {
            Console.WriteLine($"State '{name}' not found!");
            return;
        }
        
        IGameState newState = _states[name];
        
        if (_currentState != null)
        {
            _currentState.Shutdown();
        }
        
        _currentState = newState;
        _currentState.Initialize();
    }
    
    public string? GetCurrentStateName()
    {
        if (_currentState == null) return null;
        
        foreach (var kvp in _states)
        {
            if (kvp.Value == _currentState)
            {
                return kvp.Key;
            }
        }
        return null;
    }
    
    public IGameState? GetCurrentState()
    {
        return _currentState;
    }
    
    public IGameState? GetState(string name)
    {
        if (_states.ContainsKey(name))
        {
            return _states[name];
        }
        return null;
    }
    
    public void Update(float deltaTime)
    {
        _currentState?.Update(deltaTime);
    }
    
    public void Draw(RenderWindow window)
    {
        _currentState?.Draw(window);
    }
    
    public void HandleInput()
    {
        _currentState?.HandleInput();
    }
    
    public void Shutdown()
    {
        if (_currentState != null)
        {
            _currentState.Shutdown();
            _currentState = null;
        }
        
        _states.Clear();
    }
}
