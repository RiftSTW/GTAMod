using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using GTA;
using GTA.Native;
using GTA.Math;
using NativeUI;

namespace GTAMod
{
    class Class1 : Script
    {

        MenuPool modMenuPool;
        UIMenu mainMenu;

        UIMenu playerMenu;
        UIMenu weaponsMenu;
        UIMenu vehicleMenu;
        UIMenu creditsMenu;

        UIMenuItem resetWantedLevel;
        UIMenuItem CreditsShow;

        public Class1()
        {
            Setup();

            Tick += onTick;
            KeyDown += onKeyDown;
        }

        void Setup()
        {
            modMenuPool = new MenuPool();
            mainMenu = new UIMenu("Mod Menu", "Select an option");
            modMenuPool.Add(mainMenu);

            playerMenu = modMenuPool.AddSubMenu(mainMenu, "Player");
            weaponsMenu = modMenuPool.AddSubMenu(mainMenu, "Weapons");
            vehicleMenu = modMenuPool.AddSubMenu(mainMenu, "Vehicles");

            SetupPlayerFunctions();
            SetupWeaponsFunctions();
            SetupVehicleFunctions();
        }
        void SetupVehicleFunctions()
        {
            VehicleSelectorMenu();
            VehicleSpawnByName();
        }

        void SetupPlayerFunctions()
        {
            ResetWantedLevel();
            GodMode();
        }

        void SetupWeaponsFunctions()
        {
            weaponSelectorMenu();
            GetAllWeapons();
            RemoveAllWeapons();
        }

        void VehicleSpawnByName()
        {
            UIMenuItem vehicleSpawnItem = new UIMenuItem("Spawn vehicle by name");
            vehicleMenu.AddItem(vehicleSpawnItem);

            Ped gamePed = Game.Player.Character;

            vehicleMenu.OnItemSelect += (sender, item, index) =>
            {
                if (item == vehicleSpawnItem)
                {
                    string modelName = Game.GetUserInput(50);
                    Model model = new Model(modelName);
                    model.Request();

                    if (model.IsInCdImage && model.IsValid)
                    {
                        Vehicle v = World.CreateVehicle(model, gamePed.Position, gamePed.Heading);
                        v.PlaceOnGround();
                        gamePed.Task.WarpIntoVehicle(v, VehicleSeat.Driver);
                    }
                }
            };
        }

        void GetAllWeapons()
        {
            UIMenuItem allWeapons = new UIMenuItem("Get all weapons");
            weaponsMenu.AddItem(allWeapons);

            weaponsMenu.OnItemSelect += (sender, item, index) => {
                if (item == allWeapons)
                {
                    WeaponHash[] allWeaponHashes = (WeaponHash[])Enum.GetValues(typeof(WeaponHash));
                    for (int i = 0; i < allWeaponHashes.Length; i++)
                    {
                        Game.Player.Character.Weapons.Give(allWeaponHashes[i], 9999, true, true);
                    }
                }
            };
        }


        bool godModeOn = false;
        void GodMode()
        {
            UIMenuItem godMode = new UIMenuItem("God Mode: " + godModeOn.ToString());

            playerMenu.AddItem(godMode);

            playerMenu.OnItemSelect += (sender, item, index) =>
            {
                if (item == godMode)
                {
                    godModeOn = !godModeOn;

                    if (godModeOn)
                    {
                        Game.Player.IsInvincible = true;
                        Game.Player.Character.IsInvincible = true;

                        godMode.Text = "God Mode: " + true.ToString();
                    }
                    else
                    {
                        Game.Player.IsInvincible = false;
                        Game.Player.Character.IsInvincible = false;

                        godMode.Text = "God Mode " + false.ToString();
                    }
                }
            };
        }

        bool neverWantedLevelOn = false;
        void NeverWantedLevel()
        {
            UIMenuItem neverWanted = new UIMenuItem("Never Wanted Level " + neverWantedLevelOn.ToString());

            playerMenu.AddItem(neverWanted);
            playerMenu.OnItemSelect += (sender, item, index) =>
            {
                if (item == neverWanted)
                {
                    neverWantedLevelOn = !neverWantedLevelOn;

                    if (neverWantedLevelOn)
                        neverWanted.Text = "Never Wanted Level: " + true.ToString();
                    else
                        neverWanted.Text = "Never Wanted Level " + false.ToString();
                }
            };
        }

        void RemoveAllWeapons()
        {
            UIMenuItem removeButton = new UIMenuItem("Remove All Weapons");

            WeaponHash[] allWeaponHashes = (WeaponHash[])Enum.GetValues(typeof(WeaponHash));
            weaponsMenu.AddItem(removeButton);
            weaponsMenu.OnItemSelect += (sender, item, index) =>
            {
                if (item == removeButton)
                {
                    foreach (WeaponHash hash in allWeaponHashes)
                    {
                        if (Game.Player.Character.Weapons.HasWeapon(hash))
                            Game.Player.Character.Weapons.Remove(hash);
                    }
                }
            };
        }

        void ResetWantedLevel()
        {
            resetWantedLevel = new UIMenuItem("Reset Wanted Level");
            playerMenu.AddItem(resetWantedLevel);

            playerMenu.OnItemSelect += (sender, item, index) =>
            {
                if (item == resetWantedLevel)
                {
                    if (Game.Player.WantedLevel == 0)
                    {
                        UI.ShowSubtitle("Get a wanted level first!");
                    }
                    else
                    {
                        Game.Player.WantedLevel = 0;
                    }
                }
            };
        }

        void weaponSelectorMenu()
        {
            UIMenu submenu = modMenuPool.AddSubMenu(mainMenu, "Weapon Selector Menu");

            List<dynamic> listOfWeapons = new List<dynamic>();
            WeaponHash[] allWeaponHashes = (WeaponHash[])Enum.GetValues(typeof(WeaponHash));
            for (int i = 0; i < allWeaponHashes.Length; i++)
            {
                listOfWeapons.Add(allWeaponHashes[i]);
            }

            UIMenuListItem list = new UIMenuListItem("Weapons: ", listOfWeapons, 0);
            submenu.AddItem(list);

            UIMenuItem getWeapon = new UIMenuItem("Get weapon: ");
            submenu.AddItem(getWeapon);

            submenu.OnItemSelect += (sender, item, index) =>
            {
                if (item == getWeapon)
                {
                    int listIndex = list.Index;
                    WeaponHash currentHash = allWeaponHashes[listIndex];
                    Game.Player.Character.Weapons.Give(currentHash, 9999, true, true);
                }
            };
        }

        void VehicleSelectorMenu()
        {
            UIMenu subMenu = modMenuPool.AddSubMenu(weaponsMenu, "Vehicle Selector");

            List<dynamic> listOfVehicles = new List<dynamic>();
            VehicleHash[] allVehicleHashes = (VehicleHash[])Enum.GetValues(typeof(VehicleHash));
            for (int i = 0; i < allVehicleHashes.Length; i++)
            {
                listOfVehicles.Add(allVehicleHashes[i]);
            }

            UIMenuListItem list = new UIMenuListItem("Vehicle: ", listOfVehicles, 0);
            subMenu.AddItem(list);

            UIMenuItem getVehicle = new UIMenuItem("Get Vehicle");
            subMenu.AddItem(getVehicle);

            subMenu.OnItemSelect += (sender, item, index) =>
            {
                if (item == getVehicle)
                {
                    int listIndex = list.Index;
                    VehicleHash hash = allVehicleHashes[listIndex];

                    Ped gamePed = Game.Player.Character;

                    Vehicle v = World.CreateVehicle(hash, gamePed.Position, gamePed.Heading);
                    v.PlaceOnGround();
                    gamePed.Task.WarpIntoVehicle(v, VehicleSeat.Driver);
                }
            };
        }

        void onTick(object sender, EventArgs e)
        {
            if (modMenuPool != null)
                modMenuPool.ProcessMenus();

            if(neverWantedLevelOn)
            {
                Game.Player.WantedLevel = 0;
            }
        }

        void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F10 && !modMenuPool.IsAnyMenuOpen())
            {
                mainMenu.Visible = !mainMenu.Visible;
            }
        }
    }
}