# Game Engine Project Guide

## Overview
This is a simple 2D game engine built with C# and SFML.Net. It provides basic game development functionality with a component-based architecture.

## Technology Stack
- **Language**: C# (.NET 10.0)
- **Graphics Library**: SFML.Net 3.0.0

## Project Structure

### Core Systems
- **Program.cs** - Main entry point and game loop
- **SystemManager.cs** - Controls game state and window management
- **AssetManager.cs** - Handles loading of fonts, textures, and sounds
- **InputManager.cs** - Processes keyboard and mouse input

### Game Architecture
- **GameObjects/** - All game objects inherit from GameObject base class
- **GameStates/** - Different game states (Menu, Game, etc.)
- **Physics/** - Collision detection and physics calculations

### Key Classes
- **GameObject** - Base class for all game objects
- **GameObjectManager** - Manages all game objects in the scene
- **GameStateManager** - Handles switching between game states
- **PhysicsManager** - Manages collision detection

## Detailed Class and Manager Explanations

### Core Managers

#### SystemManager
- **Purpose**: Controls the main game window and running state
- **Key Functions**:
  - `Initialize(window)` - Sets up the game window
  - `CloseGame()` - Safely closes the game
  - `IsGameRunning()` - Returns true while game should continue
- **Usage**: Singleton pattern, accessed via `SystemManager.Instance`

#### GameObjectManager
- **Purpose**: Manages all game objects in the current scene
- **Key Functions**:
  - `Add(gameObject)` - Adds a game object to the scene
  - `Remove(gameObject)` - Removes a specific game object
  - `FindObjectByName(name)` - Finds a game object by its name
  - `Update(deltaTime)` - Updates all active game objects
  - `Draw(window)` - Renders all visible game objects
  - `Clear()` - Removes all game objects from the scene
  - `GetGameObjectCount()` - Returns the number of objects in the scene
- **Usage**: Automatically manages object lifecycle, prevents modification during iteration

#### GameStateManager
- **Purpose**: Handles switching between different game states (menu, gameplay, etc.)
- **Key Functions**:
  - `RegisterState(name, state)` - Registers a new game state
  - `SetState(name)` - Switches to a specific game state
  - `GetCurrentState()` - Returns the currently active state
  - `Update(deltaTime)` - Updates the current state
  - `Draw(window)` - Renders the current state
  - `HandleInput()` - Processes input for the current state
- **Usage**: Manages state transitions, calls shutdown on previous state before switching

#### InputManager
- **Purpose**: Handles all keyboard and mouse input with event-based tracking
- **Key Functions**:
  - `GetKeyPressed(key)` - Returns true if key is currently held down
  - `GetKeyDown(key)` - Returns true only on the frame key was pressed
  - `GetKeyUp(key)` - Returns true only on the frame key was released
  - `GetMouseButtonPressed(button)` - Returns true if mouse button is held
  - `GetMouseButtonDown(button)` - Returns true only on mouse button press
  - `GetMouseButtonUp(button)` - Returns true only on mouse button release
  - `GetMousePosition()` - Returns current mouse cursor position
- **Usage**: Pre-configured for WASD movement, Space, R, G, Q keys and left mouse button

#### AssetManager
- **Purpose**: Loads and manages game assets (textures, fonts, sounds)
- **Key Functions**:
  - `LoadTexture(key, filePath)` - Loads a texture with a string key
  - `GetTexture(key)` - Retrieves a loaded texture
  - `LoadFont(key, filePath)` - Loads a font with a string key
  - `GetFont(key)` - Retrieves a loaded font
  - `LoadSound(key, filePath)` - Loads a sound buffer with a string key
  - `GetSoundBuffer(key)` - Retrieves a loaded sound buffer
  - `CreateSound(key)` - Creates a playable sound from a sound buffer
- **Usage**: Prevents duplicate loading, assets stored in dictionaries with string keys

#### PhysicsManager
- **Purpose**: Handles collision detection and resolution between physics shapes
- **Key Functions**:
  - `AddShape(shape)` - Adds a physics shape to the simulation
  - `RemoveShape(shape)` - Removes a physics shape from the simulation
  - `CheckAndResolveCollisions()` - Processes all collision pairs
  - `GetCollidingShapes(shape)` - Returns all shapes colliding with given shape
  - `GetCollidingGameObjects(shape)` - Returns all game objects colliding with shape
  - `GetTriggerCollidingShapes(shape)` - Returns trigger-only collisions
  - `IsCollidingWithGameObject(shape, name)` - Checks collision with specific object
- **Supported Shapes**: AABB (axis-aligned bounding box), Circle, OBB (oriented bounding box)
- **Features**: Static vs dynamic objects, trigger collisions, collision events, automatically added to the PhysicsManager during initialization

### Core Classes

#### GameObject (Abstract Base Class)
- **Purpose**: Base class for all game objects with transform properties
- **Properties**:
  - `Position` - World position (Vector2f)
  - `Rotation` - Rotation angle in degrees
  - `Scale` - Scale factor (Vector2f)
  - `Name` - Unique identifier string
  - `IsActive` - Controls whether Update() is called
  - `IsVisible` - Controls whether Draw() is called
- **Methods**:
  - `Initialize()` - Adds object to GameObjectManager
  - `Update(deltaTime)` - Override for game logic
  - `Draw(window)` - Override for rendering
  - `Destroy()` - Removes object from GameObjectManager
- **Usage**: All game entities inherit from this class, automatically added to the GameObjectManager during initialization

#### IGameState (Interface)
- **Purpose**: Defines the contract for game states
- **Methods**:
  - `Initialize()` - Called when state becomes active
  - `Update(deltaTime)` - Called each frame for state logic
  - `Draw(window)` - Called each frame for rendering
  - `HandleInput()` - Called each frame for input processing
  - `Shutdown()` - Called when state becomes inactive
- **Usage**: Implement this interface for menu, gameplay, pause states, etc.

### Required GameState Manager Calls

Every GameState must follow this specific pattern for manager calls:

#### **Update() Method - Required Calls**
```csharp
public void Update(float deltaTime)
{
    GameObjectManager.Instance.Update(deltaTime);
    PhysicsManager.Instance.CheckAndResolveCollisions();
}
```

#### **Draw() Method - Required Calls**
```csharp
public void Draw(RenderWindow window)
{
    GameObjectManager.Instance.Draw(window);
    PhysicsManager.Instance.DebugDraw(window); // Call to draw physics debug shapes
}
```

#### **Shutdown() Method - Required Cleanup**
```csharp
public void Shutdown()
{
    PhysicsManager.Instance.ClearShapes();
    GameObjectManager.Instance.Clear();
}
```

**Critical**: Always clear PhysicsManager before GameObjectManager in Shutdown() to prevent access to destroyed objects.

### Physics Shapes

#### Shape (Base Class)
- **Purpose**: Base class for all collision shapes
- **Properties**:
  - `Position` - Shape position
  - `IsStatic` - If true, shape doesn't move during collision resolution
  - `IsTrigger` - If true, collision detection only (no physical response)
  - `Owner` - Reference to the GameObject that owns this shape
- **Derived Classes**: AABB, Circle, OBB

## How to Work with This Project

### Creating Game Objects
1. Inherit from `GameObject` class
2. Override `Update()` for game logic
3. Override `Draw()` for rendering
4. Call `Initialize()` to add to the game world
5. Use `IsActive` and `IsVisible` to control behavior
6. Add physics shapes if collision detection is needed

### Adding Game States
1. Create a class that implements `IGameState`
2. Implement all required methods (Initialize, Update, Draw, HandleInput, Shutdown)
3. Register it in `Program.cs` using `GameStateManager.Instance.RegisterState()`
4. Set the active state with `GameStateManager.Instance.SetState()`

### Managing Assets
- Use `AssetManager.Instance.LoadTexture()` to load images
- Use `AssetManager.Instance.LoadFont()` to load fonts
- Use `AssetManager.Instance.LoadSound()` to load audio files
- Place asset files in the `resources/` folder
- Access assets using the string keys you provided during loading

### Working with Physics
1. Create physics shapes (AABB, Circle, or OBB)
2. Set the `Owner` property to your GameObject
3. Add shapes to `PhysicsManager.Instance`
4. Use collision detection methods to check for interactions
5. Set `IsTrigger = true` for non-physical collisions

### Connecting GameObjects with Colliders

To connect a GameObject with a collider, you need to establish a two-way relationship between the visual representation and the physics shape.

#### **The Connection Process**

**1. Create a collider field** in your GameObject class - choose Circle, AABB, or OBB based on shape needs.

**2. Initialize the collider** in the Initialize() method. The critical step is setting `collider.Owner = this` which links the physics shape to your GameObject. This enables name-based collision detection. Also set the initial position and whether it's a trigger.

**3. Keep positions synchronized** during Update(). When your GameObject moves, update the collider's position to match: `collider.Position = _position`.

**4. Follow physics resolution** in Draw(). After the physics system processes collisions, the collider position might have changed. Sync your visual back to the collider: `_position = collider.Position`.

**5. Handle position changes** (optional). Override the Position property so when external code moves your GameObject, the collider moves too.

#### **Why This Two-Way Sync?**

The physics system can move colliders during collision resolution. If two objects collide, the physics engine pushes them apart by modifying their collider positions. Your visual needs to follow this new position, so you sync from collider to visual in Draw().

#### **Trigger vs Physical Colliders**

Set `IsTrigger = true` for UI elements like buttons - they detect collisions but don't physically push things. Set `IsTrigger = false` for gameplay objects that should bounce off each other.

#### **Collision Detection**

Once connected, you can check collisions using the collider:
- `IsCollidingWithGameObject()` for physical collisions
- `IsTriggerCollidingWithGameObject()` for trigger events
- `GetCollidingGameObjects()` to get all colliding objects

#### **Checking Collisions by Name**

To check if your collider is touching a specific GameObject, use the name-based collision methods:

**For Physical Collisions:**
```csharp
if (PhysicsManager.Instance.IsCollidingWithGameObject(myCollider, "Enemy"))
{
    // Handle collision with object named "Enemy"
}
```

**For Trigger Collisions:**
```csharp
if (PhysicsManager.Instance.IsTriggerCollidingWithGameObject(myCollider, "Checkpoint"))
{
    // Handle trigger event with object named "Checkpoint"
}
```

**How It Works:**
The physics system finds all objects colliding with your collider, then checks if any of those objects have the specified name. This only works because you set `collider.Owner = this` during initialization - that links the physics shape to the GameObject's name.

**Common Use Cases:**
- Player touching "Enemy" objects - take damage
- Player reaching "Checkpoint" objects - save progress
- Bullet hitting "Target" objects - destroy target
- Character entering "Portal" objects - teleport

**Performance Note:**
Name-based collision checks are efficient for small numbers of specific objects. For checking many objects of the same type, consider using `GetCollidingGameObjects()` and filtering the results.

#### **Key Requirements**

- Always set `Owner = this` during initialization
- Update collider position when GameObject moves
- Sync visual from collider after physics processes
- Use triggers for non-physical interactions

### Using Buttons

Buttons are interactive UI elements that respond to mouse interactions. They use trigger colliders to detect mouse hover and clicks.

#### **Creating Buttons**

**Basic Creation:**
```csharp
Button startButton = new Button(new Vector2f(200, 60), "START", "StartButton");
startButton.Position = new Vector2f(960, 450);
```

**Customizing Appearance:**
```csharp
startButton.NormalColor = new Color(0, 100, 200);   // Default color
startButton.HoverColor = new Color(0, 150, 255);   // Mouse hover color
startButton.ClickColor = new Color(0, 50, 150);    // Mouse pressed color
```

#### **Button Events**

Buttons use C# events to notify when interactions occur:

**Subscribe to Events:**
```csharp
startButton.OnClick += OnStartClicked;
startButton.OnHover += OnButtonHovered;
startButton.OnHoverExit += OnButtonHoverLeft;
```

**Handle Events:**
```csharp
private void OnStartClicked()
{
    GameStateManager.Instance.SetState("game");
}

private void OnButtonHovered()
{
    // Play hover sound or show tooltip
}

private void OnButtonHoverLeft()
{
    // Hide tooltip
}
```

You may also copy this logic of C# events to your own custom game objects for different purposes.

#### **How Buttons Work**

**Mouse Detection:** Buttons use an AABB trigger collider to detect when the mouse (named "MouseTrigger") is hovering over them.

**Visual Feedback:** Buttons automatically change color based on mouse state:
- Normal color when mouse is away
- Hover color when mouse is over button
- Click color when mouse is pressed

**Event Timing:**
- `OnClick` fires when mouse button is pressed while hovering
- `OnHover` fires when mouse first enters button area
- `OnHoverExit` fires when mouse leaves button area

#### **Button Requirements**

**Mouse Trigger:** For buttons to work, you need a MouseTrigger GameObject in your scene:
```csharp
MouseTrigger mouseTrigger = new MouseTrigger(5f);
```

**Button Properties:**
- Set colors to match your UI theme
- Position buttons relative to screen center for easy layout
- Use unique names for each button to avoid conflicts

**Event Cleanup:** When shutting down a state with buttons, unsubscribe from events to prevent memory leaks:
```csharp
public void Shutdown()
{
    // Unsubscribe from button events
    startButton.OnClick -= OnStartClicked;
    startButton.OnHover -= OnButtonHovered;
    startButton.OnHoverExit -= OnButtonHoverLeft;
    
    quitButton.OnClick -= OnQuitClicked;
    
    // Clear managers and dispose objects
    PhysicsManager.Instance.ClearShapes();
    GameObjectManager.Instance.Clear();
    background?.Dispose();
}
```

#### **Common Button Patterns**

**Menu Navigation:**
```csharp
// Use named methods for proper unsubscription
Button playButton = new Button(size, "PLAY", "PlayButton");
playButton.OnClick += OnPlayClicked;

Button quitButton = new Button(size, "QUIT", "QuitButton");
quitButton.OnClick += OnQuitClicked;

// In Shutdown(), unsubscribe:
playButton.OnClick -= OnPlayClicked;
quitButton.OnClick -= OnQuitClicked;

// Event handler methods:
private void OnPlayClicked()
{
    GameStateManager.Instance.SetState("game");
}

private void OnQuitClicked()
{
    SystemManager.Instance.CloseGame();
}
```

**UI Actions:**
```csharp
// Use named methods for proper unsubscription
Button settingsButton = new Button(size, "SETTINGS", "SettingsButton");
settingsButton.OnClick += OnSettingsClicked;

// In Shutdown(), unsubscribe:
settingsButton.OnClick -= OnSettingsClicked;

// Event handler method:
private void OnSettingsClicked()
{
    ShowSettingsPanel();
}
```

**Important:** Never use anonymous functions (lambda expressions) for button events if you need to unsubscribe later. Always use named methods so you can properly unsubscribe using `-=`.

### Handling Input
- Use `GetKeyDown()` for single-frame events (jumping, shooting)
- Use `GetKeyPressed()` for continuous actions (movement)
- Use `GetKeyUp()` for release events
- Access mouse position with `GetMousePosition()`

### Game Loop Flow
1. Process window events
2. Handle input through current game state
3. Update game objects with delta time
4. Process physics collisions
5. Draw everything to the screen
6. Update input manager (clears frame-specific input states)
7. Display the frame

## Development Tips
- All managers use singleton pattern - access via `Instance` property
- Game objects are automatically managed through `GameObjectManager`
- Use `IsActive` and `IsVisible` properties to control object behavior
- Delta time is calculated each frame for smooth animations
