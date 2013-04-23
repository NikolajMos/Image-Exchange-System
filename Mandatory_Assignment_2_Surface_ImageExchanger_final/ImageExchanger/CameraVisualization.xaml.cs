using System;
using System.Collections.Generic;
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
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;

namespace ImageExchanger
{
    /// <summary>
    /// Interaction logic for CameraVisualization.xaml
    /// </summary>
    public partial class CameraVisualization : TagVisualization
    {
        #region private Members
        private bool isPinned = false;
        #endregion 

        public event RoutedEventHandler PinButtonClicked;

        public CameraVisualization()
        {
            InitializeComponent();
        }

        private void CameraVisualization_Loaded(object sender, RoutedEventArgs e)
        {
            //TODO: customize CameraVisualization's UI based on this.VisualizedTag here
        }


        #region Buttons
        private void ButtonPin_Click(object sender, RoutedEventArgs e)
        {
            //TagRemovedBehavior = TagRemovedBehavior.Persist;
            ButtonPin.Visibility = Visibility.Hidden;
            isPinned = true;
            PinButtonClicked(this, e);
        }

        private void ButtonPin_TouchUp(object sender, TouchEventArgs e)
        {
           // TagRemovedBehavior = TagRemovedBehavior.Persist;
            ButtonPin.Visibility = Visibility.Hidden;           
            isPinned = true;
            PinButtonClicked(this, e);
        }
   
        #endregion


        #region Properties
        public bool IsPinned
        {
            get { return isPinned; }
            set { isPinned = value; }
        }
        #endregion

        
    }
}
