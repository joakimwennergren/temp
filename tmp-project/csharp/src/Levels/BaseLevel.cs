namespace Svampjakt;

public abstract class BaseLevel
{
    public enum DifficultyLevel
    {
        Easy,
        Medium,
        Hard
    };

    public string Name { get; set; }
    public string Description { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public MushroomSpawner MushroomSpawner { get; set; } = new MushroomSpawner();

    public abstract void Update(float deltaTime, int screenWidth, int screenHeight);
}