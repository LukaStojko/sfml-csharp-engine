using SFML.System;
using SFML.Graphics;

namespace Physics
{
    public abstract class Shape
    {
        public Vector2f Position { get; set; }
        public bool IsStatic { get; set; } = false;
        public bool IsTrigger { get; set; } = false;
        public GameObject? Owner { get; set; }
        
        protected Shape()
        {
            PhysicsManager.Instance.AddShape(this);
        }
        protected Shape(Vector2f position)
        {
            PhysicsManager.Instance.AddShape(this);
            Position = position;
        }

        public abstract bool ContainsPoint(Vector2f point);
        public abstract void DebugDraw(RenderWindow window, Color color);
    }

    public class AABB : Shape
    {
        public Vector2f Size { get; set; }
        public Vector2f Min => Position - Size / 2f;
        public Vector2f Max => Position + Size / 2f;

        public AABB(Vector2f size) : base()
        {
            Size = size;
        }
        public AABB(Vector2f position, Vector2f size) : base(position)
        {
            Size = size;
        }

        public override bool ContainsPoint(Vector2f point)
        {
            return point.X >= Min.X && point.X <= Max.X &&
                   point.Y >= Min.Y && point.Y <= Max.Y;
        }

        public override void DebugDraw(RenderWindow window, Color color)
        {
            RectangleShape rect = new RectangleShape(Size);
            rect.Position = Min;
            rect.FillColor = Color.Transparent;
            rect.OutlineColor = color;
            rect.OutlineThickness = 2.0f;
            window.Draw(rect);
            rect.Dispose();
        }
    }

    public class Circle : Shape
    {
        public float Radius { get; set; }

        public Circle(float radius) : base()
        {
            Radius = radius;
        }
        public Circle(Vector2f position, float radius) : base(position)
        {
            Radius = radius;
        }

        public override bool ContainsPoint(Vector2f point)
        {
            Vector2f diff = point - Position;
            return (diff.X * diff.X + diff.Y * diff.Y) <= (Radius * Radius);
        }

        public override void DebugDraw(RenderWindow window, Color color)
        {
            CircleShape circle = new CircleShape(Radius);
            circle.Position = new Vector2f(Position.X - Radius, Position.Y - Radius);
            circle.FillColor = Color.Transparent;
            circle.OutlineColor = color;
            circle.OutlineThickness = 2.0f;
            window.Draw(circle);
            circle.Dispose();
        }
    }

    public class OBB : Shape
    {
        public Vector2f Size { get; set; }
        public float Rotation { get; set; } // In radians

        public OBB(Vector2f size, float rotation = 0) : base()
        {
            Size = size;
            Rotation = rotation;
        }
        public OBB(Vector2f position, Vector2f size, float rotation = 0) : base(position)
        {
            Size = size;
            Rotation = rotation;
        }

        public Vector2f[] GetCorners()
        {
            // Half size is used because the box is centered at Position
            Vector2f halfSize = Size / 2f;

            // Cosine and sine of rotation
            float cos = (float)Math.Cos(Rotation);
            float sin = (float)Math.Sin(Rotation);

            Vector2f[] corners = new Vector2f[4];
            
            // Local space corners
            Vector2f[] localCorners = {
                new Vector2f(-halfSize.X, -halfSize.Y), // bottom-left
                new Vector2f( halfSize.X, -halfSize.Y), // bottom-right
                new Vector2f( halfSize.X,  halfSize.Y), // top-right
                new Vector2f(-halfSize.X,  halfSize.Y)  // top-left
            };

            // Transform to world space
            for (int i = 0; i < 4; i++)
            {
                corners[i] = new Vector2f(
                    // Apply rotation (2D rotation matrix) and then translate by Position
                    Position.X + localCorners[i].X * cos - localCorners[i].Y * sin,
                    Position.Y + localCorners[i].X * sin + localCorners[i].Y * cos
                );
            }

            return corners;
        }

        public Vector2f[] GetAxes()
        {
            // Get world-space corners
            Vector2f[] corners = GetCorners();
            
            // Only 2 axes are needed for a rectangle (the others are parallel)
            Vector2f[] axes = new Vector2f[2];

            // Get the two perpendicular axes from the edges
            Vector2f edge1 = corners[1] - corners[0]; // bottom edge
            Vector2f edge2 = corners[3] - corners[0]; // left edge

            // Normalize and get perpendicular vectors
            float edge1Length = (float)Math.Sqrt(edge1.X * edge1.X + edge1.Y * edge1.Y);
            float edge2Length = (float)Math.Sqrt(edge2.X * edge2.X + edge2.Y * edge2.Y);

            // Get perpendicular (normal) vectors and normalize them
            // Perpendicular of (x, y) is (-y, x)
            axes[0] = new Vector2f(-edge1.Y / edge1Length, edge1.X / edge1Length);
            axes[1] = new Vector2f(-edge2.Y / edge2Length, edge2.X / edge2Length);

            return axes;
        }

        public override bool ContainsPoint(Vector2f point)
        {
            // Move point into box-centered coordinate system
            Vector2f diff = point - Position;

            // "Undo" the box rotation (rotate point in opposite direction)
            float cos = (float)Math.Cos(-Rotation);
            float sin = (float)Math.Sin(-Rotation);
            
            Vector2f localPoint = new Vector2f(
                diff.X * cos - diff.Y * sin,
                diff.X * sin + diff.Y * cos
            );

            // Check if point lies within axis-aligned bounds
            Vector2f halfSize = Size / 2f;
            return Math.Abs(localPoint.X) <= halfSize.X &&
                   Math.Abs(localPoint.Y) <= halfSize.Y;
        }

        public override void DebugDraw(RenderWindow window, Color color)
        {
            Vector2f[] corners = GetCorners();
            
            // Create lines to draw the rectangle outline
            for (int i = 0; i < 4; i++)
            {
                int nextIndex = (i + 1) % 4;
                Vertex[] line = new Vertex[]
                {
                    new Vertex(corners[i], color),
                    new Vertex(corners[nextIndex], color)
                };
                window.Draw(line, PrimitiveType.Lines);
            }
        }
    }
}
