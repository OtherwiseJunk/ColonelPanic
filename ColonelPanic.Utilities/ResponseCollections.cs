using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonelPanic.Utilities
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
                "I mean. It's more likely than Geo playing Tf2, at least?",
                "You know it!",
        };

        public static List<string> UserNotFoundResponses = new List<string>
        {
            "I don't know who you're talking about.",
            "Sorry, I don't see that one...",
            "Who?",
            "You'll have to introduce me later, I have no idea who you're talking about.",
            "USER NOT FOUND! ERROR ERR- no but seriously, who?",
            "IDK dude. Don't recognize them."
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
    }
}
