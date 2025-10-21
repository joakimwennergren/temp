using System;
using Entropy.ECS;
using Entropy.ECS.Components;

namespace Svampjakt;
 
public class Mushroom
{   
    // Entity & components
    public Entity Entity { get; set; } 

    // Mushroom properties
    private Dimension BaseDimension { get; set; } = new Dimension(5.0f, 5.0f);
    public Position BasePosition {get; set;}

    // Growth animation
    private float Time = 0.0f;
    private float BounceValue = 0.0f;

    private bool hasBeenClicked = false;

    public Mushroom(string path, Dimension ratio)
    {
        Entity mushroom = new Entity();
        mushroom.Set(new Position(0.0f, 0.0f, 0.0f));
        mushroom.Set(new Dimension(
            BaseDimension.Width * ratio.Width,
            BaseDimension.Height * ratio.Height
        ));
        mushroom.Set(new Color(1.0f, 1.0f, 1.0f, 1.0f));
        mushroom.Set(new Rotation(0.0f, 1.0f, 0.0f, 0.0f));
        mushroom.Set(new Texture(path));
        Entity = mushroom;
    }

    public static bool OnHover(
        float centerX, float centerY,
        float width,  float height,   
        float mouseX, float mouseY,
        bool yDown = true,
        float scaleX = 1f, float scaleY = 1f)
    {
        float halfW = 0.5f * width  * scaleX;
        float halfH = 0.5f * height * scaleY;

        float left   = centerX - halfW;
        float right  = centerX + halfW;

        // Handle y-down vs y-up screen spaces
        float top    = yDown ? centerY - halfH : centerY + halfH;
        float bottom = yDown ? centerY + halfH : centerY - halfH;

        return mouseX >= left && mouseX <= right &&
            mouseY >= top  && mouseY <= bottom;
    }

    private void UpdateMushroom(float screenWidth, float screenHeight)
    {
        Dimension dimension = new Dimension();
        Position position = new Position();

        // Percent -> pixel scale
        float sx = screenWidth  / 100.0f;
        float sy = screenHeight / 100.0f;
        float visualHeight = 0.0f;

        Entity.Mutate<Dimension>((ref Dimension d) =>
        {
            float visualWidth  = (BaseDimension.Width * 0.5f) * sx;
            visualHeight = (BaseDimension.Height) * sy;
            d.Width = visualWidth;
            d.Height = visualHeight;
            dimension = d;
        });

        position.X = BasePosition.X * sx;
        position.Y = BasePosition.Y * sy + visualHeight;
        position.Z = BasePosition.Z;
        Entity.Set(position);
        
        if (OnHover(
            position.X,
            position.Y,
            dimension.Width,
            dimension.Height,
            Pointer.X,
            Pointer.Y
        )) 
        {
            Entity.Dispose();
            hasBeenClicked = true;
        } else 
        {
            if (!Pointer.IsDown && hasBeenClicked) {
                Console.WriteLine("Mushroom clicked!");
                Entity.Dispose();
                hasBeenClicked = false;
            }
        }
    }

    public void Update(float deltaTime, int screenWidth, int screenHeight)
    {   
        /*
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
        */
        UpdateMushroom(screenWidth, screenHeight);
    }
}
