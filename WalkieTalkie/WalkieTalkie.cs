using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace WalkieTalkie
{
    public class WalkieTalkie : WebSocketBehavior
    {
        public WalkieTalkie()
        {

        }

        protected override void OnMessage (MessageEventArgs e)
        {
            var msg = e.Data;
        }

    }


}
