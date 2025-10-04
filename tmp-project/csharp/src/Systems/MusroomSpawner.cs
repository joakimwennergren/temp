using System;
using System.Collections.Generic;
using Entropy.ECS;
using Entropy.ECS.Components;

namespace Svampjakt;
 
public class MushroomSpawner
{
    public bool ShouldSpawn = true;
    public int SpawnedMushrooms = 0;
    private Random Random = new Random();
    private float time = 0.0f;

    public List<Mushroom> items = new List<Mushroom>()
    {
        new Mushroom{ Texture = new Texture("assets/sprites/ametistskivling.png"), 
                      Ratio = new Dimension(0.5f, 1.8f)},

        new Mushroom{ Texture = new Texture("assets/sprites/arg_gron_kragskivling.png"), 
                      Ratio = new Dimension(0.8f, 1.6f)},

        new Mushroom{ Texture = new Texture("assets/sprites/blek_kantarell.png"), 
                      Ratio = new Dimension(1.0f, 2.0f)},

        new Mushroom{ Texture = new Texture("assets/sprites/blodhatta.png"), 
                      Ratio = new Dimension(0.5f, 1.5f)},

        new Mushroom{ Texture = new Texture("assets/sprites/blodsopp.png"), 
                      Ratio = new Dimension(1.5f, 3.0f)},

        new Mushroom{ Texture = new Texture("assets/sprites/blodvax_skivling.png"), 
                      Ratio = new Dimension(1.0f, 2.0f)},

        new Mushroom{ Texture = new Texture("assets/sprites/bombmurkla.png"), 
                      Ratio = new Dimension(1.5f, 2.5f)},

        new Mushroom{ Texture = new Texture("assets/sprites/citrongul_slemskivling.png"), 
                      Ratio = new Dimension(1.0f, 2.5f)},

        new Mushroom{ Texture = new Texture("assets/sprites/eldsopp.png"), 
                      Ratio = new Dimension(1.0f, 2.5f)},

        new Mushroom{ Texture = new Texture("assets/sprites/fjallig_blacksvamp.png"), 
                      Ratio = new Dimension(0.6f, 3.5f)},

        new Mushroom{ Texture = new Texture("assets/sprites/fjallsopp.png"), 
                      Ratio = new Dimension(1.0f, 3.0f)},

        new Mushroom{ Texture = new Texture("assets/sprites/flugsvamp.png"), 
                      Ratio = new Dimension(2.0f, 5.0f)},

        new Mushroom{ Texture = new Texture("assets/sprites/gifthatting.png"), 
                      Ratio = new Dimension(1.0f, 2.4f)},

        new Mushroom{ Texture = new Texture("assets/sprites/goliatmusseron.png"), 
                      Ratio = new Dimension(1.0f, 4.0f)},

        new Mushroom{ Texture = new Texture("assets/sprites/grafotad_flugsvamp.png"), 
                      Ratio = new Dimension(2.0f, 5.0f)},

        new Mushroom{ Texture = new Texture("assets/sprites/gronkremla.png"), 
                      Ratio = new Dimension(1.4f, 2.4f)},

        new Mushroom{ Texture = new Texture("assets/sprites/hostmusseron.png"), 
                      Ratio = new Dimension(1.2f, 2.2f)},

        new Mushroom{ Texture = new Texture("assets/sprites/kantarell.png"), 
                      Ratio = new Dimension(1.2f, 2.2f)},

        new Mushroom{ Texture = new Texture("assets/sprites/kungschampion.png"), 
                      Ratio = new Dimension(2.5f, 3.5f)},

        new Mushroom{ Texture = new Texture("assets/sprites/laxskivling.png"), 
                      Ratio = new Dimension(1.2f, 2.5f)},

        new Mushroom{ Texture = new Texture("assets/sprites/lomsk_flugsvamp.png"), 
                      Ratio = new Dimension(2.0f, 6.0f)},

        new Mushroom{ Texture = new Texture("assets/sprites/lovviolspindling.png"), 
                      Ratio = new Dimension(2.0f, 5.0f)},

        new Mushroom{ Texture = new Texture("assets/sprites/narrkantarell.png"), 
                      Ratio = new Dimension(1.0f, 2.0f)},

        new Mushroom{ Texture = new Texture("assets/sprites/prickmusseron.png"), 
                      Ratio = new Dimension(2.0f, 4.0f)},

        new Mushroom{ Texture = new Texture("assets/sprites/rattigmusseron.png"), 
                      Ratio = new Dimension(1.0f, 2.0f)},

        new Mushroom{ Texture = new Texture("assets/sprites/rimskivling.png"), 
                      Ratio = new Dimension(2.0f, 5.0f)},

        new Mushroom{ Texture = new Texture("assets/sprites/rodbrun_stensopp.png"), 
                      Ratio = new Dimension(2.5f, 5.0f)},

        new Mushroom{ Texture = new Texture("assets/sprites/sammetsfotad_pluggskivling.png"), 
                      Ratio = new Dimension(1.5f, 3.0f)},

        new Mushroom{ Texture = new Texture("assets/sprites/sammetssopp.png"), 
                      Ratio = new Dimension(1.5f, 3.0f)},

        new Mushroom{ Texture = new Texture("assets/sprites/scharlakans_vaxskivling.png"), 
                      Ratio = new Dimension(1.5f, 3.0f)},

        new Mushroom{ Texture = new Texture("assets/sprites/slidsilkesskivling.png"), 
                      Ratio = new Dimension(1.5f, 3.0f)},

        new Mushroom{ Texture = new Texture("assets/sprites/smorsopp.png"), 
                      Ratio = new Dimension(1.5f, 3.5f)},

        new Mushroom{ Texture = new Texture("assets/sprites/sprod_vaxskivling.png"), 
                      Ratio = new Dimension(1.3f, 3.0f)},

        new Mushroom{ Texture = new Texture("assets/sprites/stensopp.png"), 
                      Ratio = new Dimension(1.3f, 3.0f)},

        new Mushroom{ Texture = new Texture("assets/sprites/stjarnrodhatting.png"), 
                      Ratio = new Dimension(0.8f, 2.0f)},

        new Mushroom{ Texture = new Texture("assets/sprites/stolt_fjallskivling.png"), 
                      Ratio = new Dimension(3f, 8.0f)},

        new Mushroom{ Texture = new Texture("assets/sprites/svavelmusseron.png"), 
                      Ratio = new Dimension(2.0f, 5.0f)},

        new Mushroom{ Texture = new Texture("assets/sprites/tallsopp.png"), 
                      Ratio = new Dimension(1.0f, 4.0f)},

        new Mushroom{ Texture = new Texture("assets/sprites/tegelsopp.png"), 
                      Ratio = new Dimension(2.0f, 4.0f)},

        new Mushroom{ Texture = new Texture("assets/sprites/toppig_giftspindelskivling.png"), 
                      Ratio = new Dimension(1.5f, 3.0f)},

        new Mushroom{ Texture = new Texture("assets/sprites/toppmurkla.png"), 
                      Ratio = new Dimension(1.5f, 5f)},

        new Mushroom{ Texture = new Texture("assets/sprites/varmusseron.png"), 
                      Ratio = new Dimension(1.3f, 3.0f)},

        new Mushroom{ Texture = new Texture("assets/sprites/vit_flugsvamp.png"), 
                      Ratio = new Dimension(2.0f, 6.0f)},
    };
        
    public MushroomSpawner()
    {
        foreach(var item in items)
        {
            item.Create();
        }
    }

    public Mushroom GetRandomMushroom()
    {
        int index = Random.Next(items.Count);
        return items[index];
    }

    public void Update(float deltaTime, int screenWidth, int screenHeight)
    {
        time += deltaTime * 1.0f;
        if(time >= 5.0f) {
            foreach (var item in items)
            {
                item.GrowOneStep();
                time = 0.0f; // Reset the timer after growing
            }
        }
    }
}