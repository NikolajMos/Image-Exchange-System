using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using System.Diagnostics;
using System.Windows.Input;
using System.Threading;
using Surface;
using Surface.Utilities;
using System.IO;
using ImageExchanger.Utilities;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Net;

namespace ImageExchanger
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {

        #region private members
        private long tagValue;
        private Style style;
        private Point pinnedPoint = new Point(0.0, 0.0);
        private FrameworkElement elementAlreadyInMap;
        private CameraVisualization camera;
        private Dictionary<long, CameraVisualization> taggedPhones = new Dictionary<long, CameraVisualization>();
        private List<Watermark> imagesFromPhone = new List<Watermark>();
        private IMockUpConnection mockUpConnection = new MockUpConnection();
        private Dictionary<long, FrameworkElement> pinnedPhones = new Dictionary<long, FrameworkElement>();
        private ObservableCollection<FrameworkElement> scatterViewItems = new ObservableCollection<FrameworkElement>();
 
        public static List<User> users=new List<User>();
        private ObservableCollection<ScatterViewItem> targetItems = new ObservableCollection<ScatterViewItem>();
        public delegate void MyDelegate(ImageCollection weps);
        public delegate void MyDelegateWatermark(BitmapImage image, string watermark, string file_name);
        public delegate void WaterMarkDelegate(string android_ip, string file_name);
        private string imagesRequest;
        private const int android1_tag = 27;
        private const int android2_tag = 192;
        private ScatterViewItem draggedElement;
        
        #endregion 

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindow1()
        {
            InitializeComponent();

           
            BitmapImage img1_bgr = new BitmapImage();
            string icon1 = "pack://application:,,,/Images/user2.png";
            //scatterView.Items.Add(img1_bgr);

            img1_bgr = new BitmapImage(new Uri("pack://application:,,,/Images/ray.png"));
            users.Add(new User("Sony Xperia Ray", "192.168.1.4", icon1, img1_bgr, android1_tag));

            BitmapImage img2_bgr = new BitmapImage();
            string icon2 = "pack://application:,,,/Images/user1.png";
            img2_bgr = new BitmapImage(new Uri("pack://application:,,,/Images/ideos.png"));
            users.Add(new User("HUAWEI Ideos", "192.168.1.5", icon2, img2_bgr, android2_tag));

            SendImages si = new SendImages();
            si.ip_target = Util.localIPAddress();
            si.method = "SendImages";
            imagesRequest = JsonConvert.SerializeObject(si).ToString();


            Thread t1 = new Thread(() => server());
            t1.Start();
            Thread t2 = new Thread(() => comm_server());
            t2.Start();
            // Add handlers for window availability events
           AddWindowAvailabilityHandlers();
           scatterView.ItemsSource = scatterViewItems;
       //   DataContext = this;
    
        }
        public void server()
        {
            
            /*string text = System.IO.File.ReadAllText(@"C:\Users\Nikolaj\Desktop\Mandatory_Assignment_2_Surface_ImageExchanger_fra jacob\ImageExchanger\Resources\test.txt");
            imgJson(text);*/
            
            
            TcpListener server = null;
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 15124;
                //  IPAddress localAddr = IPAddress.Parse(IPAddress.Any);

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(IPAddress.Any, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[500000];

                // Enter the listening loop. 
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests. 
                    // You could also user server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();

                    //if (server.AcceptTcpClient()) 
                    Console.WriteLine("Connected!");
                    Thread t3 = new Thread(() => getData(client, bytes));
                    t3.Start();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }


            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }
        }
        public void comm_server()
        {
            TcpListener server = null;
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 15126;
                //  IPAddress localAddr = IPAddress.Parse(IPAddress.Any);

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(IPAddress.Any, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[500000];

                // Enter the listening loop. 
                while (true)
                {
                    Console.Write("Waiting for a Message connection... ");

                    // Perform a blocking call to accept requests. 
                    // You could also user server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();

                    //if (server.AcceptTcpClient()) 
                    Console.WriteLine("Connected!");
                    Thread t3 = new Thread(() => getMessage(client, bytes));
                    t3.Start();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }


            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }
        }
        private void getMessage(TcpClient client, Byte[] bytes)
        {
            NetworkStream stream = client.GetStream();

            // Loop to receive all the data sent by the client. 
            bytes = Util.ReadFully(stream);
            string result = System.Text.Encoding.UTF8.GetString(bytes);
            InMsg inMsg = JsonConvert.DeserializeObject<InMsg>(result);
            if (inMsg.method == "transfer_ok") 
            { 
                Console.WriteLine("from ip:"+inMsg.android_ip+", file "+inMsg.file_name+" was recieved succesfully");

                WaterMarkDelegate del = new WaterMarkDelegate(this.updateWaterMark);
                            
                //Dispatcher UserDispatcher =  Dispatcher.CurrentDispatcher;
                this.Dispatcher.Invoke(del, new object[] { inMsg.android_ip, inMsg.file_name });
                //updateWaterMark(inMsg.android_ip, inMsg.file_name); 
            }

 
        }

        public void updateWaterMark(string android_ip, string file_name)
        {
            User user = null;
            for (int i = 0; i < users.Count;i++ )
            {
                if (users.ElementAt(i).ip == android_ip)
                {
                    user = users.ElementAt(i);
                }
            }
            if (user != null)
            {
                for (int i = 0; i < scatterViewItems.Count; i++)
                {
                    if (scatterViewItems.ElementAt(i).GetType() == new Watermark(null, null, null, null).GetType())
                    {
                        
                        Watermark tmp = (Watermark)scatterViewItems.ElementAt(i);
                        //Console.WriteLine(tmp.name + "==" + file_name);
                        if (tmp.name == file_name)
                        {
                            bool ownerExist = false;
                            for (int j = 0; j < tmp.owners.Count; j++)
                            {
                                
                                if (tmp.owners.ElementAt(j).ip==user.ip)
                                {
                                    Console.WriteLine(tmp.owners.ElementAt(j).ip + "==" + user.ip+ "is true!");
                                    ownerExist = true;
                                }
                            }
                            if (!ownerExist)
                            {
                                Console.WriteLine("But I did it Anyway!");
                                tmp.addOwner(user); }
                        }
                        
                    }

                }
            }
        }

        static void client(int port,String target_ip, String json)
        {
            TcpClient clientSocket = new TcpClient();
            clientSocket.Connect(target_ip, port);
            NetworkStream clientStream = clientSocket.GetStream();
            byte[] outStream = new System.Text.UTF8Encoding(true).GetBytes(json);
            clientStream.Write(outStream, 0, outStream.Length);
            clientStream.Flush();
            clientSocket.Close();
        }
        public void getData(TcpClient client, Byte[] bytes)
        {
            // Get a stream object for reading and writing
            NetworkStream stream = client.GetStream();

            // Loop to receive all the data sent by the client. 
            bytes = Util.ReadFully(stream);
            //imageCollection = System.Text.Encoding.UTF8.GetString(bytes);
            //Console.WriteLine("INPUT: "+imageCollection);

            //imgJson(System.Text.Encoding.UTF8.GetString(bytes));

            
           /*using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\test\test.txt", true))
            {
                string result = System.Text.Encoding.UTF8.GetString(bytes);
                file.WriteLine(result);
            }*/
            imgJson(System.Text.Encoding.UTF8.GetString(bytes));
            // Shutdown and end connection
            client.Close();
        }
        
        private void SurfaceWindow_Loaded(object sender, RoutedEventArgs e)
        {
            style = new Style(typeof(ContentControl));
            Setter setter = new Setter();
            setter.Property = ContentControl.OpacityProperty;
            setter.Value = 0.5;

            style.Setters.Add(setter);

            Trigger t = new Trigger();
            t.Property = TagProperty;
            t.Value = "DragEnter";
                setter.Property = OpacityProperty;
                setter.Value = 1.0;

            t.Setters.Add(setter);
            style.Triggers.Add(t);
        }
        
        public void dragDrop(object sender, InputEventArgs e)
        {
            Point phoneUpperLeft = new Point();
            Point phoneUpperRight = new Point();
            Point phoneLowerRight = new Point();
            Point imageDragged = new Point();

            double phoneHeight = 0;
            double phoneWidth = 0;
            ScatterViewItem phone = null;
            FrameworkElement findSource = e.OriginalSource as FrameworkElement;
            ScatterViewItem draggedElement = null;
            while (draggedElement == null && findSource != null)
            {
                if ((draggedElement = findSource as ScatterViewItem) == null)
                {
                    findSource = VisualTreeHelper.GetParent(findSource) as FrameworkElement;
                }
            }
            Watermark water = new Watermark();
            if (draggedElement.DataContext as Watermark != null)
            {
                water = draggedElement.DataContext as Watermark;
            }
            List<ScatterViewItem> phones = new List<ScatterViewItem>();

            if (water != null && draggedElement.DataContext as Watermark != null)
            {
                imageDragged = water.TranslatePoint(new Point(0, 0), this);
               
                for (int i = 0; i < scatterViewItems.Count; i++)
                {

                    if (scatterViewItems.ElementAt(i).GetType() == new ScatterViewItem().GetType())
                    {
                        phone = (ScatterViewItem)scatterViewItems.ElementAt(i);
                        phones.Add(phone);
                        
                    }
                }

                if (phones.Count() != 0)
                {
                    bool hasCollision = false;
                    //long collisionPhone = 0;
                    ScatterViewItem tmpPhone = null;
                    for (int i = 0; i < phones.Count(); i++)
                    {
                        phoneHeight = phones.ElementAt(i).ActualHeight;
                        phoneWidth = phones.ElementAt(i).ActualWidth;
                        phoneUpperLeft = phones.ElementAt(i).TranslatePoint(new Point(0, 0), this);
                        phoneUpperRight = phones.ElementAt(i).TranslatePoint(new Point(phoneWidth, 0), this);
                        phoneLowerRight = phones.ElementAt(i).TranslatePoint(new Point(phoneWidth, phoneHeight), this);
                        //Console.WriteLine("TAG: "+phone.Tag);
                        hasCollision= collision(imageDragged, phoneUpperLeft, phoneUpperRight, phoneLowerRight);
                        if (hasCollision)
                        {
                            tmpPhone = phones.ElementAt(i);
                            break;
                        }
                        //collisionPhone = (long)phones.ElementAt(i).Tag;
                    }
                    if (tmpPhone != null)
                    {
                        Console.WriteLine("TAG: " + tmpPhone.Tag);
                        if (hasCollision == true)
                        {
                            Console.WriteLine("COLLISION!!");
                            requestTransfer((long)tmpPhone.Tag, water);
                        }
                        else Console.WriteLine("NO COLLISION!");
                    }
                }
            }
        }
        public void requestTransfer(long tag, Watermark watermark)
        {       
             //t2 Imitere at tlf er lagt på bordet:(kommer fra TAGVisualizer object)
                    OutMsg outMsg = new OutMsg();
                    User dest_user = null;
                    for (int i = 0; i < users.Count; i++)
                    {
                        if (users.ElementAt(i).tag_value == tag)
                        {
                            dest_user = users.ElementAt(i);
                        }
                    }
                    bool userHasImage = false;
                    for (int i = 0; i < watermark.owners.Count; i++)
                    {
                        if (watermark.owners.ElementAt(i).ip == dest_user.ip)
                        {
                            userHasImage = true;
                        }
                    }
                    if (userHasImage) { Console.WriteLine("The phone already ownes this picture!"); }
                        if (dest_user != null && !userHasImage)
                        {
                            outMsg.dest_ip = dest_user.ip;//Telefonen der er collission med's ips
                            outMsg.file_name = watermark.name;
                            outMsg.method = "transfer";
                            string src_ip = watermark.owners.First().ip;

                            if (src_ip != null)
                            {

                                if (src_ip != dest_user.ip)
                                {
                                    Thread t2 = new Thread(() => client(15123, src_ip, JsonConvert.SerializeObject(outMsg)));
                                    //Console.WriteLine(src_ip + ": " + JsonConvert.SerializeObject(outMsg));
                                    t2.Start();
                                }
                                else Console.WriteLine("The phone already ownes this picture!");
                            }
                        }

                
        }
        public bool collision(Point imageDragged, Point phoneUpperLeft, Point phoneUpperRight, Point phoneLowerRight)
        {
            //     Console.WriteLine("START:\n"+imageCoordinates.X+" >= "+phoneUpperLeft+" &&\n"+imageCoordinates.X+" <= "+phoneUpperLeft.X+"&&\n"+imageCoordinates.Y+" >= "+phoneLowerRight+"&&\n"+imageCoordinates.Y+" <= "+phoneUpperRight);
            if (imageDragged.X >= phoneUpperLeft.X && imageDragged.X <= phoneUpperRight.X && imageDragged.Y <= phoneLowerRight.Y && imageDragged.Y >= phoneUpperRight.Y)
            {
                
                return true;
            }
            else 
            { 
                
                return false; 
            }
        }

        #region Events
        private void OnVisualizationAdded(object sender, TagVisualizerEventArgs e)
        {
            //t2 Imitere at tlf er lagt på bordet:(kommer fra TAGVisualizer object)
            camera = (CameraVisualization)e.TagVisualization;
            tagValue = camera.VisualizedTag.Value;
            elementAlreadyInMap = null;

            User user = null;
            for (int i = 0; i < users.Count; i++)
            {
                if (users.ElementAt(i).tag_value == tagValue)
                {
                    user = users.ElementAt(i);
                }
            }
            if (user != null)
            {
                Thread t2 = new Thread(() => client(15123,user.ip, imagesRequest));
                Console.WriteLine(imagesRequest);
                t2.Start();
            }

            if (pinnedPhones.TryGetValue(tagValue, out elementAlreadyInMap))
            {
                for (int i = scatterViewItems.Count -1 ; i >= 0; i--)
                {
                    if (scatterViewItems.ElementAt(i).GetType().Equals(new ScatterViewItem().GetType()))
                    {
                        //elementAlreadyInMap.Tag = tagValue;
                        if ((long)scatterViewItems.ElementAt(i).Tag == tagValue)
                        { 
                            scatterViewItems.RemoveAt(i);
                        }
                        
                    }
                    else if(scatterViewItems.ElementAt(i).Name.Equals("OwnerId" + tagValue.ToString()))
                    {
                        scatterViewItems.RemoveAt(i);
                    }
                }
                pinnedPhones.Remove(tagValue);
            }

            camera.PinButtonClicked += new RoutedEventHandler(PinButtonClicked);
           
            // Add a handler for the GotTag event
            camera.GotTag += new RoutedEventHandler(OnGotTag);

            // Add a handler for the LostTag event
            camera.LostTag += new RoutedEventHandler(OnLostTag);

            elementAlreadyInMap = null;
            // Checks if visualisation is removed but pinned, if not a new visualisation is created
            if (!pinnedPhones.TryGetValue(tagValue, out elementAlreadyInMap))
            {
                Console.WriteLine("TAGVALUE: " + tagValue);
                 switch (tagValue)
                {
                    case android1_tag:
                        camera.myImage.Source= users.ElementAt(0).bgr; //new BitmapImage(new Uri("pack://application:,,,/Images/Nexus.jpg"));
                        camera.LabelTagValue.Content = tagValue;
                        break;
                    case android2_tag:
                        camera.myImage.Source = users.ElementAt(1).bgr;
                        camera.LabelTagValue.Content = tagValue;
                        break;
                    /*case 3:
                        camera.myImage.Source = phone3;
                        camera.LabelTagValue.Content = tagValue;
                        break;*/
                    default:
                     //   camera.myRectangle.Fill = SurfaceColors.Accent4Brush;
                        break;
                }
            camera.Tag = camera.Effect;
            }
        }

        private void OnGotTag(object sender, RoutedEventArgs e)
        {
           //taggedPhones.Add(tagValue, camera);
        }
        private void OnLostTag(object sender, RoutedEventArgs e)
        {

            //CameraVisualization camera = e.Source as CameraVisualization;
        }

        private void PinButtonClicked(object sender, RoutedEventArgs e)
        {
            camera = sender as CameraVisualization;
            tagValue = camera.VisualizedTag.Value;

            FrameworkElement frameworkElement = camera;
            pinnedPoint = camera.TranslatePoint(new Point(0.0, 0.0), null);

            elementAlreadyInMap = null;
            if (!pinnedPhones.TryGetValue(tagValue, out elementAlreadyInMap))
            {
               
                pinnedPhones.Add(tagValue, camera);
                AddPhoneToScatterView(tagValue, camera);
                camera.Visualizer.RemoveVisualization(camera);
                //RetreiveImagesFromPhone();
                AddImagesToScatterView();
            }
            else
	        {
                MessageBox.Show("The phone has already been pinned");
	        }
        }

        private void AddPhoneToScatterView(long tagValue, CameraVisualization camera)
        {
            ScatterViewItem phone = new ScatterViewItem();
            phone.Tag = tagValue;
            FrameworkElement frameworkElement = new FrameworkElement();
            if (pinnedPhones.TryGetValue(tagValue, out frameworkElement))
            {
                Image myPhoneImage = new Image();
                if (tagValue == android1_tag)
                    myPhoneImage.Source = users.ElementAt(0).bgr;
                if (tagValue == android2_tag)
                    myPhoneImage.Source = users.ElementAt(1).bgr;
                /*
                if (tagValue == 3)
                    myPhoneImage.Source = phone3;*/

                phone.Content = myPhoneImage;
                phone.Height = frameworkElement.ActualHeight;
                phone.Width = frameworkElement.ActualWidth;
                phone.Orientation = 0;
                phone.Center = pinnedPoint; // Original position of phone
                phone.Name = "PhoneId" + tagValue.ToString() ; // set the name property
                phone.CanScale = false;
                phone.CanRotate = true;
                phone.AllowDrop = true;
                /*
                SurfaceDragDrop.AddDragEnterHandler(phone, DropTargetDragEnter);
                SurfaceDragDrop.AddDragLeaveHandler(phone, DropTargetDragLeave);
                SurfaceDragDrop.AddDropHandler(phone, DropTargetDrop);*/
                scatterViewItems.Add(phone);
       
            }
        }


        private void AddImagesToScatterView()
        {
            try
            {
                foreach (Watermark image in imagesFromPhone.ToList())
                {
                    
                    scatterViewItems.Add(image);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }
        public void scatterViewControle(ImageCollection weps)
        {
            // MyDelegateWatermark del = new MyDelegateWatermark(this.WatermarkControl);
            ////OPTIMIZE!!!! n3!!!
            User tmp_user = null;
            for (int i = 0; i < users.Count; i++)
            {
                
                if (users.ElementAt(i).ip == weps.src_ip)
                {
                    tmp_user = users.ElementAt(i);
                    break;
                }
            }
            if (tmp_user != null)
            {
                for (int i = 0; i < weps.images.Count; i++)
                {/*
                    User user=null;
                    for (int j = 0; j < users.Count; j++)
                    {
                        Trace.WriteLine("name: " + users.ElementAt(i).ip + "name: " + weps.src_ip);
                        if (users.ElementAt(i).ip == weps.src_ip)
                        {
                            user = users.ElementAt(i);
                            Trace.WriteLine("User found!");
                            break;
                        }
                    }
                    Trace.WriteLine(user==null);*/
                    Watermark tmp = new Watermark(Util.LoadImage(weps.images.ElementAt(i).image.bytes), tmp_user.icon, weps.images.ElementAt(i).image.name, tmp_user);

                    tmp.Tag = weps.images.ElementAt(i).image.name;
                    
                   
                    
                    bool copy = true;
                    for (int k = 0; k < scatterViewItems.Count; k++)
                    {
                        if (scatterViewItems.ElementAt(k).GetType() == tmp.GetType())
                        {
                            Watermark w = (Watermark)scatterViewItems.ElementAt(k);
                            if (w.name == tmp.name) { copy = false; }//it already exist!
                            //Trace.WriteLine("name: " + w.name + "name: " + tmp.name);
                        }
                    }
                    if (copy == true) scatterViewItems.Add(tmp);
                }
                //this.Dispatcher.Invoke(del, new object[] { image, watermark });
            }
            else Console.WriteLine("unregistred user, please contact admin.");
        }
        public void imgJson(string json)
        {


            ImageCollection weps = JsonConvert.DeserializeObject<ImageCollection>(json);
            //Trace.Write(weps.images.First().image.bytes);

            MyDelegate del = new MyDelegate(this.scatterViewControle);
            //Dispatcher UserDispatcher = Dispatcher.CurrentDispatcher;
            this.Dispatcher.Invoke(del, new object[] { weps });

        }
        public void WatermarkControl(BitmapImage image, string watermark, string file_name,User owner)
        {
            Watermark uc = new Watermark(image, watermark, file_name,owner);
            //myScatterView.Items.Add(uc);
        }

        
        public class ImageCollection
        {
            public string src_ip { get; set; }
            public List<ImageLists> images { get; set; }
        }
        public class ImageLists
        {
            public ImageDetails image { get; set; }
        }
        public class ImageDetails
        {
            public string bytes { get; set; }
            public string name { get; set; }
        }
        public class InMsg
        {
            public string android_ip { get; set; }
            public string method { get; set; }
            public string file_name { get; set; }
        }
        public class OutMsg
        {
            public string dest_ip { get; set; }
            public string method { get; set; }
            public string file_name { get; set; }
        }
        private void VisualizationRemoved(object sender, TagVisualizerEventArgs e)
        {
            
        }

        /// <summary>
        /// This is only a temporary method should be replaced by threads and an RPC call
        /// </summary>
        private void RetreiveImagesFromPhone()
        {
            /*
            if(tagValue == 1)
            imagesFromPhone = mockUpConnection.SendImages1();
            
            if(tagValue == 2)
            imagesFromPhone = mockUpConnection.SendImages2();
            
            if (tagValue == 3)
            imagesFromPhone = mockUpConnection.SendImages3();
            */

        }
        #endregion 

        #region Autogenerated code
        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Remove handlers for window availability events
            RemoveWindowAvailabilityHandlers();
        }

        /// <summary>
        /// Adds handlers for window availability events.
        /// </summary>
        private void AddWindowAvailabilityHandlers()
        {
            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;
        }

        /// <summary>
        /// Removes handlers for window availability events.
        /// </summary>
        private void RemoveWindowAvailabilityHandlers()
        {
            // Unsubscribe from surface window availability events
            ApplicationServices.WindowInteractive -= OnWindowInteractive;
            ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
        }

        /// <summary>
        /// This is called when the user can interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowInteractive(object sender, EventArgs e)
        {
            //TODO: enable audio, animations here
        }

        /// <summary>
        /// This is called when the user can see but not interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: optionally enable animations here
        }

        /// <summary>
        /// This is called when the application's window is not visible or interactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowUnavailable(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here
        }
        #endregion
        public class SendImages
        {
            public string method { get; set; }
            public string ip_target { get; set; }
        }
  
    }
}