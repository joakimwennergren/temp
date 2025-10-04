using System;
using Entropy.ECS;
using Entropy.ECS.Components;

namespace Svampjakt;
 
public class Mushroom
{
    public Entity Entity { get; set; } 
    public Texture Texture { get; set; } 
    public Position Position { get; set; } = new Position(0.0f, 0.0f, 0.0f);
    public Dimension Dimension { get; set; } = new Dimension(0.0f, 0.0f);
    public Dimension Ratio { get; set; } = new Dimension(0.5f, 0.5f);
    public bool ShouldGrow { get; set; } = false;

    private float Time = 0.0f;
    private float BounceValue = 0.0f;
    private float Offset = 0.0f;
    private bool FirstGrow = true;
    private int GrowStep = 0;
    private Dimension BaseDimension = new Dimension(1.4f, 1.4f);

    public void Create()
    {
        Dimension = new Dimension(
            BaseDimension.x * Ratio.x,
            BaseDimension.y * Ratio.y
        );

        Entity mushroom = new Entity();
        mushroom.AddComponent(Position);
        mushroom.AddComponent(Dimension);
        mushroom.AddComponent(new Color(1.0f, 1.0f, 1.0f, 1.0f));
        mushroom.AddComponent(new Rotation(0.0f, 1.0f, 0.0f, 0.0f));
        mushroom.AddComponent(Texture);
        mushroom.AddComponent(new Type2D(12));
        Entity = mushroom;
    }

    public void GrowOneStep() 
    {
        if(GrowStep > 1) return;
        Time = 0.0f;
        if(!FirstGrow){
            Offset += 1.0f;
        }
        ShouldGrow = true;
    }

    public void Update(float deltaTime, int screenWidth, int screenHeight, Position pos)
    {        
        if(ShouldGrow) {
            if(Time < 1.0f) {
                Time += deltaTime * 1.0f;
                BounceValue = Easing.EaseBounceOut(Time) + Offset;
                //Console.WriteLine($"BounceValue: {BounceValue}");
            } else {
                ShouldGrow = false;
                FirstGrow = false;
                GrowStep++;
            }
        }

        // Percent -> pixel scale
        float sx = screenWidth  / 100.0f;
        float sy = screenHeight / 100.0f;

        // Visual size in pixels
        float baseWidth  = Dimension.x;
        float baseHeight = Dimension.y;

        float visualWidth  = (baseWidth  + BounceValue * 0.5f) * sx;
        float visualHeight = (baseHeight + BounceValue) * sy;

        var dimension = new Dimension(visualWidth, visualHeight);

        // Anchor in pixels = desired bottom-middle point
        float anchorX = pos.x * sx;
        float anchorY = pos.y * sy;

        // If your engine treats Position as the CENTER of the sprite:
        var position = new Position(
            anchorX,                      // keep X centered on the anchor
            anchorY + visualHeight, // move center up by half height
            pos.z
        );
        Entity.Update(dimension);
        Entity.Update(position);
    }
}
