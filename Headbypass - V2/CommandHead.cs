using AdvancedBot.client;
using AdvancedBot.client.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeadBypass
{
    class CommandHead : CommandBase
    {
        public CommandHead(MinecraftClient cli)
            : base(cli, "Head", "Bypass head captcha.", "head")
        {
        }
        public override CommandResult Run(string alias, string[] args)
        {
            new HBypass(Client).Run();
            return CommandResult.Success;
        }
    }
}
