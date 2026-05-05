using SFML.Graphics;
using SFML.System;

public class TextObject : GameObject
{
    private Font? font;
    private Text? textVisual;

    public TextObject(string name = "TextObject") : base(name)
    {
        Initialize();
    }

    public override void Initialize()
    {
        base.Initialize();
        font = AssetManager.Instance.GetFont("arial");
        textVisual = new Text(font, "Enter Text");
        textVisual?.FillColor = Color.White;
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        // Text objects typically don't have dynamic behavior, but you could add animations or effects here
    }

    public override void Draw(RenderWindow window)
    {
        base.Draw(window);
        FloatRect bounds = textVisual!.GetLocalBounds();
        textVisual.Origin = new Vector2f(bounds.Width / 2, bounds.Height / 2);
        textVisual.Position = _position;
        textVisual.Rotation = _rotation;
        window.Draw(textVisual);
    }

    public override void Destroy()
    {
        base.Destroy();
        textVisual?.Dispose();
    }

    public void SetFont(Font newFont)
    {
        font = newFont;
        if (textVisual != null)
        {
            textVisual.Font = font;
        }
    }

    public void SetText(string newText)
    {
        if (textVisual != null)
        {
            textVisual.DisplayedString = newText;
        }
    }

    public void SetColor(Color color)
    {
        if (textVisual != null)
        {
            textVisual.FillColor = color;
        }
    }

    public void SetCharacterSize(uint size)
    {
        if (textVisual != null)
        {
            textVisual.CharacterSize = size;
        }
    }
}