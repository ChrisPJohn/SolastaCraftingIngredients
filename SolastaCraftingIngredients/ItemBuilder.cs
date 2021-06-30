
using SolastaModApi;
using System.Collections.Generic;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;

namespace SolastaCraftingIngredients
{
    class ItemBuilder
    {

        private class ItemDefinitionBuilder : BaseDefinitionBuilder<ItemDefinition>
        {
            public ItemDefinitionBuilder(ItemDefinition original, string name, string guid) : base(original, name, guid)
            {
            }

            public void SetDocumentInformation(RecipeDefinition recipeDefinition, List<ContentFragmentDescription> contentFragments)
            {
                if (Definition.DocumentDescription == null)
                {
                    Definition.SetDocumentDescription(new DocumentDescription());
                }
                Definition.IsDocument = true;
                Definition.DocumentDescription.SetRecipeDefinition(recipeDefinition);
                Definition.DocumentDescription.SetLoreType(RuleDefinitions.LoreType.CraftingRecipe);
                Definition.DocumentDescription.SetDestroyAfterReading(true);
                Definition.DocumentDescription.SetLocationKnowledgeLevel(GameCampaignDefinitions.NodeKnowledge.Known);
                Definition.DocumentDescription.SetField("contentFragments", contentFragments);
            }

            public void SetGuiPresentation(GuiPresentation guiPresentation)
            {
                Definition.SetGuiPresentation(guiPresentation);
            }

            public void SetGold(int gold)
            {
                Definition.SetCosts(new int[] { 0, gold, 0, 0, 0 });
            }

        }

        public static ItemDefinition BuilderCopyFromItemSetRecipe(RecipeDefinition recipeDefinition, ItemDefinition toCopy, string name, GuiPresentation guiPresentation, int gold)
        {
            ItemDefinitionBuilder builder = new ItemDefinitionBuilder(toCopy, name, GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
            builder.SetDocumentInformation(recipeDefinition, toCopy.DocumentDescription.ContentFragments);
            builder.SetGuiPresentation(guiPresentation);
            builder.SetGold(gold);
            return builder.AddToDB();
        }
    }
}
