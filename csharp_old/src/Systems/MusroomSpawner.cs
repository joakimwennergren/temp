using System;
using System.Collections.Generic;
using Entropy.ECS;
using Entropy.ECS.Components;

namespace Svampjakt;
 
public class MushroomSpawner
{
    // Spawning properties
    public bool ShouldSpawn = true;
    public int SpawnedMushrooms = 0;
    private float spawnInterval = 1.0f;
    private float ZOffset = 0.001f;
    private Random Random = new Random();

    private float time = 0.0f;

    private List<Position> Positions = new List<Position>();
    private List<Mushroom> ActiveMushrooms = new List<Mushroom>();

    public List<Mushroom> mushrooms = new List<Mushroom>()
    {
        new Mushroom("assets/sprites/ametistskivling.png", new Dimension(0.5f, 1.8f)),
        new Mushroom("assets/sprites/arg_gron_kragskivling.png", new Dimension(0.8f, 1.6f)),
        new Mushroom("assets/sprites/blek_kantarell.png", new Dimension(1.0f, 2.0f)),
        new Mushroom("assets/sprites/blodhatta.png", new Dimension(0.5f, 1.5f)),
        new Mushroom("assets/sprites/blodsopp.png", new Dimension(1.5f, 3.0f)),
        new Mushroom("assets/sprites/blodvax_skivling.png", new Dimension(1.0f, 2.0f)),
        new Mushroom("assets/sprites/bombmurkla.png", new Dimension(1.5f, 2.5f)),
        new Mushroom("assets/sprites/citrongul_slemskivling.png", new Dimension(1.0f, 2.5f)),
        new Mushroom("assets/sprites/eldsopp.png", new Dimension(1.0f, 2.5f)),
        new Mushroom("assets/sprites/fjallig_blacksvamp.png", new Dimension(0.6f, 3.5f)),
        new Mushroom("assets/sprites/fjallsopp.png", new Dimension(1.0f, 3.0f)),
        new Mushroom("assets/sprites/flugsvamp.png", new Dimension(2.0f, 5.0f)),
        new Mushroom("assets/sprites/gifthatting.png", new Dimension(1.0f, 2.4f)),
        new Mushroom("assets/sprites/goliatmusseron.png", new Dimension(1.0f, 4.0f)),
        new Mushroom("assets/sprites/grafotad_flugsvamp.png", new Dimension(2.0f, 5.0f)),
        new Mushroom("assets/sprites/gronkremla.png", new Dimension(1.4f, 2.4f)),
        new Mushroom("assets/sprites/hostmusseron.png", new Dimension(1.2f, 2.2f)),
        new Mushroom("assets/sprites/kantarell.png", new Dimension(1.2f, 2.2f)),
        new Mushroom("assets/sprites/kungschampion.png", new Dimension(2.5f, 3.5f)),
        new Mushroom("assets/sprites/laxskivling.png", new Dimension(1.2f, 2.5f)),
        new Mushroom("assets/sprites/lomsk_flugsvamp.png", new Dimension(2.0f, 6.0f)),
        new Mushroom("assets/sprites/lovviolspindling.png", new Dimension(2.0f, 5.0f)),
        new Mushroom("assets/sprites/narrkantarell.png", new Dimension(1.0f, 2.0f)),
        new Mushroom("assets/sprites/prickmusseron.png", new Dimension(2.0f, 4.0f)),
        new Mushroom("assets/sprites/rattigmusseron.png", new Dimension(1.0f, 2.0f)),
        new Mushroom("assets/sprites/rimskivling.png", new Dimension(2.0f, 5.0f)),
        new Mushroom("assets/sprites/rodbrun_stensopp.png", new Dimension(2.5f, 5.0f)),
        new Mushroom("assets/sprites/sammetsfotad_pluggskivling.png", new Dimension(1.5f, 3.0f)),
        new Mushroom("assets/sprites/sammetssopp.png", new Dimension(1.5f, 3.0f)),
        new Mushroom("assets/sprites/scharlakans_vaxskivling.png", new Dimension(1.5f, 3.0f)),
        new Mushroom("assets/sprites/slidsilkesskivling.png", new Dimension(1.5f, 3.0f)),
        new Mushroom("assets/sprites/smorsopp.png", new Dimension(1.5f, 3.5f)),
        new Mushroom("assets/sprites/sprod_vaxskivling.png", new Dimension(1.3f, 3.0f)),
        new Mushroom("assets/sprites/stensopp.png", new Dimension(1.3f, 3.0f)),
        new Mushroom("assets/sprites/stjarnrodhatting.png", new Dimension(0.8f, 2.0f)),
        new Mushroom("assets/sprites/stolt_fjallskivling.png", new Dimension(3f, 8.0f)),
        new Mushroom("assets/sprites/svavelmusseron.png", new Dimension(2.0f, 5.0f)),
        new Mushroom("assets/sprites/tallsopp.png", new Dimension(1.0f, 4.0f)),
        new Mushroom("assets/sprites/tallsopp.png", new Dimension(1.0f, 4.0f)),
        new Mushroom("assets/sprites/tegelsopp.png", new Dimension(2.0f, 4.0f)),
        new Mushroom("assets/sprites/toppig_giftspindelskivling.png", new Dimension(1.5f, 3.0f)),
        new Mushroom("assets/sprites/toppmurkla.png", new Dimension(1.5f, 5f)),
        new Mushroom("assets/sprites/varmusseron.png", new Dimension(1.3f, 3.0f)),
        new Mushroom("assets/sprites/vit_flugsvamp.png", new Dimension(2.0f, 6.0f)),
    };
        
    public MushroomSpawner(List<Position> positions)
    {
        Positions = positions;
    }

    public Mushroom GetRandomMushroom()
    {
        int index = Random.Next(mushrooms.Count);
        return mushrooms[index];
    }

    public void Update(float deltaTime, int screenWidth, int screenHeight)
    {
        if(ShouldSpawn)
        {
            Position randomPosition = Positions[Random.Next(Positions.Count)];
            int index = Random.Next(mushrooms.Count);
            Mushroom randomMushroom = mushrooms[index];
            randomPosition.Z += ZOffset;
            ZOffset += 0.1f;
            randomMushroom.BasePosition = randomPosition;

            ActiveMushrooms.Add(randomMushroom);

            if(SpawnedMushrooms < 5)
            {
                SpawnedMushrooms++;
            } else {
                ShouldSpawn = false;
            }
        }
        foreach (var am in ActiveMushrooms)
        {
            am.Update(deltaTime, screenWidth, screenHeight);
        }
    }
}