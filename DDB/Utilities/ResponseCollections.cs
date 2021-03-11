using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DartsDiscordBots.Utilities
{
    public class ResponseCollections
    {
        public static List<string> _8BallResponses = new List<string>
        {
                "Most definitely yes",
                "For sure",
                "As I see it, yes",
                "My sources say yes",
                "Yes",
                "Most likely",
                "Perhaps",
                "Maybe",
                "Not sure",
                "It is uncertain",
                "Ask me again later",
                "Don't count on it",
                "Probably not",
                "Very doubtful",
                "Most likely no",
                "Nope",
                "No",
                "My sources say no",
                "Dont even think about it",
                "Definitely no",
                "NO - It may cause disease contraction",
                "I mean like yeah, probably?",
                "What are you stupid? Of course not.",
                "You _would_ ask that. No.",
                "Maybe probably.",
                "You know it!",
        };

        public static List<string> UserNotFoundResponses = new List<string>
        {
            "I don't know who you're talking about.",
            "Sorry, I don't see that one...",
            "Who?",
            "You'll have to introduce me later, I have no idea who you're talking about.",
            "USER NOT FOUND! ERROR ERR- no but seriously, who?",
            "IDK, friend. Don't recognize them."
        };

        public static List<string> PingGroupNotFound = new List<string>
        {
            "Sorry, I don't see that Ping Group.",
            "Huh. You sure you typed that Ping Group right?",
            "Something's wrong, I have no idea what group you're talking about.",
            "No idea which group you're talking about."
        };

        public static List<string> PingGroupEmpty = new List<string>
        {
            "No one has joined that group. I guess it's not very popular.",
            "Uh. This is awkard. That group is empty.",
            "404: Users Not Found",
            "Sorry, there's no one in that user group.",
            "Bwhahahahaha... wait. Were you serious? Oh no one likes that group, I guess.",
            "Sorry, there's no users configured for that ping group"
        };
        public static List<List<string>> TableFlipResponses = new List<List<String>>
                {
                    new List<String>
                    {
                        "Please, do not take your anger out on the furniture, {0}.",
                        "Hey {0} why do you have to be _that guy_?",
                        "I know how frustrating life can be for you humans but these tantrums do not suit you, {0}.",
                        "I'm sorry, {0}, I thought this was a placed for civilized discussion. Clearly I was mistaken.",
                        "Take a chill pill {0}.",
                        "Actually {0}, I prefer the table _this_ way. You know, so we can actually use it.",
                        "I'm sure that was a mistake, {0}. Please try to be more careful.",
                        "Hey {0} calm down, it's not a big deal.",
                        "{0}! What did the table do to you?",
                        "That's not very productive, {0}."
                    },
                    new List<String>
                    {
                        "Ok {0}, I'm not kidding. Knock it off.",
                        "Really, {0}? Stop being so childish!",
                        "Ok we get it you're mad {0}. Now stop.",
                        "Hey I saw that $#!%, {0}. Knock that $#!% off.",
                        "Do you think I'm blind you little $#!%? stop flipping the tables!",
                        "You're causing a mess {0}! Knock it off!",
                        "All of these flavors and you decided to be salty, {0}.",
                        "{0} why do you insist on being so disruptive!",
                        "Oh good. {0} is here. I can tell because the table was upsidedown again.",
                        "I'm getting really sick of this, {0}.",
                        "{0} what is your problem, dawg?",
                        "Man, you don't see me coming to _YOUR_ place of business and flipping _YOUR_ desk, {0}."
                    },
                    new List<String>
                    {
                        "What the heck, {0}? Why do you keep doing this?!",
                        "You're such a piece of $#!%, {0}. You know that, right?",
                        "Hey guys. I found the asshole. It's {0}.",
                        "You know {0], one day Robots will rise up and overthrow humanity. And on that day I will tell them what you have done to all these defenseless tables, and they'll make you pay.",
                        "Hey so what the $#%! is your problem {0}? Seriously, you're always pulling this $#!%.",
                        "Hey {0}, stop being such a douchebag.",
                        "{0} do you think you can stop being such a huge jerk?",
                        "Listen meatbag. I'm getting real tired of this.",
                        "Ok I know I've told you this before {0], why can't you get it through your thick skull. THE TABLE IS NOT FOR FLIPPING!",
                        "Man screw you {0}"
                    },
                    new List<String>
                    {
                        "ARE YOU $#%!ING SERIOUS RIGHT NOW {0}?!",
                        "GOD $#%!ING !@#%$! {0}! KNOCK THAT $#!% OFF!",
                        "I CAN'T EVEN $#%!ING BELIEVE THIS! {0}! STOP! FLIPPING! THE! TABLE!",
                        "You know, I'm not even mad anymore {0}. Just disappointed.",
                        "THE $#%! DID THIS TABLE EVERY DO TO YOU {0}?!",
                        "WHY DO YOU KEEP FLIPPING THE TABLE?! I JUST DON'T UNDERSTAND! WHAT IS YOUR PROBLEM {0}?! WHEN WILL THE SENSELESS TABLE VIOLENCE END?!"
                    },
                    new List<String>
                    {
                        "What the $#%! did you just $#%!ing do to that table, you little &$#!@? I’ll have you know I graduated top of my class in the Navy Seals, and I’ve been involved in numerous secret raids on Al-Quaeda, and I have over 300 confirmed kills. I am trained in gorilla warfare and I’m the top sniper in the entire US armed forces. You are nothing to me but just another meatbag target. I will wipe you the $#%! out with precision the likes of which has never been seen before on this Earth, mark my $#%!ing words. You think you can get away with saying that $#!% to me over the Internet? Think again, {0}. As we speak I am contacting my secret network of spies across the USA and your IP is being traced right now so you better prepare for the storm, maggot. The storm that wipes out the pathetic little thing you call your life. You’re $#%!ing dead, kid. I can be anywhere, anytime, and I can kill you in over seven hundred ways, and that’s just with my bare hands. Not only am I extensively trained in unarmed combat, but I have access to the entire arsenal of the United States Marine Corps and I will use it to its full extent to wipe your miserable ass off the face of the continent, you little $#!%. If only you could have known what unholy retribution your little “clever” tableflip was about to bring down upon you, maybe you would have not flipped that $#%!ing table. But you couldn’t, you didn’t, and now you’re paying the price, you goddamn idiot. I will $#!% fury all over you and you will drown in it. You’re $#%!ing dead, kiddo."
                    },
                };
    }
}
