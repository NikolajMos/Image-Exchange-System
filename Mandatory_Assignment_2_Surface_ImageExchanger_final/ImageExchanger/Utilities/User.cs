using System.Windows.Controls;
using System.Windows.Media.Imaging;
namespace Surface.Utilities 
{
    public class User 
    {
        public string name;
        public string ip;
        public string icon;
        public BitmapImage bgr;
        public long tag_value;
        public User(string name,string ip,string icon,BitmapImage bgr,int tag_value)
        {
            this.name = name;
            this.ip = ip;
            this.icon = icon;
            this.bgr = bgr;
            this.tag_value = tag_value;
        }
        public void setid(string name) { this.name = name; }
        public string getid() { return this.name; }
        public void setIP(string ip){this.ip = ip;}
        public string getIP() { return this.ip; }
        public void setIcon(string icon) { this.icon = icon; }
        public string getIcon() { return this.icon; }
        public void setBgr(BitmapImage bgr) { this.bgr = bgr; }
        public BitmapImage getBgr() { return this.bgr; }
        public void setTagValue(long tag_value) { this.tag_value = tag_value; }
        public long getTagValue() { return this.tag_value; }
    }
}
