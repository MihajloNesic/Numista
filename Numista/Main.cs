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
    public partial class Main : Form
    {
        private string accessToken;
        private string obversePhotoUrl, reversePhotoUrl;
        private bool isLogout = false;
        private static string NUMISTAAPI = "https://qmegas.info/numista-api/";
        private string coinExt = null;
        private Coin coin = new Coin();
        private Stream stream;

        public Main()
        {
            InitializeComponent();
        }

        public Main(String[] coinID)
        {
            InitializeComponent();
            if (coinID.Length != 0 && coinID.Length == 1)
            {
                coinExt = coinID[0];
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if(coinExt != null)
            {
                string c = File.ReadLines(coinExt).First();
                nud_coinID.Value = Convert.ToDecimal(c);
                SearchCoin(c);
                tabControl1.SelectedIndex = 1;
            }
            
        }

        // Coin search button handler
        private void btn_coinsearch_Click(object sender, EventArgs e)
        {
            SearchCoin(nud_coinID.Value.ToString());
        }

        // Random coin button handler
        private void btn_randomcoin_Click(object sender, EventArgs e)
        {
            Random r = new Random();
            int randID = r.Next(1, 180000);
            nud_coinID.Value = randID;
            SearchCoin(Convert.ToString(randID));
        }

        // Profile search button handler
        private void btn_profilesearch_Click(object sender, EventArgs e)
        {
            SearchProfile(nud_profileID.Value.ToString());
            llb_profilelink.Enabled = true;
        }
        
        // Pretty prints the json
        private string formatJson(string json)
        {
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            var f = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            return f;
        }

        /// <summary>
        /// Search coin
        /// </summary>
        /// <param name="coinID">Numista ID of the coin</param>
        private void SearchCoin(String coinID)
        {
            try
            {
                cmb_coin_years.Items.Clear();
                cmb_coin_refnum.Items.Clear();

                using (var webClient = new WebClient())
                {
                    var json = webClient.DownloadString(NUMISTAAPI + "coin/" + coinID);
                    dynamic array = JsonConvert.DeserializeObject(json);

                    coin = new Coin();

                    coin.Id = Int32.Parse(coinID);

                    if (array.title != null)
                        coin.Title = array.title.ToString();
                    if (array.country != null)
                        coin.Country = array.country.name.ToString();
                    if (array.diameter != null)
                        coin.Diameter = array.diameter.ToString();
                    if (array.weight != null)
                        coin.Weight = array.weight.ToString();
                    if (array.metal != null)
                        coin.Metal = array.metal.ToString();
                    if (array.orientation != null)
                        coin.Orientation = array.orientation.ToString();
                    if (array.thickness != null)
                        coin.Thickness = array.thickness.ToString();
                    if (array.shape != null)
                        coin.Shape = array.shape.ToString();
                    if (array.is_commemorative != null)
                        coin.IsCommemorative = array.is_commemorative;
                    if (array.commemorative_description != null)
                        coin.CommemorativeDescription = array.commemorative_description.ToString();
                    if (array.years_range != null)
                    {
                        coin.YearsRange = array.years_range.ToString();
                        foreach (dynamic year in array["years"])
                        {
                            cmb_coin_years.Items.Add(year["year"].ToString() + " " + year["remark"].ToString());
                        }
                    }
                    if (array.km != null)
                    {
                        if (array.km.ToString() != "[]")
                        {
                            coin.RefNumber = array["km"][0].ToString();
                            foreach (dynamic refN in array["km"])
                            {
                                cmb_coin_refnum.Items.Add(refN.ToString());
                            }
                        }
                    }

                    if (array.images != null)
                    {
                        coin.ObversePhoto = array.images.obverse.preview.ToString();
                        if (array.images.obverse.fullsize != "")
                        {
                            obversePhotoUrl = array.images.obverse.fullsize.ToString();
                            llb_obverselink.Enabled = true;
                        }
                        else
                        {
                            obversePhotoUrl = coin.ObversePhoto;
                            llb_obverselink.Enabled = false;
                        }

                        coin.ReversePhoto = array.images.reverse.preview.ToString();
                        if (array.images.reverse.fullsize != "")
                        {
                            reversePhotoUrl = array.images.reverse.fullsize.ToString();
                            llb_reverselink.Enabled = true;
                        }
                        else
                        {
                            reversePhotoUrl = coin.ReversePhoto;
                            llb_reverselink.Enabled = false;
                        }

                        if (array.images.copyright != null && array.images.copyright != "")
                            lbl_coin_image_cpy.Text = array.images.copyright.ToString();
                        else lbl_coin_image_cpy.Text = "Coin images © Numista and their owners";
                    }

                    txb_coin_title.Text = coin.Title;
                    txb_coin_country.Text = coin.Country;
                    txb_coin_diameter.Text = coin.Diameter;
                    txb_coin_weight.Text = coin.Weight;
                    txb_coin_metal.Text = coin.Metal;
                    txb_coin_orient.Text = coin.Orientation;
                    txb_coin_thickness.Text = coin.Thickness;
                    txb_coin_shape.Text = coin.Shape;
                    txb_coin_yearsrange.Text = coin.YearsRange;
                    pcb_coin_obverse.Load(coin.ObversePhoto);
                    pcb_coin_reverse.Load(coin.ReversePhoto);

                    if (coin.IsCommemorative)
                    {
                        chb_coin_isCommemorative.Checked = true;
                        txb_coin_commemorativedesc.Text = coin.CommemorativeDescription;
                    }
                    else
                    {
                        chb_coin_isCommemorative.Checked = false;
                        txb_coin_commemorativedesc.Text = "";
                    }

                    if (!coin.Title.Equals(""))
                    {
                        txb_output.Text = formatJson(json);
                        btn_savecoin.Enabled = true;

                        addToCoinList(coin.Id+"", coin.Country, coin.Title);
                    }

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
        
        /// <summary>
        /// Search profile
        /// </summary>
        /// <param name="profileID">Numista ID of the user</param>
        private void SearchProfile(string profileID)
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
                        profile.Username = array.name.ToString();
                    if (array.special_status != null)
                        profile.Title = array.special_status.ToString();
                    if (array.location != null)
                        profile.Location = array.location.ToString();
                    if (array.member_since != null)
                        profile.Member = array.member_since.ToString() ;
                    if (array.exchange_coins_count != null)
                        profile.Swap = array.exchange_coins_count.ToString();
                    if (array.website != null)
                        profile.Web = array.website.ToString();
                    if (array.forum_posts_count != null)
                        profile.Forum = array.forum_posts_count.ToString();
                    if (array.feedback != null)
                    {
                        profile.FeedbackNumber = array.feedback.count.ToString();
                        profile.FeedbackAvg = array.feedback.average.ToString();
                    }
                    if (array.is_collection_visible != null)
                        profile.CollectionVisible = array.is_collection_visible;
                    if (array.is_exchange_coins_visible != null)
                        profile.SwapVisible = array.is_exchange_coins_visible;
                    if (array.image != null)
                        profile.Avatar = array.image.ToString();
                    if (array.languages != null)
                    {
                        foreach (dynamic lang in array["languages"])
                        {
                            cmb_profile_languages.Items.Add(lang.ToString());
                        }
                    }

                    txb_profile_username.Text = profile.Username;
                    txb_profile_title.Text = profile.Title;
                    txb_profile_location.Text = profile.Location;
                    txb_profile_membersince.Text = profile.Member;
                    txb_profile_coinstoswap.Text = profile.Swap;
                    txb_profile_website.Text = profile.Web;
                    txb_profile_forum.Text = profile.Forum;
                    txb_profile_feedbackcount.Text = profile.FeedbackNumber;
                    txb_profile_feedbackavg.Text = profile.FeedbackAvg;

                    pcb_profile_avatar.Load(profile.Avatar);

                    if (profile.CollectionVisible)
                        chb_profile_collectionvisible.Checked = true;
                    else chb_profile_collectionvisible.Checked = false;

                    if (profile.SwapVisible)
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

        // Save coin button handler
        private void btn_savecoin_Click(object sender, EventArgs e)
        {
            try
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Numista|*.num";

                    string fname = (coin.Country + " - " + coin.Title).Replace('/','-');
                    fname = fname.Replace("** ", "").Replace(" **", "").Replace("* ", "").Replace(" *", "");

                    sfd.FileName = fname;
                    string info = "Title: " + coin.Title + Environment.NewLine +
                                 "Country: " + coin.Country + Environment.NewLine +
                                 "Years Range: " + coin.YearsRange + Environment.NewLine +
                                 "Reference number: " + coin.RefNumber + Environment.NewLine +
                                 "Metal: " + coin.Metal + Environment.NewLine +
                                 "Weight: " + coin.Weight + Environment.NewLine +
                                 "Diameter: " + coin.Diameter + Environment.NewLine +
                                 "Thickness: " + coin.Thickness + Environment.NewLine +
                                 "Shape: " + coin.Shape + Environment.NewLine +
                                 "Orientation: " + coin.Orientation+ Environment.NewLine + 
                                 Environment.NewLine +
                                 "Saved: " + Convert.ToString(DateTime.Today.Day + "/" + DateTime.Today.Month + "/" + DateTime.Today.Year) + " " + Convert.ToString(DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(sfd.FileName, nud_coinID.Value.ToString()+Environment.NewLine + Environment.NewLine + info);
                        MessageBox.Show("Coin successfully saved!", "Save coin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("An error has occurred \nTry again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Clear coin history
        private void btnClearHistory_Click(object sender, EventArgs e)
        {
            lsv_history.Items.Clear();
        }

        // Read coin data when the coin is double clicked
        private void lsv_history_DoubleClick(object sender, EventArgs e)
        {
            SearchCoin(lsv_history.SelectedItems[0].SubItems[0].Text);
            nud_coinID.Value = Convert.ToDecimal(lsv_history.SelectedItems[0].SubItems[0].Text);
            tabControl1.SelectedIndex = 1;
        }

        // Open content menu on coin right click in history list
        private void lsv_history_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (lsv_history.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    cms_copydelete.Show(Cursor.Position);
                }
            }
        }

        // Open content menu on right click in history list
        private void lsv_history_MouseDown(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo hit = lsv_history.HitTest(e.Location);
            if(e.Button == MouseButtons.Right)
                if (hit.Location == ListViewHitTestLocations.None)
                    cms_add.Show(Cursor.Position);
        }

        // Adds a .num file to the coin history list.
        // If you select only one file, it will read the data. Otherwise it will jush add them to the list.
        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (var ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Numista|*.num";
                    ofd.Multiselect = true;

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        if (ofd.FileNames.Count() == 1)
                        {
                            string idc = File.ReadLines(ofd.FileName).First();
                            string cnt = File.ReadLines(ofd.FileName).Skip(3).Take(1).First().Substring(9);
                            string titl = File.ReadLines(ofd.FileName).Skip(2).Take(1).First().Substring(7);
                            SearchCoin(idc);
                            tabControl1.SelectedIndex = 1;
                        }
                        else
                        {
                            foreach (String file in ofd.FileNames)
                            {
                                try
                                {
                                    if ((stream = ofd.OpenFile()) != null)
                                    {
                                        using (stream)
                                        {
                                            string idc = File.ReadLines(file).First();
                                            string cnt = File.ReadLines(file).Skip(3).Take(1).First().Substring(9);
                                            string titl = File.ReadLines(file).Skip(2).Take(1).First().Substring(7);

                                            addToCoinList(idc, cnt, titl);
                                        }
                                    }
                                }
                                catch (Exception) { }
                            }
                        }
                    }
                }
            }
            catch(Exception) {}
        }

        // Copy coin id
        private void iDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(lsv_history.FocusedItem != null) {
                Clipboard.SetText(lsv_history.FocusedItem.Text);
            }
        }

        // Copy coin country information
        private void countryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lsv_history.FocusedItem != null)
            {
                string c = lsv_history.FocusedItem.SubItems[1].Text;
                Clipboard.SetText(c);
            }
        }

        // Copy coin title information
        private void titleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lsv_history.FocusedItem != null)
            {
                string c = lsv_history.FocusedItem.SubItems[2].Text;
                Clipboard.SetText(c);
            }
        }

        // Copy all coin information
        private void allToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lsv_history.FocusedItem != null)
            {
                string c = lsv_history.FocusedItem.Text + " - " + lsv_history.FocusedItem.SubItems[1].Text + " - " + lsv_history.FocusedItem.SubItems[2].Text;
                Clipboard.SetText(c);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (lsv_history.FocusedItem != null)
            {
                lsv_history.FocusedItem.Remove();
            }
        }

        // Gets a list of all Numsita countries and issuers
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
                MessageBox.Show(lsb_cntr_countrieslist.Items.Count + " countries and issuers found", "Issuers", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("An error has occurred \nTry again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Copy country list
        private void btn_cntr_copy_Click(object sender, EventArgs e)
        {
            string temp = "";

            foreach (object item in lsb_cntr_countrieslist.Items)
                temp += item.ToString() + "\r\n";

            Clipboard.SetText(temp);
        }

        // Login button handler. Password is not saved anywhere.
        private void btn_log_login_Click(object sender, EventArgs e)
        {
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

        // Logout button handler
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

        /// <summary>
        /// Logs in the user. Password is not saved anywhere.
        /// </summary>
        /// <param name="user">Username</param>
        /// <param name="pass">Password</param>
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

                    if (this.accessToken != "")
                    {
                        btn_log_login.Enabled = false;
                        btn_log_logout.Enabled = true;
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

        /// <summary>
        /// Logs out the user
        /// </summary>
        /// <param name="access_token">User access token fro mthe API</param>
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

        // Gets user messages. Adds them to the list box first, then in list view.
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

                        if(message["is_replied"].ToString() == "True")
                            lsb_log_messages.Items.Add("Yes");
                        else lsb_log_messages.Items.Add("No");
                    }

                    txb_output.Text = formatJson(json);
                }
                addMessagesToList();
            }
            catch(Exception)
            {
                MessageBox.Show("Could not get messages. \nTry again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Adds messages from list box to the list view
        /// </summary>
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

        // Photo links

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

        // Handle items to not be selectable in combobox
        private void cmb_coin_years_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmb_coin_years.SelectedIndex = -1;
        }

        // Handle items to not be selectable in combobox
        private void cmb_profile_languages_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmb_profile_languages.SelectedIndex = -1;
        }

        // Disable history list headers resizing
        private void lsv_history_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = lsv_history.Columns[e.ColumnIndex].Width;
        }

        /// <summary>
        /// Sets a width of a combo box to fit it's items
        /// </summary>
        /// <param name="myCombo"></param>
        /// <returns>width</returns>
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

        // Various links 

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
            System.Diagnostics.Process.Start("https://mihajlonesic.gitlab.io");
        }

        // 'Open in Excel' option from the content menu strip 'cms_add'
        private void openInExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lsv_history.Items.Count > 0)
                ToExcel();
        }

        /// <summary>
        /// Opens a history list to Microsoft Excel
        /// </summary>
        private void ToExcel()
        {
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Microsoft.Office.Interop.Excel.Workbook wb = app.Workbooks.Add(1);
            Microsoft.Office.Interop.Excel.Worksheet ws = (Microsoft.Office.Interop.Excel.Worksheet)wb.Worksheets[1];
            int i = 1;
            int i2 = 1;
            foreach (ListViewItem lvi in lsv_history.Items)
            {
                i = 1;
                foreach (ListViewItem.ListViewSubItem lvs in lvi.SubItems)
                {
                    ws.Cells[i2, i] = lvs.Text;
                    i++;
                }
                i2++;
            }
        }

        /// <summary>
        /// Adds a coin to the history list
        /// </summary>
        /// <param name="id">Numsita ID of the coin</param>
        /// <param name="country">Coin country</param>
        /// <param name="title">Coin title</param>
        private void addToCoinList(string id, string country, string title)
        {
            ListViewItem lvi = new ListViewItem(id + "");
            lvi.SubItems.Add(country);
            lvi.SubItems.Add(title);
            lsv_history.Items.Add(lvi);
        }
    }
 }
