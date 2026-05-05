using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using System.Linq;

namespace Physics
{
    // Represents a collision pair between two shapes
    public readonly struct CollisionPair
    {
        public Shape ShapeA { get; }
        public Shape ShapeB { get; }

        public CollisionPair(Shape shapeA, Shape shapeB)
        {
            ShapeA = shapeA;
            ShapeB = shapeB;
        }

        // Check if this pair contains the given shapes (order doesn't matter)
        public bool Contains(Shape shapeA, Shape shapeB)
        {
            return (ShapeA == shapeA && ShapeB == shapeB) || (ShapeA == shapeB && ShapeB == shapeA);
        }
    }

    public class PhysicsManager
    {
        private static PhysicsManager? _instance;
        public static PhysicsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PhysicsManager();
                }
                return _instance;
            }
        }

        private List<Shape> _shapes;
        private List<CollisionPair> _currentCollisions;
        private List<CollisionPair> _previousCollisions;

        private PhysicsManager()
        {
            _shapes = new List<Shape>();
            _currentCollisions = new List<CollisionPair>();
            _previousCollisions = new List<CollisionPair>();
        }

        public void AddShape(Shape shape)
        {
            if (!_shapes.Contains(shape))
            {
                _shapes.Add(shape);
            }
        }

        public void RemoveShape(Shape shape)
        {
            _shapes.Remove(shape);
        }

        public void ClearShapes()
        {
            _shapes.Clear();
        }

        internal void DebugDraw(RenderWindow window)
        {
            foreach (var shape in _shapes)
            {
                shape.DebugDraw(window, Color.Red);
            }
        }

        public void CheckAndResolveCollisions()
        {
            // Swap collision lists
            _previousCollisions.Clear();
            _previousCollisions.AddRange(_currentCollisions);
            _currentCollisions.Clear();

            // Check and resolve collisions between all possible shape pairs
            for (int i = 0; i < _shapes.Count; i++)
            {
                for (int j = i + 1; j < _shapes.Count; j++)
                {
                    Shape shapeA = _shapes[i];
                    Shape shapeB = _shapes[j];

                    // Skip if both are static
                    if (shapeA.IsStatic && shapeB.IsStatic)
                        continue;

                    CollisionResult collision = CheckCollision(shapeA, shapeB);
                    if (collision.HasCollision)
                    {
                        var pair = new CollisionPair(shapeA, shapeB);
                        _currentCollisions.Add(pair);
                        
                        // Handle collision events and resolution
                        HandleCollision(shapeA, shapeB, collision);
                    }
                }
            }
        }

        // Get all shapes that are currently colliding with the given shape
        public List<Shape> GetCollidingShapes(Shape shape)
        {
            List<Shape> collidingShapes = new List<Shape>();
            
            foreach (var pair in _currentCollisions)
            {
                if (pair.ShapeA == shape)
                {
                    collidingShapes.Add(pair.ShapeB);
                }
                else if (pair.ShapeB == shape)
                {
                    collidingShapes.Add(pair.ShapeA);
                }
            }
            
            return collidingShapes;
        }

        // Check if a shape is colliding with any shape owned by a game object with the given name
        public bool IsCollidingWithGameObject(Shape shape, string gameObjectName)
        {
            var collidingShapes = GetCollidingShapes(shape);
            
            foreach (var collidingShape in collidingShapes)
            {
                if (collidingShape.Owner != null && collidingShape.Owner.Name == gameObjectName)
                {
                    return true;
                }
            }
            
            return false;
        }

        // Get all game objects that the given shape is colliding with
        public List<GameObject> GetCollidingGameObjects(Shape shape)
        {
            List<GameObject> collidingGameObjects = new List<GameObject>();
            var collidingShapes = GetCollidingShapes(shape);
            
            foreach (var collidingShape in collidingShapes)
            {
                if (collidingShape.Owner != null && !collidingGameObjects.Contains(collidingShape.Owner))
                {
                    collidingGameObjects.Add(collidingShape.Owner);
                }
            }
            
            return collidingGameObjects;
        }

        // Get all shapes that are currently colliding with the given shape (trigger collisions only)
        public List<Shape> GetTriggerCollidingShapes(Shape shape)
        {
            List<Shape> triggerCollidingShapes = new List<Shape>();
            
            foreach (var pair in _currentCollisions)
            {
                Shape? otherShape = null;
                if (pair.ShapeA == shape)
                {
                    otherShape = pair.ShapeB;
                }
                else if (pair.ShapeB == shape)
                {
                    otherShape = pair.ShapeA;
                }

                if (otherShape != null && (shape.IsTrigger || otherShape.IsTrigger))
                {
                    triggerCollidingShapes.Add(otherShape);
                }
            }
            
            return triggerCollidingShapes;
        }

        // Get all game objects that the given shape is colliding with (trigger collisions only)
        public List<GameObject> GetTriggerCollidingGameObjects(Shape shape)
        {
            List<GameObject> triggerCollidingGameObjects = new List<GameObject>();
            var triggerCollidingShapes = GetTriggerCollidingShapes(shape);
            
            foreach (var collidingShape in triggerCollidingShapes)
            {
                if (collidingShape.Owner != null && !triggerCollidingGameObjects.Contains(collidingShape.Owner))
                {
                    triggerCollidingGameObjects.Add(collidingShape.Owner);
                }
            }
            
            return triggerCollidingGameObjects;
        }

        // Check if a shape is colliding with any shape owned by a game object with the given name (trigger collisions only)
        public bool IsTriggerCollidingWithGameObject(Shape shape, string gameObjectName)
        {
            var triggerCollidingShapes = GetTriggerCollidingShapes(shape);
            
            foreach (var collidingShape in triggerCollidingShapes)
            {
                if (collidingShape.Owner != null && collidingShape.Owner.Name == gameObjectName)
                {
                    return true;
                }
            }
            
            return false;
        }

        private void HandleCollision(Shape shapeA, Shape shapeB, CollisionResult collision)
        {
            // Check if this is a trigger collision
            bool isTriggerCollision = shapeA.IsTrigger || shapeB.IsTrigger;
            
            // If it's a trigger collision, don't resolve physically
            if (!isTriggerCollision)
            {
                // Resolve the physical collision only if neither is a trigger
                ResolveCollision(shapeA, shapeB, collision);
            }
            
            // TODO: Add trigger event callbacks here if needed
            // For example: OnTriggerEnter, OnTriggerStay, etc.
        }

        private void ResolveCollision(Shape shapeA, Shape shapeB, CollisionResult collision)
        {
            if (!collision.HasCollision)
                return;

            // Simple position-based collision response
            SeparateShapes(shapeA, shapeB, collision);
        }
        
        private void SeparateShapes(Shape shapeA, Shape shapeB, CollisionResult collision)
        {
            //Penetration depth in half so each object moves equally
            Vector2f separation = collision.Normal * (collision.PenetrationDepth * 0.5f);

            // Handle static objects
            if (shapeA.IsStatic && !shapeB.IsStatic)
            {
                // Move only the non-static object
                shapeB.Position += collision.Normal * collision.PenetrationDepth;
            }
            else if (!shapeA.IsStatic && shapeB.IsStatic)
            {
                // Move only the non-static object
                shapeA.Position -= collision.Normal * collision.PenetrationDepth;
            }
            else if (!shapeA.IsStatic && !shapeB.IsStatic)
            {
                // Move both objects apart equally
                shapeA.Position -= separation;
                shapeB.Position += separation;
            }
            // If both are static, do nothing
        }

        public CollisionResult CheckCollision(Shape shapeA, Shape shapeB)
        {
            // Use dynamic dispatch based on shape types
            if (shapeA is AABB a1 && shapeB is AABB a2)
                return CollisionDetection.CheckAABBvsAABB(a1, a2);
            
            if (shapeA is Circle c1 && shapeB is Circle c2)
                return CollisionDetection.CheckCirclevsCircle(c1, c2);
            
            if (shapeA is AABB a3 && shapeB is Circle c3)
                return CollisionDetection.CheckAABBvsCircle(a3, c3);
            
            if (shapeA is Circle c4 && shapeB is AABB a4)
                return CollisionDetection.CheckCirclevsAABB(c4, a4);
            
            if (shapeA is OBB o1 && shapeB is OBB o2)
                return CollisionDetection.CheckOBBvsOBB(o1, o2);
            
            if (shapeA is OBB o3 && shapeB is Circle c5)
                return CollisionDetection.CheckOBBvsCircle(o3, c5);
            
            if (shapeA is Circle c6 && shapeB is OBB o4)
                return CollisionDetection.CheckCirclevsOBB(c6, o4);
            
            if (shapeA is AABB a5 && shapeB is OBB o5)
                return CollisionDetection.CheckAABBvsOBB(a5, o5);
            
            if (shapeA is OBB o6 && shapeB is AABB a6)
                return CollisionDetection.CheckOBBvsAABB(o6, a6);

            return new CollisionResult(); // No collision for unknown combinations
        }
    }
}
