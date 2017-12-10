using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numista
{
    class Coin
    {
        private String title, country, metal, orientation, shape, yearsRange, refNumber;
        private String obversePhoto, reversePhoto;
        private String diameter, weight, thickness;

        public Coin()
        {
            this.title = "";
            this.country = "";
            this.diameter = "";
            this.weight = "";
            this.metal = "";
            this.orientation = "";
            this.thickness = "";
            this.shape = "";
            this.yearsRange = "";
            this.refNumber = "";
            this.obversePhoto = "https://en.numista.com/catalogue/photos/no-obverse-en.png";
            this.reversePhoto = "https://en.numista.com/catalogue/photos/no-reverse-en.png";

        }

        public Coin(String title, String country, String diameter, String weight, String metal, String orientation, String thickness, String shape, String yearsRange, String refNumber)
        {
            this.title = title;
            this.country = country;
            this.diameter = diameter;
            this.weight = weight;
            this.metal = metal;
            this.orientation = orientation;
            this.thickness = thickness;
            this.shape = shape;
            this.yearsRange = yearsRange;
            this.refNumber = refNumber;
        }

        public String getTitle()
        {
            return this.title;
        }

        public void setTitle(String title)
        {
            this.title = title;
        }

        public String getCountry()
        {
            return this.country;
        }

        public void setCountry(String country)
        {
            this.country = country;
        }

        public String getDiameter()
        {
            return this.diameter;
        }

        public void setDiameter(String diameter)
        {
            this.diameter = diameter;
        }

        public String getWeight()
        {
            return this.weight;
        }

        public void setWeight(String weight)
        {
            this.weight = weight;
        }

        public String getMetal()
        {
            return this.metal;
        }

        public void setMetal(String metal)
        {
            this.metal = metal;
        }

        public String getOrientation()
        {
            return this.orientation;
        }

        public void setOrientation(String orientation)
        {
            this.orientation = orientation;
        }

        public String getThickness()
        {
            return this.thickness;
        }

        public void setThickness(String thickness)
        {
            this.thickness = thickness;
        }

        public String getShape()
        {
            return this.shape;
        }

        public void setShape(String shape)
        {
            this.shape = shape;
        }

        public String getYearsRange()
        {
            return this.yearsRange;
        }

        public void setYearsRange(String yearsRange)
        {
            this.yearsRange = yearsRange;
        }

        public String getRefNumber()
        {
            return this.refNumber;
        }

        public void setRefNumber(String refNumber)
        {
            this.refNumber = refNumber;
        }

        public String getObversePhoto()
        {
            return this.obversePhoto;
        }

        public void setObversePhoto(String obversePhoto)
        {
            this.obversePhoto = obversePhoto;
        }

        public String getReversePhoto()
        {
            return this.reversePhoto;
        }

        public void setReversePhoto(String reversePhoto)
        {
            this.reversePhoto = reversePhoto;
        }
        
        public override String ToString()
        {
            return "";
        }
    }
}
