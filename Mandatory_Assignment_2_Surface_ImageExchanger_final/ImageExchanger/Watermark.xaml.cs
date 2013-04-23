using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Surface.Utilities;

namespace ImageExchanger
{
    /// <summary>
    /// Interaction logic for Watermark.xaml
    /// </summary>
    public partial class Watermark : UserControl
    {
        //TODO
        
        //getters setters
        //owner opdatering
        //tomt image handling(watermark)
        //constructor add canDrag
        
        public string name;
        public bool canDrag=true;
        public List<User> owners= new List<User>();

        public Watermark(BitmapImage img, string watermarkSource, string name,User owner)
        {
            InitializeComponent();
            
            this.name = name;
            
            if (watermarkSource != null)
            {
                owners.Add(owner);
                canDrag = true;
                image.Source = img;

               addOwner(owner);
            }
                 
        }

        public Watermark()
        {
            InitializeComponent();
        }

        public bool CanDrag
        {
            get { return canDrag; }
        } 

        public object DraggedElement
        {
            get;
            set;
        }

        public void addOwner(User owner)
        {
            Image newOwner = new Image();
            newOwner.Source = new BitmapImage(new Uri(owner.icon));

            owners.Add(owner);
            // int x = owners.Count;
            newOwner.Opacity = 0.8;
            newOwner.Height = 30;
            newOwner.Width = 30;
            InnerGrid.Orientation = Orientation.Vertical;

            InnerGrid.Children.Add(newOwner);
        }
    }
}
