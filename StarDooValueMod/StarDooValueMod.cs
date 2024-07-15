using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Force.DeepCloner;
using System.Collections.Generic;
using System.Linq;
using StardewValley.Inventories;
using StardewValley.Quests;
using StardewValley.Pathfinding;
using StardewValley.GameData.Characters;
using StardewValley.TerrainFeatures;

public class StarDooValueMod : Mod
{
    private const string IntroductionFlag = "DataObservabilityOfficerIntroduction";
    private const string MailFlag = "DataObservabilityOfficerInvitation";
    private const string NextQuestFlag = "DataObservabilityOfficerNextQuest";
    private QuestManager questManager;
    private int placedComputers = 0;

    private Texture2D droneTexture;
    private Vector2 dronePosition;
    private Vector2 droneVelocity;
    private Random random;
    private int animationFrame;
    private int animationTimer;
    private int droneNumberOfFrames = 1;

    private Dictionary<string, (string, Vector2)> npcCameraQuest;

    public override void Entry(IModHelper helper)
    {
        questManager = new QuestManager(helper, Monitor);
        npcCameraQuest = new Dictionary<string, (string, Vector2)>
        {
            { "7000", ("ScienceHouse", new Vector2(23, 15)) },
            { "7001", ("LeahHouse", new Vector2(6, 2)) }
        };

        droneVelocity = new Vector2(0, 0);
        random = new Random();
        animationFrame = 0;
        animationTimer = 0;

        helper.Events.GameLoop.DayStarted += OnDayStarted;
        helper.Events.Input.ButtonPressed += questManager.OnButtonPressed;
        helper.Events.Player.Warped += OnWarped;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.Display.RenderedWorld += OnRenderedWorld;
    }

    private void InstallCamera(string locationName, Vector2 cameraPosition)
    {
        var location = Game1.getLocationFromName(locationName);

        if (location != null)
        {
            StardewValley.Object cam = ItemRegistry.Create<StardewValley.Object>("(O)9002", 1);
            location.objects[cameraPosition] = cam;
            Monitor.Log($"Installed camera in {cameraPosition} in {locationName}.", LogLevel.Info);
        }
    }
    
    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        if (Context.IsWorldReady)
        {
            Monitor.Log("Checking if mail should be sent...", LogLevel.Debug);

            if (!Game1.player.mailReceived.Contains(MailFlag))
            {
                Game1.addMailForTomorrow(MailFlag);
                Monitor.Log("Added DataObservabilityOfficerInvitation mail for tomorrow.", LogLevel.Debug);
            }

            if (Game1.player.mailReceived.Contains(IntroductionFlag) && !Game1.player.mailReceived.Contains(NextQuestFlag))
            {
                Game1.addMailForTomorrow(NextQuestFlag);
                Monitor.Log("Added DataObservabilityOfficerNextQuest mail for tomorrow.", LogLevel.Debug);
            }

            string qid = "7000";
            if (!Game1.player.hasQuest(qid))
            {
                Game1.player.addQuest(qid);
                StardewValley.Object cam = ItemRegistry.Create<StardewValley.Object>("(O)9002", 1);
                Game1.player.addItemToInventory(cam);
                cam = ItemRegistry.Create<StardewValley.Object>("(O)9002", 1);
                Game1.player.addItemToInventory(cam);

                StardewValley.Object dooMachine = ItemRegistry.Create<StardewValley.Object>("(BC)9017", 1);
                Game1.player.addItemToInventory(dooMachine);
            }
        }
    }

    private void OnWarped(object? sender, WarpedEventArgs e)
    {
        if (e.NewLocation.Name == "Farm")
        {
            // Start the drone near the player when they enter the farm
            dronePosition = e.Player.Position.DeepClone();
        }
    }

    private void OnUpdateTicked(object? sender, EventArgs e)
    {
       
        if (Context.IsWorldReady)
        {
            foreach (var quest in Game1.player.questLog)
            {
                if (quest.completed.Value && npcCameraQuest.ContainsKey(quest.id.Value))
                {
                    var npcQuest = npcCameraQuest[quest.id.Value];
                    npcCameraQuest.Remove(quest.id.Value);
                    InstallCamera(npcQuest.Item1, npcQuest.Item2);
                }
            }

        }
        if (Context.IsWorldReady && Game1.player?.currentLocation is MineShaft mine)
        {
            PlaceBrokenComputers(mine);
        }
        if (Context.IsWorldReady && Game1.currentLocation.Name == "Farm" && droneTexture != null) 
        {
            // Update animation
            animationTimer++;
            if (animationTimer > 10) // Change frame every 10 ticks
            {
                animationFrame = (animationFrame + 1) % droneNumberOfFrames; 
                animationTimer = 0;
            }

            // Update position
            if (random.NextDouble() < 0.05) // Change direction occasionally
            {
                droneVelocity = new Vector2((float)(random.NextDouble() * 2 - 1), (float)(random.NextDouble() * 2 - 1));
                droneVelocity.Normalize();
                droneVelocity *= 2; // Speed of the drone
            }
            dronePosition += droneVelocity;

            // Ensure the drone stays within the bounds of the farm/forest area
            dronePosition.X = Math.Clamp(dronePosition.X, 0, Game1.currentLocation.Map.DisplayWidth * 64 - droneTexture.Width);
            dronePosition.Y = Math.Clamp(dronePosition.Y, 0, Game1.currentLocation.Map.DisplayHeight * 64 - droneTexture.Height);
        }
    }

    private void PlaceBrokenComputers(MineShaft mine)
    {
        Random random = new Random();
        while (placedComputers < 5) {
            Vector2 position = new Vector2(random.Next(mine.Map.DisplayWidth / 64), random.Next(mine.Map.DisplayHeight / 64));
            if (mine.isTileClearForMineObjects(position) && mine.isTileOnClearAndSolidGround(position))
            {
                StardewValley.Object computer = ItemRegistry.Create<StardewValley.Object>("(O)9001", 1);
                computer.CanBeSetDown = true;
                computer.CanBeGrabbed = false;
                mine.objects[position] = computer;

                int mineLevel = mine.mineLevel;
                Monitor.Log($"BrokenComputer placed at level {mineLevel}, position ({position.X}, {position.Y})", LogLevel.Info);
                placedComputers++;
            } else {
                Monitor.Log($"Cannot place computer on {position}, retrying", LogLevel.Info);
            }
        }
    }

    private void OnRenderedWorld(object? sender, RenderedWorldEventArgs e)
    {
        if (Context.IsWorldReady && Game1.currentLocation.Name == "Farm")
        {
            // Draw the drone
            droneTexture = Helper.GameContent.Load<Texture2D>("Mods/StarDooValueContent/assets/do-eye.png");
            int frameWidth = droneTexture.Width / droneNumberOfFrames; 
            Rectangle sourceRect = new Rectangle(animationFrame * frameWidth, 0, frameWidth, droneTexture.Height);
            Vector2 screenPosition = Game1.GlobalToLocal(Game1.viewport, dronePosition);
            e.SpriteBatch.Draw(droneTexture, screenPosition, sourceRect, Color.White);
        }
    }

}
