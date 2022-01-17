using AdvancedBot;
using AdvancedBot.client;
using AdvancedBot.client.NBT;
using AdvancedBot.Viewer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HeadBypass
{

    using NamedColor = Tuple<string, int>;
    public class HBypass
    {

        public static Dictionary<MinecraftClient, long> delay = new Dictionary<MinecraftClient, long>();

        public MinecraftClient client;

        public MinecraftClient getClient()
        {
            return client;
        }

        public HBypass(MinecraftClient client)
        {
            this.client = client;
        }

        private Inventory chest;

        public void Run()
        {
            try
            {

                if (delay.ContainsKey(client))
                {
                    if (Utils.GetTimestamp() - delay[client] < 5000) return;
                }

                delay[client] = Utils.GetTimestamp();

                this.chest = getClient().OpenWindow;

                if (this.chest == null) return;

                var title = Utils.StripColorCodes(chest.Title);

                if (!title.ToLower().Contains("cabeça") &&
                    !title.ToLower().Contains("cabeca") &&
                    !title.ToLower().Contains("clique") &&
                    !title.ToLower().Contains("clica") &&
                    !title.ToLower().Contains("click")) return;

                if (!inOnlyHead(chest)) return;

                String url = getUrlToClick();

                if (url == null) return;

                DoBypassAsync(url).ContinueWith(task =>
                {
                    getClient().PrintToChat("§c[HeadBypass] Ocorreu um erro ao tentar burlar o teste");
                    this.chest = getClient().OpenWindow;
                }, TaskContinuationOptions.OnlyOnFaulted);
            }
            catch (Exception ex) { };
        }

        public String getUrlToClick()
        {
            this.chest = getClient().OpenWindow;
            //Program.FrmMain.DebugConsole($"({getClient().Username}) Cor={colorToClick}");
            //Debug.WriteLine("chest> " + (chest == null ? "true" : "False"));
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            for (int i = 0; i < this.chest.NumSlots; i++)
            {
                var stack = this.chest.Slots[i];
                if (stack != null && stack.NBTData != null)
                {

                    var tag = stack.NBTData;

                    //SkullOwner/Properties/textures/Value
                    var textures = tag.GetCompound("SkullOwner")
                                      .GetCompound("Properties")
                                      .GetList("textures");

                    if (textures != null && textures.Count > 0)
                    {
                        string base64 = ((CompoundTag)textures.GetTag(0)).GetString("Value");
                        byte[] data = Convert.FromBase64String(base64);
                        string json = Encoding.UTF8.GetString(data);
                        int num = 0;
                        var m = Regex.Match(json, @"url:\s*""(.+?)""");
                        if (m.Success)
                        {
                            string url = m.Groups[1].Value;
                            if (dictionary.ContainsKey(url))
                            {
                                num = dictionary[url];
                            }
                            else
                            {
                                dictionary.Add(url, 0);
                            }
                            num++;
                            dictionary[url] = num;
                        }
                    }

                }
            }
            dictionary.OrderBy(x => x.Value).Select(x => x.Key);
            return dictionary.Keys.Last<string>();
        }

        public bool inOnlyHead(Inventory inv)
        {
            int itemcount = 0;
            int headcount = 0;
            for (int i = 0; i < inv.NumSlots; i++)
            {
                var stack = inv.Slots[i];
                if (stack != null && stack.NBTData != null)
                {
                    itemcount++;
                    var tag = stack.NBTData;

                    //SkullOwner/Properties/textures/Value
                    var textures = tag.GetCompound("SkullOwner")
                                      .GetCompound("Properties")
                                      .GetList("textures");

                    if (textures != null && textures.Count > 0)
                    {
                        headcount++;
                    }
                }
            }
            return headcount == itemcount;
        }
        public static Dictionary<string, string> urls = new Dictionary<string, string>();
        private async Task DoBypassAsync(string colorToClick)
        {
            this.chest = getClient().OpenWindow;
            //Program.FrmMain.DebugConsole($"({getClient().Username}) Cor={colorToClick}");
            //Debug.WriteLine("chest> " + (chest == null ? "true" : "False"));
            for (int i = 0; i < this.chest.NumSlots; i++)
            {
                var stack = this.chest.Slots[i];
                if (stack != null && stack.NBTData != null)
                {
                    var tag = stack.NBTData;

                    //SkullOwner/Properties/textures/Value
                    var textures = tag.GetCompound("SkullOwner")
                                      .GetCompound("Properties")
                                      .GetList("textures");

                    if (textures != null && textures.Count > 0)
                    {
                        string base64 = ((CompoundTag)textures.GetTag(0)).GetString("Value");
                        byte[] data = Convert.FromBase64String(base64);
                        string json = Encoding.UTF8.GetString(data);

                        var m = Regex.Match(json, @"url:\s*""(.+?)""");
                        if (m.Success)
                        {
                            string url = m.Groups[1].Value;

                            if (url.EqualsIgnoreCase(colorToClick))
                            {
                                Program.FrmMain.DebugConsole(url);
                                chest.Click(getClient(), (short)i, false, false);
                                getClient().PrintToChat("§a[HeadBypass] O teste foi burlado!");
                                chest = null;
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
