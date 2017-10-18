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
                var json = webClient.DownloadString("http://qmegas.info/numista-api/coin/" + coinID);
                dynamic array = JsonConvert.DeserializeObject(json);

                var title = "";
                var country = "";
                var diameter = "";
                var weight = "";
                var metal = "";
                var orientation = "";
                var thickness = "";
                var yearsR = "";
                var kmNum = "";

                var obverse_photo = "https://en.numista.com/catalogue/photos/no-obverse-en.png";
                var reverse_photo = "https://en.numista.com/catalogue/photos/no-reverse-en.png";

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

                if (array.thickness != null)
                    thickness = array.thickness.ToString();

                if (array.images != null)
                    obverse_photo = array.images.obverse.preview.ToString();
                if (array.images != null)
                    reverse_photo = array.images.reverse.preview.ToString();

                if (array.km != null)
                    kmNum = array["km"][0];

                if (array.years_range != null)
                {
                    yearsR = array.years_range.ToString();
                    foreach (dynamic year in array["years"])
                    {
                        cmb_coin_years.Items.Add(year["year"].ToString());
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
                txb_coin_yearsrange.Text = yearsR;
                txb_coin_refnumber.Text = kmNum;

                pcb_coin_obverse.Load(obverse_photo);
                pcb_coin_reverse.Load(reverse_photo);
            }
        }

        private void searchProfile(string profileID)
        {
            using (var webClient = new WebClient())
            {
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
    }
 }
