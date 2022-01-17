using AdvancedBot.Plugins;
using AdvancedBot.client;
using System;
using AdvancedBot.client.Commands;
using System.Threading.Tasks;
using System.Threading;

namespace HeadBypass
{
    public class MainHead : IPlugin
    {

        public MainHead()
        {
            
        }

        CommandBase headBypass;

        public void onClientConnect(MinecraftClient client)
        {
            if (client.CmdManager.GetCommand("head") == null)
            {
                headBypass = new CommandHead(client);
                client.CmdManager.Commands.Add(headBypass);
            }

        }

        public void onReceiveChat(string chat, byte pos, MinecraftClient client)
        {}

        public void OnReceivePacket(ReadBuffer pkt, MinecraftClient client)
        {

            try
            {
                if(pkt.ID == 0x2D)
                {
                    Task.Run(async () => {
                        try
                        {
                            Thread.Sleep(5000);
                            new HBypass(client).Run();
                        }
                        catch (Exception ex)
                        {}
                    });
                }
            }catch(Exception ex) { }

        }

        public void onSendChat(string chat, MinecraftClient client)
        {}

        public void OnSendPacket(IPacket packet, MinecraftClient client)
        {}

        public void Tick()
        { }

        public void Unload()
        {}
    }
}
