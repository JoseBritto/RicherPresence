using DiscordRPC;
using DiscordRPC.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RicherPresence
{
    public partial class Form1 : Form
    {
        DiscordRpcClient client;
        public Form1()
        {
            InitializeComponent();
        }

        public void Log(string s)
        {
            Output.Text = s + "\r\n" + Output.Text;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            OpenDiag.FileOk += OpenDiag_FileOk;
            OpenDiag.Filter = "Json Files|*.json";
            SaveDiag.FileOk += SaveDiag_FileOk;
            SaveDiag.DefaultExt = "json";
            SaveDiag.Filter = "Json Files|*.json";
            SaveDiag.AddExtension = true;

            
            try
            {
                if (!File.Exists("data.json"))
                    return;

                var json = File.ReadAllText("data.json");

                var save = JsonConvert.DeserializeObject<SaveData>(json);

                ClientIdTextBox.Text = save.cid;

                DetailsTextBox.Text = save.RichPresence.Details;

                StateTextBox.Text = save.RichPresence.State;

                SmallTextTextBox.Text = save.RichPresence.Assets.SmallImageText;
                SmallAssetTextBox.Text = save.RichPresence.Assets.SmallImageKey;

                LargeTextTextBox.Text = save.RichPresence.Assets.LargeImageText;
                LargeAssetTextBox.Text = save.RichPresence.Assets.LargeImageKey;

                // PartySizeBox.Text = save.RichPresence.Party?.Size.ToString();
                // PartyMaxBox.Text = save.RichPresence.Party?.Max.ToString();

                if (!save.RichPresence.HasButtons())
                    return;

                if (save.RichPresence.Buttons.Length == 0)
                    return;

                Button1Box.Text = save.RichPresence.Buttons[0].Label;
                Button1LinkBox.Text = save.RichPresence.Buttons[0].Url;

                if (save.RichPresence.Buttons.Length == 1)
                    return;
                Button2Box.Text = save.RichPresence.Buttons[1].Label;
                Button2LinkBox.Text = save.RichPresence.Buttons[1].Url;


            }
            catch (Exception ex)
            {
                Log($"ERROR: {ex.Message}");
            }
        }

        private void UpdateBtn_Click(object sender, EventArgs e)
        {   
            var presence = GetPresence();
            
            if (presence == null)
                return;

            client.SetPresence(presence);
        }

        private RichPresence GetPresence()
        {
            if (!long.TryParse(ClientIdTextBox.Text.Trim(), out var cid))
            {
                Log("ERROR: Provide a valid client id!");
                return null;
            }

            if (client == null)
            {
                client = new DiscordRpcClient(cid.ToString());


                client.OnPresenceUpdate += (sender, e) =>
                {
                    Output.Invoke((MethodInvoker)delegate {
                        var json = JsonConvert.SerializeObject(e.Presence);
                        // Running on the UI thread
                        Log($"Received Update! {json}");
                    });

                };

                client.Initialize();
            }

            var presence = new RichPresence
            {
                Assets = new Assets
                {
                    LargeImageKey = LargeAssetTextBox.Text,
                    LargeImageText = LargeTextTextBox.Text,
                    SmallImageKey = SmallAssetTextBox.Text,
                    SmallImageText = SmallTextTextBox.Text
                },

                Details = DetailsTextBox.Text,
                State = StateTextBox.Text,
                Buttons = GetButtons(),
                Party = GetParty(),

            };

            return presence;
        }

        private Party GetParty()
        {
            return null;
           /* var party = new Party();

            if (string.IsNullOrWhiteSpace(PartySizeBox.Text))
                return null;

            if (!int.TryParse(PartySizeBox.Text, out int size))
                return null;

            if (!int.TryParse(PartyMaxBox.Text, out int max))
                max = size;
            
            party.Size = size;
            party.Max = max;

            return party;*/
        }

        private DiscordRPC.Button[] GetButtons()
        {
            string text1 = Button1Box.Text.Trim();
            string link1 = Button1LinkBox.Text?.Trim();

            if (!string.IsNullOrWhiteSpace(text1))
            {
                if (link1 != null && !Uri.IsWellFormedUriString(link1, UriKind.Absolute))
                {
                    Log("ERROR: Link for button 1 is not valid");
                    return null;
                }
            }

            string text2 = Button2Box.Text;
            string link2 = Button2LinkBox.Text?.Trim();

            if (!string.IsNullOrWhiteSpace(text2))
            {
                if (link2 != null && !Uri.IsWellFormedUriString(link2, UriKind.Absolute))
                {
                    Log("ERROR: Link for button 2 is not valid");
                    return null;
                }
            }
            var list = new List<DiscordRPC.Button>();

            if (!string.IsNullOrWhiteSpace(text1))
            {
                list.Add(new DiscordRPC.Button { Label = text1, Url = link1 });
            }

            if (!string.IsNullOrWhiteSpace(text2))
            {
                list.Add(new DiscordRPC.Button { Label = text2, Url = link2 });
            }

            if (list.Count > 0)
                return list.ToArray();

            return null;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var res = OpenDiag.ShowDialog();
            
        }

        private void OpenDiag_FileOk(object sender, CancelEventArgs e)
        {
            Log(OpenDiag.FileName);
            if (string.IsNullOrWhiteSpace(OpenDiag.FileName))
                return;

            var json = File.ReadAllText(OpenDiag.FileName);

            try
            {
                var save = JsonConvert.DeserializeObject<SaveData>(json);

                ClientIdTextBox.Text = save.cid;

                DetailsTextBox.Text = save.RichPresence.Details;

                StateTextBox.Text = save.RichPresence.State;

                SmallTextTextBox.Text = save.RichPresence.Assets.SmallImageText;
                SmallAssetTextBox.Text = save.RichPresence.Assets.SmallImageKey;

                LargeTextTextBox.Text = save.RichPresence.Assets.LargeImageText;
                LargeAssetTextBox.Text = save.RichPresence.Assets.LargeImageKey;

               // PartySizeBox.Text = save.RichPresence.Party?.Size.ToString();
               // PartyMaxBox.Text = save.RichPresence.Party?.Max.ToString();

                if (!save.RichPresence.HasButtons())
                    return;

                if (save.RichPresence.Buttons.Length == 0)
                    return;

                Button1Box.Text = save.RichPresence.Buttons[0].Label;
                Button1LinkBox.Text = save.RichPresence.Buttons[0].Url;

                if (save.RichPresence.Buttons.Length == 1)
                    return;
                Button2Box.Text = save.RichPresence.Buttons[1].Label;
                Button2LinkBox.Text = save.RichPresence.Buttons[1].Url;


            }
            catch (Exception ex)
            {
                Log($"ERROR: {ex.Message}");
            }


        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            var result = SaveDiag.ShowDialog();
        }

        private void SaveDiag_FileOk(object sender, CancelEventArgs e)
        {

            var presence = GetPresence();

            if (presence == null)
            {
                Log("ERROR: Save Failed!");
                return;
            }
            var save = new SaveData { cid = ClientIdTextBox.Text, RichPresence = presence };

            var json = JsonConvert.SerializeObject(save, Formatting.Indented);

            if (string.IsNullOrWhiteSpace(SaveDiag.FileName))
                return;

            File.WriteAllText(SaveDiag.FileName, json);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
           // Hide();
        }

        private void ShowButton_Click(object sender, EventArgs e)
        {
            Show();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            try
            {
                var presence = GetPresence();

                if (presence == null)
                {
                    Dispose();
                    return;
                }
                var save = new SaveData { cid = ClientIdTextBox.Text, RichPresence = presence };

                var json = JsonConvert.SerializeObject(save, Formatting.Indented);


                File.WriteAllText("data.json", json);

                Dispose();

            }
            catch { }

            void Dispose()
            {
                if (client != null)
                    client.Dispose();

                client = null;
            }
            Application.Exit();
            Environment.Exit(0);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }

    public class SaveData
    {
        public string cid;

        public RichPresence RichPresence;
    }
}
