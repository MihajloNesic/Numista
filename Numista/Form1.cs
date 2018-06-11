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
using System.IO;

namespace Numista
{
    public partial class Form1 : Form
    {
        private string accessToken;
        private string obversePhotoUrl, reversePhotoUrl;
        private bool isLogout = false;
        private static string NUMISTAAPI = "https://qmegas.info/numista-api/";
        private string coinExt = null;
        private Coin coin = new Coin();

        public Form1()
        {
            InitializeComponent();
        }

        public Form1(String[] coinID)
        {
            InitializeComponent();
            if (coinID.Length != 0 && coinID.Length == 1)
            {
                //MessageBox.Show(coinID[0]);
                coinExt = coinID[0];
                //searchCoinOOP(coinID[0]);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if(coinExt != null)
            {
                //string c = System.IO.File.ReadAllText(coinExt);
                string c = File.ReadLines(coinExt).First();
                //MessageBox.Show(c);
                nud_coinID.Value = Convert.ToDecimal(c);
                searchCoinOOP(c);
                tabControl1.SelectedIndex = 1;
            }
        }

        private void btn_coinsearch_Click(object sender, EventArgs e)
        {
            searchCoinOOP(nud_coinID.Value.ToString());
        }

        private void btn_randomcoin_Click(object sender, EventArgs e)
        {
            Random r = new Random();
            int randID = r.Next(1, 120000);
            nud_coinID.Value = randID;
            searchCoinOOP(Convert.ToString(randID));
        }

        private void btn_profilesearch_Click(object sender, EventArgs e)
        {
            // 101744
            searchProfileOOP(nud_profileID.Value.ToString());
            llb_profilelink.Enabled = true;
        }
        
        private string formatJson(string json)
        {
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            var f = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            return f;
        }

        private void searchCoinOOP(String coinID)
        {
            try
            {
                cmb_coin_years.Items.Clear();
                cmb_coin_refnum.Items.Clear();

                using (var webClient = new WebClient())
                {
                    var json = webClient.DownloadString(NUMISTAAPI + "coin/" + coinID);
                    dynamic array = JsonConvert.DeserializeObject(json);

                    this.coin.reset();

                    if (array.title != null)
                        this.coin.setTitle(array.title.ToString());
                    if (array.country != null)
                        this.coin.setCountry(array.country.name.ToString());
                    if (array.diameter != null)
                        this.coin.setDiameter(array.diameter.ToString());
                    if (array.weight != null)
                        this.coin.setWeight(array.weight.ToString());
                    if (array.metal != null)
                        this.coin.setMetal(array.metal.ToString());
                    if (array.orientation != null)
                        this.coin.setOrientation(array.orientation.ToString());
                    if (array.thickness != null)
                        this.coin.setThickness(array.thickness.ToString());
                    if (array.shape != null)
                        this.coin.setShape(array.shape.ToString());
                    if (array.is_commemorative != null)
                        this.coin.setCommemorative(array.is_commemorative.ToString());
                    if (array.commemorative_description != null)
                        this.coin.setCommemorativeDescription(array.commemorative_description.ToString());
                    if (array.years_range != null)
                    {
                        this.coin.setYearsRange(array.years_range.ToString());
                        foreach (dynamic year in array["years"])
                        {
                            cmb_coin_years.Items.Add(year["year"].ToString() + " " + year["remark"].ToString());
                        }
                    }
                    if (array.km != null)
                    {
                        if (array.km.ToString() != "[]")
                        {
                            this.coin.setRefNumber(array["km"][0].ToString());
                            foreach (dynamic refN in array["km"])
                            {
                                cmb_coin_refnum.Items.Add(refN.ToString());
                            }
                        }
                    }

                    if (array.images != null)
                    {
                        this.coin.setObversePhoto(array.images.obverse.preview.ToString());
                        if (array.images.obverse.fullsize != "")
                        {
                            obversePhotoUrl = array.images.obverse.fullsize.ToString();
                            llb_obverselink.Enabled = true;
                        }
                        else
                        {
                            obversePhotoUrl = coin.getObversePhoto();
                            llb_obverselink.Enabled = false;
                        }

                        this.coin.setReversePhoto(array.images.reverse.preview.ToString());
                        if (array.images.reverse.fullsize != "")
                        {
                            reversePhotoUrl = array.images.reverse.fullsize.ToString();
                            llb_reverselink.Enabled = true;
                        }
                        else
                        {
                            reversePhotoUrl = coin.getReversePhoto();
                            llb_reverselink.Enabled = false;
                        }

                        if (array.images.copyright != null && array.images.copyright != "")
                            lbl_coin_image_cpy.Text = array.images.copyright.ToString();
                        else lbl_coin_image_cpy.Text = "Coin images © Numista and their owners";
                    }

                    txb_coin_title.Text = this.coin.getTitle();
                    txb_coin_country.Text = this.coin.getCountry();
                    txb_coin_diameter.Text = this.coin.getDiameter();
                    txb_coin_weight.Text = this.coin.getWeight();
                    txb_coin_metal.Text = this.coin.getMetal();
                    txb_coin_orient.Text = this.coin.getOrientation();
                    txb_coin_thickness.Text = this.coin.getThickness();
                    txb_coin_shape.Text = this.coin.getShape();
                    txb_coin_yearsrange.Text = this.coin.getYearsRange();
                    //txb_coin_refnumber.Text = coin.getRefNumber();
                    pcb_coin_obverse.Load(this.coin.getObversePhoto());
                    pcb_coin_reverse.Load(this.coin.getReversePhoto());
                    if (this.coin.isItCommemorative())
                    {
                        chb_coin_isCommemorative.Checked = true;
                        txb_coin_commemorativedesc.Text = this.coin.getCommemorativeDescription();
                    }
                    else
                    {
                        chb_coin_isCommemorative.Checked = false;
                        txb_coin_commemorativedesc.Text = "";
                    }

                    txb_output.Text = formatJson(json);
                    btn_savecoin.Enabled = true;

                }
                llb_coinlink.Enabled = true;

                if (cmb_coin_years.Items.Count != 0)
                    cmb_coin_years.DropDownWidth = DropDownWidth(cmb_coin_years) + 3;
                else cmb_coin_years.DropDownWidth = 157;

                if (cmb_coin_refnum.Items.Count != 0)
                    cmb_coin_refnum.DropDownWidth = DropDownWidth(cmb_coin_refnum) + 3;
                else cmb_coin_refnum.DropDownWidth = 62;
            }
            catch (Exception)
            {
                MessageBox.Show("An error has occurred \nTry again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void searchProfileOOP(string profileID)
        {
            try
            {
                cmb_profile_languages.Items.Clear();

                using (var webClient = new WebClient())
                {
                    var json = webClient.DownloadString(NUMISTAAPI+"user?user_id=" + profileID + "&force_cache=1");
                    dynamic array = JsonConvert.DeserializeObject(json);

                    Profile profile = new Profile();

                    if (array.name != null)
                        profile.setUsername(array.name.ToString());
                    if (array.special_status != null)
                        profile.setTitle(array.special_status.ToString());
                    if (array.location != null)
                        profile.setLocation(array.location.ToString());
                    if (array.member_since != null)
                        profile.setMember(array.member_since.ToString());
                    if (array.exchange_coins_count != null)
                        profile.setSwap(array.exchange_coins_count.ToString());
                    if (array.website != null)
                        profile.setWeb(array.website.ToString());
                    if (array.forum_posts_count != null)
                        profile.setForum(array.forum_posts_count.ToString());
                    if (array.feedback != null)
                    {
                        profile.setFeedbackNumber(array.feedback.count.ToString());
                        profile.setFeedbackAvg(array.feedback.average.ToString());
                    }
                    if (array.is_collection_visible != null)
                        profile.setColl(array.is_collection_visible.ToString());
                    if (array.is_exchange_coins_visible != null)
                        profile.setSwapColl(array.is_exchange_coins_visible.ToString());
                    if (array.image != null)
                        profile.setAvatar(array.image.ToString());
                    if (array.languages != null)
                    {
                        foreach (dynamic lang in array["languages"])
                        {
                            cmb_profile_languages.Items.Add(lang.ToString());
                        }
                    }

                    txb_profile_username.Text = profile.getUsername();
                    txb_profile_title.Text = profile.getTitle();
                    txb_profile_location.Text = profile.getLocation();
                    txb_profile_membersince.Text = profile.getMember();
                    txb_profile_coinstoswap.Text = profile.getSwap();
                    txb_profile_website.Text = profile.getWeb();
                    txb_profile_forum.Text = profile.getForum();
                    txb_profile_feedbackcount.Text = profile.getFeedbackNumber();
                    txb_profile_feedbackavg.Text = profile.getFeedbackAvg();

                    pcb_profile_avatar.Load(profile.getAvatar());

                    if (profile.isItColl())
                        chb_profile_collectionvisible.Checked = true;
                    else chb_profile_collectionvisible.Checked = false;

                    if (profile.isItSwapColl())
                        chb_profile_swapcoins.Checked = true;
                    else chb_profile_swapcoins.Checked = false;

                    txb_output.Text = formatJson(json);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("An error has occurred \nTry again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_savecoin_Click(object sender, EventArgs e)
        {
            // save()
            try
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Numista|*.num";

                    sfd.FileName = this.coin.getCountry() + " - " + this.coin.getTitle();
                    string info = "Title: " + this.coin.getTitle() + Environment.NewLine +
                                 "Country: " + this.coin.getCountry() + Environment.NewLine +
                                 "Years Range: " + this.coin.getYearsRange() + Environment.NewLine +
                                 "Reference number: " + this.coin.getRefNumber() + Environment.NewLine +
                                 "Metal: " + this.coin.getMetal() + Environment.NewLine +
                                 "Weight: " + this.coin.getWeight() + Environment.NewLine +
                                 "Diameter: " + this.coin.getDiameter() + Environment.NewLine +
                                 "Thickness: " + this.coin.getThickness() + Environment.NewLine +
                                 "Shape: " + this.coin.getShape() + Environment.NewLine +
                                 "Orientation: " + this.coin.getOrientation()+ Environment.NewLine + 
                                 Environment.NewLine +
                                 "Saved: " + Convert.ToString(DateTime.Today.Day + "/" + DateTime.Today.Month + "/" + DateTime.Today.Year) + " " + Convert.ToString(DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(sfd.FileName, nud_coinID.Value.ToString()+Environment.NewLine + Environment.NewLine + info);
                        MessageBox.Show("Coin successfully saved!", "Save coin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    //binary write test
                    /*FileStream fs = new FileStream("test.bin", FileMode.CreateNew);
                    BinaryWriter bw = new BinaryWriter(fs);

                    bw.Write(nud_coinID.Value.ToString());

                    bw.Close();
                    fs.Close();

                    //binary read test
                    FileStream fsr = new FileStream("test.bin", FileMode.Open);
                    BinaryReader br = new BinaryReader(fsr);

                    string content = br.ReadString();
                    MessageBox.Show(content, "Binary file");

                    br.Close();
                    fsr.Close();*/

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occurred \nTry again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_cntr_getlist_Click(object sender, EventArgs e)
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    var json = webClient.DownloadString(NUMISTAAPI + "country/list/");
                    dynamic array = JsonConvert.DeserializeObject(json);

                    foreach (dynamic cntr in array["countries"])
                    {
                        string temp = cntr["name"].ToString();
                        if (!lsb_cntr_countrieslist.Items.Contains(temp))
                            lsb_cntr_countrieslist.Items.Add(temp);
                    }
                    txb_output.Text = formatJson(json);
                }
                btn_cntr_copy.Enabled = true;
                btn_cntr_getlist.Enabled = false;
            }
            catch (Exception)
            {
                MessageBox.Show("An error has occurred \nTry again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            //MessageBox.Show("Message from the developer\n\nI can ensure you, I am NOT stealing your account in any way.\n\nYou can check out source code for this software on GitHub. \nhttps://github.com/MihajloNesic/Numista\n\n\nMessage from the API author\n\nWe guarantee that your Numista login and password will NOT be stored or transferred to any third party.\nAnyway, use of that function on your own risk only.\n\nSee more: http://qmegas.info/numista-api/", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

            MessageBox.Show("I can ensure you, I am NOT stealing your account in any way.\n\nYou can check out source code for this software on GitHub. \nhttps://github.com/MihajloNesic/Numista", "Message from the developer", MessageBoxButtons.OK, MessageBoxIcon.Information);
            MessageBox.Show("We guarantee that your Numista login and password will NOT be stored or transferred to any third party.\nAnyway, use of that function on your own risk only.\n\nSee more: https://qmegas.info/numista-api/", "Message from the API author", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult dialogResult = MessageBox.Show("Do you wish to continue?", "Final call", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                login(txb_log_user.Text, txb_log_pass.Text);
            }
            else if (dialogResult == DialogResult.No)
            {
                txb_log_user.Text = "";
                txb_log_pass.Text = "";
            }
        }

        private void btn_log_logout_Click(object sender, EventArgs e)
        {
            logout(this.accessToken);

            if (isLogout)
            {
                btn_log_login.Enabled = true;
                btn_log_logout.Enabled = false;
                txb_log_user.Text = "";
                txb_log_pass.Text = "";
                txb_log_user.Enabled = true;
                txb_log_pass.Enabled = true;
                grb_log_account.Enabled = false;
                lsb_log_messages.Items.Clear();
                lsv_log_messages.Items.Clear();
            }
        }

        private void login(string user, string pass)
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    var json = webClient.DownloadString(NUMISTAAPI + "authorize/?login=" + user + "&password=" + pass);
                    dynamic array = JsonConvert.DeserializeObject(json);

                    var accessToken = "";

                    if (array.access_token != null)
                    {
                        accessToken = array.access_token.ToString();
                        isLogout = false;
                    }
                    else if (array.error == "Wrong login or password")
                        MessageBox.Show("Wrong username or password", "Login", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    else MessageBox.Show("An error has occurred \nTry again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    txb_output.Text = formatJson(json);

                    this.accessToken = accessToken;

                    // MessageBox.Show(this.accessToken, "Token");

                    if (this.accessToken != "")
                    {
                        btn_log_login.Enabled = false;
                        btn_log_logout.Enabled = true;
                        //txb_log_user.Text = "";
                        //txb_log_pass.Text = "";
                        txb_log_user.Enabled = false;
                        txb_log_pass.Enabled = false;
                        grb_log_account.Enabled = true;
                        MessageBox.Show("Successfully logged in", "Login", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("An error has occurred \nTry again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void logout(String access_token)
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    var json = webClient.DownloadString(NUMISTAAPI + "authorize/destroy/?access_token=" + access_token);
                    dynamic array = JsonConvert.DeserializeObject(json);

                    if (array.destroyed == "true")
                    {
                        isLogout = true;
                        MessageBox.Show("Successfully logged out", "Logout", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (array.error == "Wrong access token")
                    {
                        isLogout = false;
                        MessageBox.Show("An error has occurred", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    txb_output.Text = formatJson(json);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("An error has occurred \nTry again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_log_getmessages_Click(object sender, EventArgs e)
        {
            try
            {
                lsb_log_messages.Items.Clear();
                lsv_log_messages.Items.Clear();

                using (var webClient = new WebClient())
                {
                    var json = webClient.DownloadString(NUMISTAAPI + "messages/inbox/?access_token=" + this.accessToken);
                    dynamic array = JsonConvert.DeserializeObject(json);

                    foreach (dynamic message in array["messages"])
                    {
                        lsb_log_messages.Items.Add(message["title"].ToString());
                        lsb_log_messages.Items.Add(message["sender"]["name"].ToString());
                        lsb_log_messages.Items.Add(message["time"].ToString());

                        if(message["is_new"].ToString() == "True")
                            lsb_log_messages.Items.Add("Yes");
                        else lsb_log_messages.Items.Add("No");
                        //lsb_log_messages.Items.Add(message["is_new"].ToString());

                        if(message["is_replied"].ToString() == "True")
                            lsb_log_messages.Items.Add("Yes");
                        else lsb_log_messages.Items.Add("No");
                        //lsb_log_messages.Items.Add(message["is_replied"].ToString());
                    }

                    txb_output.Text = formatJson(json);
                }
                addMessagesToList();
            }
            catch(Exception)
            {
                // MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show("Could not get messages. \nTry again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private void llb_obverselink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(this.obversePhotoUrl);
        }

        private void llb_reverselink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(this.reversePhotoUrl);
        }

        // Handle items to not be selectable in combobox
        private void cmb_coin_refnum_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmb_coin_refnum.SelectedIndex = -1;
        }

        private void cmb_coin_years_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmb_coin_years.SelectedIndex = -1;
        }

        private void cmb_profile_languages_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmb_profile_languages.SelectedIndex = -1;
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

        private void link_lbl_apilink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(NUMISTAAPI);
        }

        private void link_lbl_githubnumista_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/MihajloNesic/Numista");
        }

        private void link_lbl_numista_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://numista.com/");
        }

        private void link_lbl_mnnumista_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://en.numista.com/echanges/profil.php?id=60438");
        }

        private void link_lbl_mnesiccoins_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://mnesiccoins.ml");
        }

        /*private void btn_test_findnotpublished_Click(object sender, EventArgs e)
        {
            txb_output.Text = "=== Found used coin ID's ===" + Environment.NewLine;

            using (var webClient = new WebClient())
            {
                // "/1" is to disable cache
                for (int i = 89300; i < 89400; i++)
                {
                    var json = webClient.DownloadString("http://qmegas.info/numista-api/coin/" + i);
                    dynamic array = JsonConvert.DeserializeObject(json);

                    var error = "";

                    if (array.error != null)
                        error = array.error.ToString();

                    if (error == "Coin not published")
                        txb_test_notpublished.Text += i + Environment.NewLine;
                    else if(error == "Coin not found")
                        txb_test_notfound.Text += i + Environment.NewLine;
                    else txb_output.Text += i + Environment.NewLine;
                }
            }
        }*/
    }
 }
