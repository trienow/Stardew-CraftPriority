using StardewModdingAPI;
using StardewValley;

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
            const string occupied = "▓";
            const string free = "░";

            if (e.Button.IsActionButton())
            {
                Farmer player = Game1.player;
                StardewValley.Object activeObj = player.ActiveObject;
                if (activeObj != null)
                {
                    int actualEdibility = activeObj.Edibility;
                    if (actualEdibility == -300)
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
                        for (int y = -1; y <= 1 && !preventEating; y++)
                        {
                            string occupants = "";
                            for (int x = -1; x <= 1 && !preventEating; x++)
                            {
                                if (x == 0 && y == 0)
                                {
                                    occupants += "P";
                                }
                                else
                                {
                                    StardewValley.Object tile = Game1.currentLocation.getObjectAtTile(px + x, py + y);

                                    if (tile != null)
                                    {
                                        StardewValley.Object tileCopy = new StardewValley.Object(tile.TileLocation, tile.ParentSheetIndex);
                                        tileCopy.Name = tile.Name;
                                        //tileCopy.heldObject.Set(tile.heldObject.Value);

                                        preventEating = tileCopy.performObjectDropInAction(activeObj, true, player);
                                        occupants += preventEating ? occupied : free;
                                    }
                                    else
                                    {
                                        occupants += free;
                                    }
                                }
                            }
                            Monitor.Log(occupants, LogLevel.Warn);
                        }
                        Monitor.Log("", LogLevel.Alert);

                        if (preventEating)
                        {
                            //The object can be used in the surrounding tiles.
                            //BAN IT!
                            activeObj.Edibility = -300;
                        }
                        else
                        {
                            //Unban the object and make it edible again
                            activeObj.Edibility = actualEdibility;
                        }
                    }
                }
            }
        }
    }
}
