   M S B A M L   `   `   `  ����I  EImageExchanger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:ImageExchangerImageExchanger  5      #    ImageExchanger.SurfaceWindow1   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      e aMicrosoft.Surface.Presentation, Version=2.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e351s)http://schemas.microsoft.com/surface/2008  5      $rclr-namespace:ImageExchanger  5      -   5          � Title$  ImageExchanger ��5       � WindowState$ Normal=�6<   $	��600��5      $	��800��6   .��5      � -   5         �Name$ ImageExchangeGrid��5      .+�5
   
   ;  5Microsoft.Surface.Presentation.Controls.TagVisualizer  -   5        $��MyTagVisualizer��5
      $��Stretch=�5      $��Stretch=�5      
������ $
��Auto��5      $
��Auto��5         Definitions 5      H  BMicrosoft.Surface.Presentation.Controls.TagVisualizationDefinition  5         Value<  6Microsoft.Surface.Presentation.Input.TagValueConverter$
 0x1b 6/      Source$ CameraVisualization.xaml?�5   /      LostTagTimeout$
 2000Q�5   /      MaxCount$ 3��5   /      OrientationOffsetFromTag$ 0Q�5   /   "	   PhysicalCenterOffsetFromTag$		 2,2-�5   /   
   TagRemovedBehavior$

 Fade=�5   /      UsesTagOrientation . 5   /     5      $
 0xc0 6/   $ CameraVisualization.xaml?�5   /   $
 2000Q�5   /   $ 3��5   /   $ 0Q�5   /   $		 2,2-�5    /   $

 Fade=�5!   /    . 5"   /   .��5&      9  3Microsoft.Surface.Presentation.Controls.ScatterView  -   5        $��scatterView��5&         IsTagVisualizationHost . 
������ $	��750��5*      $
��1500��5+      ��5'      ��  �� Path$ scatterViewItems� � 	Resources 5.       �� DataType   ImageExchanger.Watermark(   "ImageExchanger.CameraVisualization��%�   (��       50   #   *, +)(�� i     53   #   *, +)�� 5/      ScatterviewItemDataTemplate��"  50   #   .R�51      �� _�6!   �� 65   52      �� 53      "  6#   .R�54      �� _�6!   �� 65   55      5M      5N      5O      5P      5S      