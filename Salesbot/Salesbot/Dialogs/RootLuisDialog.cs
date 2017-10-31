using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System.Collections.Generic;

namespace Salesbot.Dialogs
{
    [LuisModel("#modelId", "#subscriptionKey", domain: "#domain", staging: true, verbose: true)]
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
            string message = $"Sorry, dat heb ik niet helemaal begrepen. Typ 'help' om te zien wat ik kan doen.";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            string message = $"Ik kan effecient op zoek gaan naar profielen voor bepaalde vacatures.Probeer me vragen te stellen zoals: 'Ik ben op zoek naar consultants met ervaring in .net mvc, azure, web development' of Ik heb een vacature met de volgende keywords: java, spring, scrum, agile'.";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var input = await activity;
            
            string message = $"Hey {input.From.Name}! Wat kan ik voor je doen?";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Find profiles for opportunity")]
        private async Task FindProfilesForOpportunity(IDialogContext context, LuisResult result)
        {
            var keywords = new List<string>();

            if (result.Entities != null)
            {
                foreach (var entity in result.Entities)
                {
                    if (entity.Type == EntityKeywords)
                        keywords.Add(entity.Entity);
                }
            }

            var message = $"Ik heb de volgende keywords gevonden: {string.Join(", ", keywords)}";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

    }
}