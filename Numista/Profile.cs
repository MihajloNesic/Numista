using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numista
{
    class Profile
    {
        private String username, title, location, member, swap, web, forum, feedbackNumber, feedbackAvg, avatar;
        private bool coll, swapColl;

        public Profile()
        {
            this.username = "";
            this.title = "";
            this.location = "";
            this.member = "";
            this.swap = "";
            this.web = "";
            this.forum = "";
            this.coll = false;
            this.swapColl = false;
            this.feedbackNumber = "";
            this.feedbackAvg = "";
            this.avatar = "https://en.numista.com/echanges/avatar.png";
        }

        public String getUsername()
        {
            return this.username;
        }

        public void setUsername(String username)
        {
            this.username = username;
        }

        public String getTitle()
        {
            return this.title;
        }

        public void setTitle(String title)
        {
            this.title = title;
        }

        public String getLocation()
        {
            return this.location;
        }

        public void setLocation(String location)
        {
            this.location = location;
        }

        public String getMember()
        { 
            return this.member;
        }

        public void setMember(String member)
        {
            this.member = member;
        }

        public String getSwap()
        {
            return this.swap;
        }

        public void setSwap(String swap)
        {
            this.swap = swap;
        }

        public String getWeb()
        {
            return this.web;
        }

        public void setWeb(String web)
        {
            this.web = web;
        }

        public String getForum()
        {
            return this.forum;
        }

        public void setForum(String forum)
        {
            this.forum = forum;
        }

        public bool isItColl()
        {
            return this.coll;
        }

        public void setColl(bool state)
        {
            this.coll = state;
        }

        public void setColl(String state)
        {
            this.coll = Boolean.Parse(state);
        }

        public bool isItSwapColl ()
        {
            return this.swapColl;
        }

        public void setSwapColl(bool state)
        {
            this.swapColl = state;
        }

        public void setSwapColl(String state)
        {
            this.swapColl = Boolean.Parse(state);
        }

        public String getFeedbackNumber()
        {
            return this.feedbackNumber;
        }

        public void setFeedbackNumber(String feedbackNumber)
        {
            this.feedbackNumber = feedbackNumber;
        }

        public String getFeedbackAvg()
        {
            return this.feedbackAvg;
        }

        public void setFeedbackAvg(String feedbackAvg)
        {
            this.feedbackAvg = feedbackAvg;
        }

        public String getAvatar()
        {
            return this.avatar;
        }

        public void setAvatar(String avatar)
        {
            this.avatar = avatar;
        }
    }
}
