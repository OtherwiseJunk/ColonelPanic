using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalkieTalkie
{
    public class Request
    {
        public string Source;
        public string Timestamp;

        public Request(string source)
        {
            Source = source;
            Timestamp = DateTime.Now.ToString();
        }

    }

    public class CommandRequest : Request
    {
        public string Command;
        public string Arguments;

        public CommandRequest(string command, string arguments, string source):base(source)
        {           
            Command = command;
            Arguments = arguments;
        }
    }
}
