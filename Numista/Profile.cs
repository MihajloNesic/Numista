using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numista
{
    class Profile
    {
        public String Username { get; set; }
        public String Title { get; set; }
        public String Location { get; set; }
        public String Member { get; set; }
        public String Swap { get; set; }
        public String Web { get; set; }
        public String Forum { get; set; }
        public String FeedbackNumber { get; set; }
        public String FeedbackAvg { get; set; }
        public String Avatar { get; set; }
        public bool CollectionVisible { get; set; }
        public bool SwapVisible { get; set; }

        public Profile()
        {
            CollectionVisible = false;
            SwapVisible = false;
            Avatar = "https://en.numista.com/echanges/avatar.png";
        }
    }
}
