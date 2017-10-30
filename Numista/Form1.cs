using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using Newtonsoft.Json;

namespace Numista
{
    public partial class Form1 : Form
    {
        private string accessToken;
        private string obversePhotoUrl, reversePhotoUrl;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //btn_randomcoin.Text = "";
        }

        private void btn_coinsearch_Click(object sender, EventArgs e)
        {
            //searchCoin(txb_coinid.Text);
            searchCoin(nud_coinID.Value.ToString());
        }

        private void btn_randomcoin_Click(object sender, EventArgs e)
        {
            Random r = new Random();
            int randID = r.Next(1, 100000);

            //txb_coinid.Text = randID.ToString();
            nud_coinID.Value = randID;
            searchCoin(Convert.ToString(randID));
        }

        private void btn_profilesearch_Click(object sender, EventArgs e)
        {
            searchProfile(nud_profileID.Value.ToString());
            llb_profilelink.Enabled = true;
        }

        private string formatJson(string json)
        {
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            var f = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            return f;
        }

        private void searchCoin(String coinID)
        {
            cmb_coin_years.Items.Clear();

            using (var webClient = new WebClient())
            {
                // "/1" is to disable cache
                var json = webClient.DownloadString("http://qmegas.info/numista-api/coin/" + coinID);
                dynamic array = JsonConvert.DeserializeObject(json);

                var title = "";
                var country = "";
                var diameter = "";
                var weight = "";
                var metal = "";
                var orientation = "";
                var thickness = "";
                var shape = "";
                var yearsR = "";
                var kmNum = "";

                var obverse_photo = "https://en.numista.com/catalogue/photos/no-obverse-en.png";
                var reverse_photo = "https://en.numista.com/catalogue/photos/no-reverse-en.png";
                obversePhotoUrl = obverse_photo.ToString();
                reversePhotoUrl = reverse_photo.ToString();

                if (array.title != null)
                    title = array.title.ToString();

                if (array.country != null)
                    country = array.country.ToString();

                if (array.diameter != null)
                    diameter = array.diameter.ToString();

                if (array.weight != null)
                    weight = array.weight.ToString();

                if (array.metal != null)
                    metal = array.metal.ToString();

                if (array.orientation != null)
                    orientation = array.orientation.ToString();

                if (array.shape != null)
                    shape = array.shape.ToString();

                if (array.thickness != null)
                    thickness = array.thickness.ToString();


                if (array.images != null)
                {
                    obverse_photo = array.images.obverse.preview.ToString();

                    if (array.images.obverse.fullsize != "")
                    {
                        obversePhotoUrl = array.images.obverse.fullsize.ToString();
                        llb_obverselink.Enabled = true;
                    }
                    else
                    {
                        obversePhotoUrl = obverse_photo.ToString();
                        llb_obverselink.Enabled = false;
                    }

                    reverse_photo = array.images.reverse.preview.ToString();

                    if (array.images.reverse.fullsize != "")
                    {
                        reversePhotoUrl = array.images.reverse.fullsize.ToString();
                        llb_reverselink.Enabled = true;
                    }
                    else
                    {
                        reversePhotoUrl = reverse_photo.ToString();
                        llb_reverselink.Enabled = false;
                    }
                }
                else
                {
                    llb_obverselink.Enabled = false;
                    llb_reverselink.Enabled = false;
                }


                if (array.km != null)
                    if (array.km.ToString() != "[]")
                        kmNum = array["km"][0];

                if (array.years_range != null)
                {
                    yearsR = array.years_range.ToString();
                    foreach (dynamic year in array["years"])
                    {
                        cmb_coin_years.Items.Add(year["year"].ToString()+ " " + year["remark"].ToString());
                    }
                }

                txb_output.Text = formatJson(json);

                txb_coin_title.Text = title;
                txb_coin_country.Text = country;
                txb_coin_diameter.Text = diameter;
                txb_coin_weight.Text = weight;
                txb_coin_metal.Text = metal;
                txb_coin_orient.Text = orientation;
                txb_coin_thickness.Text = thickness;
                txb_coin_shape.Text = shape;
                txb_coin_yearsrange.Text = yearsR;
                txb_coin_refnumber.Text = kmNum;

                pcb_coin_obverse.Load(obverse_photo);
                pcb_coin_reverse.Load(reverse_photo);
            }
            llb_coinlink.Enabled = true;

            if (cmb_coin_years.Items.Count != 0)
                cmb_coin_years.DropDownWidth = DropDownWidth(cmb_coin_years) + 3;
            else cmb_coin_years.DropDownWidth = 157;
        }

        private void searchProfile(string profileID)
        {
            using (var webClient = new WebClient())
            {
                // "/1" is to disable cache
                var json = webClient.DownloadString("http://qmegas.info/numista-api/user/" + profileID);
                dynamic array = JsonConvert.DeserializeObject(json);

                var username = "";
                var title = "";
                var location = "";
                var member = "";
                var swap = "";
                var web = "";
                var forum = "";
                var coll = false;
                var swapC = false;
                var feedbackN = "";
                var feedbackA = "";

                var avatar = "https://en.numista.com/echanges/avatar.png";

                if (array.name != null)
                    username = array.name.ToString();

                if (array.special_status != null)
                    title = array.special_status.ToString();

                if (array.location != null)
                    location = array.location.ToString();

                if (array.member_since != null)
                    member = array.member_since.ToString();

                if (array.exchange_coins_count != null)
                    swap = array.exchange_coins_count.ToString();

                if (array.website != null)
                    web = array.website.ToString();

                if (array.forum_posts_count != null)
                    forum = array.forum_posts_count.ToString();

                if (array.is_collection_visible != null) {
                    coll = array.is_collection_visible;
                        if (coll == true)
                            chb_profile_collectionvisible.Checked = true;
                        else chb_profile_collectionvisible.Checked = false;
                }

                if (array.is_exchange_coins_visible != null)
                {
                    swapC = array.is_exchange_coins_visible;
                    if (swapC == true)
                        chb_profile_swapcoins.Checked = true;
                    else chb_profile_swapcoins.Checked = false;
                }

                if (array.feedback != null)
                {
                    feedbackN = array.feedback.count.ToString();
                    feedbackA = array.feedback.average.ToString();
                }

                if (array.image != null)
                    avatar = array.image.ToString();

                //var wants = array.user_notes.wants.ToString();

                txb_output.Text = formatJson(json);

                txb_profile_username.Text = username;
                txb_profile_title.Text = title;
                txb_profile_location.Text = location;
                txb_profile_membersince.Text = member;
                txb_profile_coinstoswap.Text = swap;
                txb_profile_website.Text = web;
                txb_profile_forum.Text = forum;
                txb_profile_feedbackcount.Text = feedbackN;
                txb_profile_feedbackavg.Text = feedbackA;

                pcb_profile_avatar.Load(avatar);

             }
         }

        // Handle items to not be selectable in combobox
        private void cmb_coin_years_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmb_coin_years.SelectedIndex = -1;
        }

        private void btn_cntr_getlist_Click(object sender, EventArgs e)
        {
            using (var webClient = new WebClient())
            {
                var json = webClient.DownloadString("http://qmegas.info/numista-api/country/list/");
                dynamic array = JsonConvert.DeserializeObject(json);

                foreach (dynamic cntr in array["countries"])
                    lsb_cntr_countrieslist.Items.Add(cntr["name"].ToString());
                txb_output.Text = formatJson(json);
            }
            btn_cntr_copy.Enabled = true;
            btn_cntr_getlist.Enabled = false;
        }

        private void btn_cntr_copy_Click(object sender, EventArgs e)
        {
            string temp = "";

            foreach (object item in lsb_cntr_countrieslist.Items)
                temp += item.ToString() + "\r\n";

            Clipboard.SetText(temp);
        }

        private void btn_log_login_Click(object sender, EventArgs e)
        {
            MessageBox.Show("I can ensure you, I am NOT stealing your account in any way.\nYou can check out source code for this software on GitHub. \nhttps://github.com/MihajloNesic/Numista", "Message from the developer", MessageBoxButtons.OK, MessageBoxIcon.Information);

            login(txb_log_user.Text, txb_log_pass.Text);

            if (this.accessToken != "")
            {
                btn_log_login.Enabled = false;
                btn_log_logout.Enabled = true;
                txb_log_user.Text = "";
                txb_log_pass.Text = "";
                grb_log_account.Enabled = true;
            }
        }

        private void btn_log_logout_Click(object sender, EventArgs e)
        {
            logout(this.accessToken);

            btn_log_login.Enabled = true;
            btn_log_logout.Enabled = false;
            txb_log_user.Text = "";
            txb_log_pass.Text = "";
            grb_log_account.Enabled = false;
            lsb_log_messages.Items.Clear();
            lsv_log_messages.Items.Clear();
        }

        private void login(string user, string pass)
        {
            using (var webClient = new WebClient())
            {
                var json = webClient.DownloadString("http://qmegas.info/numista-api/authorize?login="+user+"&password="+pass);
                dynamic array = JsonConvert.DeserializeObject(json);

                var accessToken = "";

                if (array.access_token != null)
                    accessToken = array.access_token.ToString();

                txb_output.Text = formatJson(json);

                this.accessToken = accessToken;
            }
        }

        private void logout(String access_token)
        {
            using (var webClient = new WebClient())
            {
                var json = webClient.DownloadString("http://qmegas.info/numista-api/authorize/destroy?access_token="+access_token);
                txb_output.Text = formatJson(json);
            }
        }

        private void btn_log_getmessages_Click(object sender, EventArgs e)
        {
            lsb_log_messages.Items.Clear();
            using (var webClient = new WebClient())
            {
                var json = webClient.DownloadString("http://qmegas.info/numista-api/messages/inbox?access_token="+this.accessToken);
                dynamic array = JsonConvert.DeserializeObject(json);
                
                foreach (dynamic message in array["messages"])
                {
                    lsb_log_messages.Items.Add(message["title"].ToString());
                    lsb_log_messages.Items.Add(message["sender"]["name"].ToString());
                    lsb_log_messages.Items.Add(message["time"].ToString());
                    lsb_log_messages.Items.Add(message["is_new"].ToString());
                    lsb_log_messages.Items.Add(message["is_replied"].ToString());
                }
                
                txb_output.Text = formatJson(json);
            }
            addMessagesToList();
        }

        private void addMessagesToList()
        {
            for (int i = 0; i < (lsb_log_messages.Items.Count - 1); i += 5)
            {
                ListViewItem lvi = new ListViewItem(lsb_log_messages.Items[i].ToString());
                lvi.SubItems.Add(lsb_log_messages.Items[i + 1].ToString());
                lvi.SubItems.Add(lsb_log_messages.Items[i + 2].ToString());
                lvi.SubItems.Add(lsb_log_messages.Items[i + 3].ToString());
                lvi.SubItems.Add(lsb_log_messages.Items[i + 4].ToString());
                lsv_log_messages.Items.Add(lvi);
            }
        }

        private int DropDownWidth(ComboBox myCombo)
        {
            int maxWidth = 0;
            int temp = 0;
            Label label1 = new Label();

            foreach (var obj in myCombo.Items)
            {
                label1.Text = obj.ToString();
                temp = label1.PreferredWidth;
                if (temp > maxWidth)
                {
                    maxWidth = temp;
                }
            }
            label1.Dispose();
            return maxWidth;
        }

        private void llb_coinlink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string link = "https://en.numista.com/catalogue/pieces" + nud_coinID.Value.ToString() + ".html";
            System.Diagnostics.Process.Start(link);
        }

        private void llb_profilelink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string link = "https://en.numista.com/echanges/profil.php?id=" + nud_profileID.Value.ToString();
            System.Diagnostics.Process.Start(link);
        }

        private void llb_obverselink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(this.obversePhotoUrl);
        }

        private void llb_reverselink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(this.reversePhotoUrl);
        }
    }
 }
