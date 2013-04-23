using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Surface.Presentation.Controls;

namespace ImageExchanger
{
    public class ImageItem
    {

        #region private members
        private ScatterViewItem svi;
        private string imageName;
        private string owner;
        private string[] ownerArray;
        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public ImageItem()
        {

        }

        /// <summary>
        /// Constructer with scatterViewItem, imageName and owner
        /// </summary>
        public ImageItem(ScatterViewItem svi, string imageName, string owner)
        {
            this.svi = svi;
            this.imageName = imageName;
            this.owner = owner;
            string[] ownerArray = new string[3];

            ownerArray[0] = owner;
        }



        #region properties
        public ScatterViewItem Svi
        {
            get { return svi; }
            set { svi = value; }
        }

        public string ImageName
        {
            get { return imageName; }
            set { imageName = value; }
        }

        public string Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        public string[] OwnerArray
        {
            get { return ownerArray; }
            set { ownerArray = value; }
        }

        #endregion
    }
    
}
