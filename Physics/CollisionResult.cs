using SFML.System;
using System.Collections.Generic;
using System;

namespace Physics
{
    public class CollisionResult
    {
        public bool HasCollision { get; set; }
        public Vector2f Normal { get; set; }
        public float PenetrationDepth { get; set; }
        public Vector2f ContactPoint { get; set; }

        public CollisionResult()
        {
            HasCollision = false;
            Normal = new Vector2f(0, 0);
            PenetrationDepth = 0f;
            ContactPoint = new Vector2f(0, 0);
        }

        public CollisionResult(bool hasCollision, Vector2f normal, float penetrationDepth, Vector2f contactPoint)
        {
            HasCollision = hasCollision;
            Normal = normal;
            PenetrationDepth = penetrationDepth;
            ContactPoint = contactPoint;
        }
    }

    // Static class for collision detection methods
    public static class CollisionDetection
    {
        // AABB vs AABB
        public static CollisionResult CheckAABBvsAABB(AABB a, AABB b)
        {
            // Get the minimum and maximum corners of both AABBs
            Vector2f aMin = a.Min;
            Vector2f aMax = a.Max;
            Vector2f bMin = b.Min;
            Vector2f bMax = b.Max;

            // Check overlap on X axis
            // If the result is <= 0, the boxes are not overlapping on X
            float overlapX = Math.Min(aMax.X, bMax.X) - Math.Max(aMin.X, bMin.X);
            if (overlapX <= 0)
                return new CollisionResult();

            // Check overlap on Y axis
            // If the result is <= 0, the boxes are not overlapping on Y
            float overlapY = Math.Min(aMax.Y, bMax.Y) - Math.Max(aMin.Y, bMin.Y);
            if (overlapY <= 0)
                return new CollisionResult();

            // Calculate normal and penetration depth
            Vector2f normal;
            float penetrationDepth;

            // Determine the axis of least penetration (smallest overlap)
            // This is used to resolve the collision in the minimal direction
            if (overlapX < overlapY)
            {
                // Collision is resolved along X axis
                penetrationDepth = overlapX;
                
                // Determine direction of the normal
                // If A is left of B, normal points left (-1, 0), else right (1, 0)
                normal = a.Position.X < b.Position.X ? new Vector2f(-1, 0) : new Vector2f(1, 0);
            }
            else
            {
                // Collision is resolved along Y axis
                penetrationDepth = overlapY;

                // Determine direction of the normal
                // If A is below B, normal points down (0, -1), else up (0, 1)
                normal = a.Position.Y < b.Position.Y ? new Vector2f(0, -1) : new Vector2f(0, 1);
            }

            // Calculate an approximate contact point
            // This is the center of the overlapping region
            Vector2f contactPoint = new Vector2f(
                Math.Max(aMin.X, bMin.X) + overlapX * 0.5f,
                Math.Max(aMin.Y, bMin.Y) + overlapY * 0.5f
            );

            return new CollisionResult(true, normal, penetrationDepth, contactPoint);
        }

        // Circle vs Circle
        public static CollisionResult CheckCirclevsCircle(Circle a, Circle b)
        {
            Vector2f diff = b.Position - a.Position;
            float distanceSquared = diff.X * diff.X + diff.Y * diff.Y;
            float radiusSum = a.Radius + b.Radius;
            float radiusSumSquared = radiusSum * radiusSum;

            if (distanceSquared >= radiusSumSquared)
                return new CollisionResult();

            float distance = (float)Math.Sqrt(distanceSquared);
            float penetrationDepth = radiusSum - distance;

            Vector2f normal;
            if (distance > 0)
            {
                normal = diff / distance;
            }
            else
            {
                normal = new Vector2f(1, 0);
            }
            Vector2f contactPoint = a.Position + normal * a.Radius;

            return new CollisionResult(true, normal, penetrationDepth, contactPoint);
        }

        // AABB vs Circle
        public static CollisionResult CheckAABBvsCircle(AABB aabb, Circle circle)
        {
            Vector2f aabbMin = aabb.Min;
            Vector2f aabbMax = aabb.Max;

            // Find closest point on AABB to circle center
            Vector2f closestPoint = new Vector2f(
                Math.Max(aabbMin.X, Math.Min(circle.Position.X, aabbMax.X)),
                Math.Max(aabbMin.Y, Math.Min(circle.Position.Y, aabbMax.Y))
            );

            // Check if closest point is inside circle
            Vector2f diff = circle.Position - closestPoint;
            float distanceSquared = diff.X * diff.X + diff.Y * diff.Y;

            if (distanceSquared > circle.Radius * circle.Radius)
                return new CollisionResult();

            float distance = (float)Math.Sqrt(distanceSquared);
            float penetrationDepth = circle.Radius - distance;

            Vector2f normal;
            if (distance > 0)
            {
                normal = diff / distance;
            }
            else
            {
                normal = new Vector2f(1, 0);
            }

            return new CollisionResult(true, normal, penetrationDepth, closestPoint);
        }

        // Circle vs AABB (reverse of AABB vs Circle)
        public static CollisionResult CheckCirclevsAABB(Circle circle, AABB aabb)
        {
            CollisionResult result = CheckAABBvsCircle(aabb, circle);
            if (result.HasCollision)
            {
                result.Normal = -result.Normal; // Reverse normal
            }
            return result;
        }

        // OBB vs OBB using SAT (Separating Axis Theorem)
        public static CollisionResult CheckOBBvsOBB(OBB a, OBB b)
        {
            Vector2f[] axesA = a.GetAxes();
            Vector2f[] axesB = b.GetAxes();
            Vector2f[] axes = new Vector2f[4];
            axes[0] = axesA[0];
            axes[1] = axesA[1];
            axes[2] = axesB[0];
            axes[3] = axesB[1];
            //axesA.CopyTo(axes, 0);
            //axesB.CopyTo(axes, 2);

            float minOverlap = float.MaxValue;
            Vector2f minAxis = new Vector2f(0, 0);

            // Test all axes for separation
            foreach (Vector2f axis in axes)
            {
                // Project both OBBs onto the current axis
                Vector2f projectionA = ProjectOBB(a, axis);
                Vector2f projectionB = ProjectOBB(b, axis);

                // Compute overlap between projections
                float overlap = Math.Min(projectionA.Y, projectionB.Y) - Math.Max(projectionA.X, projectionB.X);
                
                // If there is no overlap, a separating axis exists → no collision
                if (overlap <= 0)
                    return new CollisionResult();

                // Keep track of the smallest overlap
                if (overlap < minOverlap)
                {
                    minOverlap = overlap;
                    minAxis = axis;
                }
            }

            // Ensure normal points from A to B
            Vector2f direction = b.Position - a.Position;
            if (direction.X * minAxis.X + direction.Y * minAxis.Y < 0)
            {
                // Flip axis if it's pointing the wrong way
                minAxis = -minAxis;
            }

            // Estimate contact point (approximate midpoint along collision normal)
            Vector2f contactPoint = a.Position + minAxis * (minOverlap * 0.5f);

            return new CollisionResult(true, minAxis, minOverlap, contactPoint);
        }

        // OBB vs Circle
        public static CollisionResult CheckOBBvsCircle(OBB obb, Circle circle)
        {
            // Transform circle to OBB's local space
            Vector2f diff = circle.Position - obb.Position;
            float cos = (float)Math.Cos(-obb.Rotation);
            float sin = (float)Math.Sin(-obb.Rotation);
            
            Vector2f localCirclePos = new Vector2f(
                diff.X * cos - diff.Y * sin,
                diff.X * sin + diff.Y * cos
            );

            Vector2f halfSize = obb.Size / 2f;

            // Find closest point on OBB to circle center in local space
            Vector2f closestPoint = new Vector2f(
                Math.Max(-halfSize.X, Math.Min(localCirclePos.X, halfSize.X)),
                Math.Max(-halfSize.Y, Math.Min(localCirclePos.Y, halfSize.Y))
            );

            // Check if closest point is inside circle
            diff = localCirclePos - closestPoint;
            float distanceSquared = diff.X * diff.X + diff.Y * diff.Y;

            if (distanceSquared > circle.Radius * circle.Radius)
                return new CollisionResult();

            float distance = (float)Math.Sqrt(distanceSquared);
            float penetrationDepth = circle.Radius - distance;

            // Transform normal back to world space
            Vector2f localNormal;
            if (distance > 0)
            {
                localNormal = diff / distance;
            }
            else
            {
                localNormal = new Vector2f(1, 0);
            }
            Vector2f worldNormal = new Vector2f(
                localNormal.X * cos + localNormal.Y * sin,
                -localNormal.X * sin + localNormal.Y * cos
            );

            // Transform contact point back to world space
            cos = (float)Math.Cos(obb.Rotation);
            sin = (float)Math.Sin(obb.Rotation);
            Vector2f worldContactPoint = new Vector2f(
                obb.Position.X + closestPoint.X * cos - closestPoint.Y * sin,
                obb.Position.Y + closestPoint.X * sin + closestPoint.Y * cos
            );

            return new CollisionResult(true, worldNormal, penetrationDepth, worldContactPoint);
        }

        // Circle vs OBB (reverse of OBB vs Circle)
        public static CollisionResult CheckCirclevsOBB(Circle circle, OBB obb)
        {
            CollisionResult result = CheckOBBvsCircle(obb, circle);
            if (result.HasCollision)
            {
                result.Normal = -result.Normal; // Reverse normal
            }
            return result;
        }

        // AABB vs OBB
        public static CollisionResult CheckAABBvsOBB(AABB aabb, OBB obb)
        {
            // Treat AABB as OBB with 0 rotation
            OBB aabbAsOBB = new OBB(aabb.Position, aabb.Size, 0);
            return CheckOBBvsOBB(aabbAsOBB, obb);
        }

        // OBB vs AABB (reverse of AABB vs OBB)
        public static CollisionResult CheckOBBvsAABB(OBB obb, AABB aabb)
        {
            CollisionResult result = CheckAABBvsOBB(aabb, obb);
            if (result.HasCollision)
            {
                result.Normal = -result.Normal; // Reverse normal
            }
            return result;
        }

        // Helper method for SAT projection
        public static Vector2f ProjectOBB(OBB obb, Vector2f axis)
        {
            Vector2f[] corners = obb.GetCorners();
            float min = corners[0].X * axis.X + corners[0].Y * axis.Y;
            float max = min;

            for (int i = 1; i < corners.Length; i++)
            {
                float projection = corners[i].X * axis.X + corners[i].Y * axis.Y;
                if (projection < min)
                    min = projection;
                if (projection > max)
                    max = projection;
            }

            return new Vector2f(min, max);
        }
    }
    
}