using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numista
{
    class Coin
    {
        public int Id { get; set; }
        public String Title { get; set; }
        public String Country { get; set; }
        public String Metal { get; set; }
        public String Orientation { get; set; }
        public String Shape { get; set; }
        public String YearsRange { get; set; }
        public String RefNumber { get; set; }
        public String ObversePhoto { get; set; }
        public String ReversePhoto { get; set; }
        public String Diameter { get; set; }
        public String Weight { get; set; }
        public String Thickness { get; set; }
        public bool IsCommemorative { get; set; }
        public String CommemorativeDescription { get; set; }

        public Coin()
        {
            Id = 0;
            IsCommemorative = false;
            ObversePhoto = "https://en.numista.com/catalogue/photos/no-obverse-en.png";
            ReversePhoto = "https://en.numista.com/catalogue/photos/no-reverse-en.png";
        }

        public Coin(int id, String title, String country, String diameter, String weight, String metal, String orientation, String thickness, String shape, String yearsRange, String refNumber) : this()
        {
            Id = id;
            Title = title;
            Country = country;
            Diameter = diameter;
            Weight = weight;
            Metal = metal;
            Orientation = orientation;
            Thickness = thickness;
            Shape = shape;
            YearsRange = yearsRange;
            RefNumber = refNumber;
        }
    }
}
