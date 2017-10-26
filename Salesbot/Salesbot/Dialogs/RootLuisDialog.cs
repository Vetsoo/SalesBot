using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace Salesbot.Dialogs
{
    [LuisModel("7805cd93-3419-42ab-8948-e58870860015", "6458dacf5b4f403295125bd2dcf4330d", domain: "westeurope.api.cognitive.microsoft.com", staging: true, verbose: true)]
    [Serializable]
    public class RootLuisDialog : LuisDialog<object>
    {
        private const string EntityConsultant = "Consultant";
        private const string EntityKeywords = "Keywords";
        private const string EntityOpportunities = "Opportunity";

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand '{result.Query}'. Type 'help' if you need assistance.";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Find profiles for opportunity")]
        private async Task FindProfilesForOpportunity(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;

            EntityRecommendation keywordsEntity;

            if (result.TryFindEntity(EntityKeywords, out keywordsEntity))
            {
                
            }

            await context.PostAsync($"Hey {message.From.Name}! Ik ben op zoek naar geschikte kandidaten voor de volgende keywords: ...");

            //EntityRecommendation cityEntityRecommendation;

            //if (result.TryFindEntity(EntityGeographyCity, out cityEntityRecommendation))
            //{
            //    cityEntityRecommendation.Type = "Destination";
            //}

            //var hotelsFormDialog = new FormDialog<HotelsQuery>(hotelsQuery, this.BuildHotelsForm, FormOptions.PromptInStart, result.Entities);

            //context.Call(hotelsFormDialog, this.ResumeAfterHotelsFormDialog);
        }
    }
}