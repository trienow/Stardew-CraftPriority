﻿using StardewModdingAPI;
using StardewValley;
using System;

namespace CraftPriority
{
    class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            Helper.Events.Input.ButtonPressed += this.Input_ButtonPressed;
        }

        private void Input_ButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (e.Button.IsActionButton())
            {
                Farmer player = Game1.player;
                StardewValley.Object activeObj = player.ActiveObject;
                if (activeObj != null)
                {
                    int actualEdibility = activeObj.Edibility;
                    if (actualEdibility == -300 && !activeObj.isPlaceable()) //isPlacable prevents machines turning into food (e.g. Chest -> Tuna)
                    {
                        //Create a dummy object and check if it normally has a different edibility
                        StardewValley.Object objReference = new StardewValley.Object(activeObj.ParentSheetIndex, activeObj.Stack, activeObj.IsRecipe, activeObj.Price, activeObj.Quality);
                        actualEdibility = objReference.Edibility;
                    }

                    //If it is actually edible, check if it can be used in the player's surroundings
                    if (actualEdibility != -300)
                    {
                        int px = player.getTileX();
                        int py = player.getTileY();

                        bool preventEating = false;
                        //Test for "action" blocks around the player
                        for (int y = -1; y <= 1 && !preventEating; y++)
                        {
                            for (int x = -1; x <= 1 && !preventEating; x++)
                            {
                                if (x != 0 && y != 0)
                                {
                                    StardewValley.Object tile = Game1.currentLocation.getObjectAtTile(px + x, py + y);
                                    if (tile != null)
                                    {
                                        //Make a copy to prevent modification of the real object
                                        StardewValley.Object tileCopy = new StardewValley.Object(tile.TileLocation, tile.ParentSheetIndex);
                                        tileCopy.Name = tile.Name;

                                        // For a picky setting?
                                        //tileCopy.heldObject.Set(tile.heldObject.Value);

                                        //If a action can be performed, prevent the player from eating the object
                                        preventEating = tileCopy.performObjectDropInAction(activeObj, true, player);
                                    }
                                }
                            }
                        }

                        //If the object can be used in the surrounding tiles: BAN IT!
                        // otherwise unban the object and make it edible again
                        activeObj.Edibility = preventEating ? -300 : actualEdibility;
                    }
                }
            }
        }
    }
}
