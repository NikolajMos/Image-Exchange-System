using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ImageExchanger
{
    public class MockUpConnection : IMockUpConnection
    {
        #region Private members
        private Uri uri = new Uri(@"E:\Users\Jette\Pictures\ImageItemImages\Chrysanthemum.jpg");
        private Uri uri1 = new Uri(@"E:\Users\Jette\Pictures\ImageItemImages\Desert.jpg");
        private Uri uri2 = new Uri(@"E:\Users\Jette\Pictures\ImageItemImages\Hydrangeas.jpg");
        private Uri uri3 = new Uri(@"E:\Users\Jette\Pictures\ImageItemImages\Jellyfish.jpg");
        private Uri uri4 = new Uri(@"E:\Users\Jette\Pictures\ImageItemImages\Koala.jpg");
        private Uri uri5 = new Uri(@"E:\Users\Jette\Pictures\ImageItemImages\Lighthouse.jpg");
        private Uri uri6 = new Uri(@"E:\Users\Jette\Pictures\ImageItemImages\Penguins.jpg");
        private Uri uri7 = new Uri(@"E:\Users\Jette\Pictures\ImageItemImages\Tulips.jpg");
        private List<Uri> hardcodedImageUris = new List<Uri>();
        private List<Image> hardcodedImages = new List<Image>();
        #endregion

        public List<Image> SendImages1()
        {
            AddImagesToList1();
            return hardcodedImages;
        }

        public List<Image> SendImages2()
        {
            AddImagesToList2();
            return hardcodedImages;
        }

        public List<Image> SendImages3()
        {
            AddImagesToList3();
            return hardcodedImages;
        }

        private List<Image> AddImagesToList1()
        {
            clearHardcodedImages();

            hardcodedImageUris.Add(uri);
            hardcodedImageUris.Add(uri1);
            hardcodedImageUris.Add(uri2);
            hardcodedImageUris.Add(uri3);

            foreach (Uri u in hardcodedImageUris)
            {
                Image image = new Image();
                image.Source = new BitmapImage(u);
                hardcodedImages.Add(image);
            }
            return hardcodedImages;
        }

        private List<Image> AddImagesToList2()
        {
            clearHardcodedImages();

            hardcodedImageUris.Add(uri4);
            hardcodedImageUris.Add(uri5);
            hardcodedImageUris.Add(uri6);
            hardcodedImageUris.Add(uri7);

            foreach (Uri u in hardcodedImageUris)
            {
                Image image = new Image();
                image.Source = new BitmapImage(u);
                hardcodedImages.Add(image);
            }
            return hardcodedImages;
        }

        private List<Image> AddImagesToList3()
        {
            clearHardcodedImages();

            hardcodedImageUris.Add(uri4);
            hardcodedImageUris.Add(uri5);
            hardcodedImageUris.Add(uri6);
            hardcodedImageUris.Add(uri7);

            foreach (Uri u in hardcodedImageUris)
            {
                Image image = new Image();
                image.Source = new BitmapImage(u);
                hardcodedImages.Add(image);
            }
            return hardcodedImages;
        }

        private void clearHardcodedImages()
        {
            hardcodedImages.Clear();
            hardcodedImageUris.Clear();
        }

    }
}
