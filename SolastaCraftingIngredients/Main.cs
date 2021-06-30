using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityModManagerNet;
using SolastaModApi;
using ModKit;
using ModKit.Utility;
using SolastaModApi.Extensions;
using System.Collections.Generic;

namespace SolastaCraftingIngredients
{
    public static class Main
    {
        public static readonly string MOD_FOLDER = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static Guid ModGuidNamespace = new Guid("80a5106e-5cb7-4fdd-8f96-b94f3aafd4dd");

        [Conditional("DEBUG")]
        internal static void Log(string msg) => Logger.Log(msg);
        internal static void Error(Exception ex) => Logger?.Error(ex.ToString());
        internal static void Error(string msg) => Logger?.Error(msg);
        internal static void Warning(string msg) => Logger?.Warning(msg);
        internal static UnityModManager.ModEntry.ModLogger Logger { get; private set; }
        internal static ModManager<Core, Settings> Mod { get; private set; }
        internal static MenuManager Menu { get; private set; }
        internal static Settings Settings { get { return Mod.Settings; } }

        internal static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {
                Logger = modEntry.Logger;

                Mod = new ModManager<Core, Settings>();
                Menu = new MenuManager();
                modEntry.OnToggle = OnToggle;

                Translations.Load(MOD_FOLDER);
            }
            catch (Exception ex)
            {
                Error(ex);
                throw;
            }

            return true;
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool enabled)
        {
            if (enabled)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Mod.Enable(modEntry, assembly);
                Menu.Enable(modEntry, assembly);
            }
            else
            {
                Menu.Disable(modEntry);
                Mod.Disable(modEntry, false);
                ReflectionCache.Clear();
            }
            return true;
        }

        internal static void OnGameReady()
        {
            Dictionary<ItemDefinition, ItemDefinition> EnchantedToIngredient = new Dictionary<ItemDefinition, ItemDefinition>()
            {
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_MithralStone, DatabaseHelper.ItemDefinitions._300_GP_Opal },
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Crystal_Of_Winter, DatabaseHelper.ItemDefinitions._100_GP_Pearl },
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Blood_Gem, DatabaseHelper.ItemDefinitions._500_GP_Ruby },
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Soul_Gem, DatabaseHelper.ItemDefinitions._1000_GP_Diamond},
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Slavestone, DatabaseHelper.ItemDefinitions._100_GP_Emerald },
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Cloud_Diamond, DatabaseHelper.ItemDefinitions._1000_GP_Diamond },
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Stardust, DatabaseHelper.ItemDefinitions._100_GP_Pearl },
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Doom_Gem, DatabaseHelper.ItemDefinitions._50_GP_Sapphire },
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Shard_Of_Fire, DatabaseHelper.ItemDefinitions._500_GP_Ruby },
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Shard_Of_Ice, DatabaseHelper.ItemDefinitions._50_GP_Sapphire },
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_LifeStone, DatabaseHelper.ItemDefinitions._1000_GP_Diamond },
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Diamond_Of_Elai, DatabaseHelper.ItemDefinitions._100_GP_Emerald },
                {DatabaseHelper.ItemDefinitions.Ingredient_PrimordialLavaStones, DatabaseHelper.ItemDefinitions._20_GP_Amethyst },
                //{DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Blood_Of_Solasta, DatabaseHelper.ItemDefinitions._20_GP_Amethyst },
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Medusa_Coral, DatabaseHelper.ItemDefinitions._300_GP_Opal },
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_PurpleAmber, DatabaseHelper.ItemDefinitions._50_GP_Sapphire },
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Heartstone, DatabaseHelper.ItemDefinitions._300_GP_Opal },
            };
            List<RecipeDefinition> recipes = new List<RecipeDefinition>();
            foreach (ItemDefinition item in EnchantedToIngredient.Keys)
            {

                string recipeName = "RecipeEnchanting" + item.Name;
                RecipeBuilder builder = new RecipeBuilder(recipeName, GuidHelper.Create(Main.ModGuidNamespace, recipeName).ToString());
                builder.AddIngredient(EnchantedToIngredient[item]);
                builder.SetCraftedItem(item);
                builder.SetCraftingCheckData(36, 16, DatabaseHelper.ToolTypeDefinitions.EnchantingToolType);
                recipes.Add(builder.AddToDB());
            }

            foreach (RecipeDefinition recipe in recipes)
            {
                ItemDefinition craftintgManual = ItemBuilder.BuilderCopyFromItemSetRecipe(recipe, DatabaseHelper.ItemDefinitions.CraftingManualRemedy,
                    "CraftingManual_" + recipe.Name, DatabaseHelper.ItemDefinitions.CraftingManualRemedy.GuiPresentation, 100);
                StockItem(DatabaseHelper.MerchantDefinitions.Store_Merchant_Antiquarians_Halman_Summer, craftintgManual);
                StockItem(DatabaseHelper.MerchantDefinitions.Store_Merchant_Gorim_Ironsoot_Cyflen_GeneralStore, craftintgManual);
            }
        }

        private static void StockItem(MerchantDefinition merchant, ItemDefinition item)
        {
            StockUnitDescription stockUnit = new StockUnitDescription();
            stockUnit.SetItemDefinition(item);
            stockUnit.SetInitialAmount(1);
            stockUnit.SetInitialized(true);
            stockUnit.SetMaxAmount(2);
            stockUnit.SetMinAmount(1);
            stockUnit.SetStackCount(1);
            stockUnit.SetReassortAmount(1);
            merchant.StockUnitDescriptions.Add(stockUnit);
        }
    }
}
