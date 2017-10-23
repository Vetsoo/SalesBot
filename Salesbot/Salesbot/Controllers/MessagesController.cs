using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using AdaptiveCards;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Salesbot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                Activity reply = null;
                Attachment attachment = null;
                AdaptiveCard adaptiveCard = null;
                if (activity.Text.ToLower().Contains("hey"))
                {
                    reply = activity.CreateReply($"Hey Jente, how can I help you?");
                    await connector.Conversations.ReplyToActivityAsync(reply);
                }
                else if (activity.Text.ToLower().Contains(".net"))
                {

                    reply = activity.CreateReply($"The following consultants are interested : Rival, Kelvin and Kevin");
                    reply.Attachments = new List<Attachment>();

                    adaptiveCard = new AdaptiveCard();
                    adaptiveCard.Body.Add(new TextBlock()
                    {
                        Text = "Kevin",
                        Size = TextSize.Large,
                        Weight = TextWeight.Bolder,
                    });
                    adaptiveCard.Body.Add(new TextBlock()
                    {
                        Text = "kevin.boets@axxes.com",
                    });
                    attachment = new Attachment()
                    {
                        ContentType = AdaptiveCard.ContentType,
                        Content = adaptiveCard,

                    };

                    AdaptiveCard adaptiveCard2 = new AdaptiveCard();
                    adaptiveCard2.Body.Add(new TextBlock()
                    {
                        Text = "Rival",
                        Size = TextSize.Large,
                        Weight = TextWeight.Bolder,
                    });
                    adaptiveCard2.Body.Add(new TextBlock()
                    {
                        Text = "rival@axxes.com",
                    });
                    Attachment attachment2 = new Attachment()
                    {
                        ContentType = AdaptiveCard.ContentType,
                        Content = adaptiveCard2,

                    };

                    AdaptiveCard adaptiveCard3 = new AdaptiveCard();
                    adaptiveCard3.Body.Add(new TextBlock()
                    {
                        Text = "Kelvin",
                        Size = TextSize.Large,
                        Weight = TextWeight.Bolder,
                    });
                    adaptiveCard3.Body.Add(new TextBlock()
                    {
                        Text = "kelvin@axxes.com",
                    });
                    Attachment attachment3 = new Attachment()
                    {
                        ContentType = AdaptiveCard.ContentType,
                        Content = adaptiveCard3,

                    };
                    reply.Attachments.Add(attachment);
                    reply.Attachments.Add(attachment2);
                    reply.Attachments.Add(attachment3);
                    await connector.Conversations.ReplyToActivityAsync(reply);
                }
                else if (activity.Text.ToLower().Contains("radius"))
                {
                    reply = activity.CreateReply($"The following consultants live within 30km radius : ");
                    adaptiveCard = new AdaptiveCard();
                    adaptiveCard.Body.Add(new TextBlock()
                    {
                        Text = "Kevin",
                        Size = TextSize.Large,
                        Weight = TextWeight.Bolder,
                    });
                    adaptiveCard.Body.Add(new TextBlock()
                    {
                        Text = "kevin.boets@axxes.com",
                    });
                    attachment = new Attachment()
                    {
                        ContentType = AdaptiveCard.ContentType,
                        Content = adaptiveCard,

                    };
                    reply.Attachments.Add(attachment);
                    await connector.Conversations.ReplyToActivityAsync(reply);

                }
                else
                {
                    reply = activity.CreateReply($"I don't understand what you mean.");
                    await connector.Conversations.ReplyToActivityAsync(reply);

                }
                //await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}