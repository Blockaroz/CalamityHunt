using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityHunt.Common.Utilities;

public class Rope
{
    public List<RopeSegment> segments;
    public int accuracy;
    public float segmentLength;
    public Vector2 gravity;
    public bool twoPoint;
    public float damping;
    public bool tileCollide;
    public Vector2 StartPos { get; set; }
    public Vector2 EndPos { get; set; }

    public Rope(Vector2 startPoint, int segmentCount, float segmentLength, Vector2 gravity, float damping = 0f, int accuracy = 15, bool tileCollide = false)
    {
        segments = new List<RopeSegment>();
        for (var i = 0; i < segmentCount; i++)
            segments.Add(new RopeSegment(startPoint + gravity.SafeNormalize(Vector2.Zero) * i));
        StartPos = startPoint;
        EndPos = startPoint;

        this.segmentLength = segmentLength;
        this.gravity = gravity;
        this.damping = damping;
        this.accuracy = Math.Max(1, accuracy);
        twoPoint = false;
        this.tileCollide = tileCollide;
    }

    public Rope(Vector2 startPoint, Vector2 endPoint, int segmentCount, float segmentLength, Vector2 gravity, float damping = 0f, int accuracy = 15, bool tileCollide = false)
    {
        segments = new List<RopeSegment>();
        for (var i = 0; i < segmentCount; i++)
            segments.Add(new RopeSegment(Vector2.Lerp(startPoint, endPoint, (float)i / (segmentCount - 1))));
        StartPos = startPoint;
        EndPos = endPoint;

        this.segmentLength = segmentLength;
        this.gravity = gravity;
        this.damping = damping;
        this.accuracy = Math.Max(1, accuracy);
        twoPoint = true;
        this.tileCollide = tileCollide;
    }

    public List<Vector2> GetPoints()
    {
        List<Vector2> points = new List<Vector2>();
        foreach (var segment in segments)
            points.Add(segment.position);
        return points;
    }

    public void Update()
    {
        segments[0].position = StartPos;

        if (twoPoint)
            segments[segments.Count - 1].position = EndPos;

        for (var i = 0; i < segments.Count; i++) {
            if (segments[i].position.HasNaNs())
                segments[i].position = segments[0].position;

            var velocity = (segments[i].position - segments[i].oldPosition) / (1f + damping) + gravity;
            velocity = TileCollision(segments[i].position, velocity);

            segments[i].oldPosition = segments[i].position;
            segments[i].position += velocity;
        }

        for (var i = 0; i < accuracy; i++)
            ConstrainPoints();
    }

    private void ConstrainPoints()
    {
        for (var i = 0; i < segments.Count - 1; i++) {
            var dist = (segments[i].position - segments[i + 1].position).Length();
            var error = MathF.Abs(dist - segmentLength);
            var changeDirection = Vector2.Zero;

            if (dist > segmentLength)
                changeDirection = (segments[i].position - segments[i + 1].position).SafeNormalize(Vector2.Zero);

            else if (dist < segmentLength)
                changeDirection = (segments[i + 1].position - segments[i].position).SafeNormalize(Vector2.Zero);

            var changeAmount = changeDirection * error;
            if (i != 0) {
                segments[i].position += TileCollision(segments[i].position, changeAmount * -0.5f);
                segments[i + 1].position += TileCollision(segments[i + 1].position, changeAmount * 0.5f);
            }
            else
                segments[i + 1].position += TileCollision(segments[i + 1].position, changeAmount);
        }

        if (!twoPoint)
            EndPos = segments[segments.Count - 1].position;
    }

    private Vector2 TileCollision(Vector2 position, Vector2 velocity)
    {
        if (!tileCollide)
            return velocity;

        var newVelocity = Collision.noSlopeCollision(position - new Vector2(3), velocity, 6, 6, true, true);
        var final = velocity;

        if (Math.Abs(newVelocity.X) < Math.Abs(velocity.X))
            final.X = 0;
        if (Math.Abs(newVelocity.Y) < Math.Abs(velocity.Y))
            final.Y = 0;

        return final;
    }

    public class RopeSegment
    {
        public Vector2 position;
        public Vector2 oldPosition;

        public RopeSegment(Vector2 pos)
        {
            position = pos;
            oldPosition = pos;
        }
    }
}
