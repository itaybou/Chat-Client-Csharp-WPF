   M S B A M L   `   `   `  ����J  FMileStoneClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullk:clr-namespace:MileStoneClient.PresentationLayer.Converters,MileStoneClient.PresentationLayer.Converters  5      U/clr-namespace:MileStoneClient.PresentationLayer!MileStoneClient.PresentationLayer  5      3    -MileStoneClient.PresentationLayer.LoginWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Q
converters:clr-namespace:MileStoneClient.PresentationLayer.Converters     5      Alocal/clr-namespace:MileStoneClient.PresentationLayer     5      -   5        $��284.158��5      $��537.683��60     � Title$)  #Chat-Room - MileStone2 - User login��5	      ���	#FFC2C2C2 � 
ResizeMode$ NoResize=�6P    � WindowStartupLocation$ CenterScreen=�6f    � 	Resources 5      Y   SMileStoneClient.PresentationLayer.Converters.InvertableBooleanToVisibilityConverter '  #InvertableBoolToVisibilityConverter��%   &	          5   
   6<   5      .��5      � -   5         �Name$ loginRegisterDetails��5      $��0,10,3.5,2.5q�6)   $��Stretch=�6?    � ColumnDefinitions 5   
   �� 5      $	��125 �6   �� 5      $��* �6    � RowDefinitions 5   
   �� 5      $��34 �6   �� 5      $��38 �6   �� 5      $��38 �6   �� 5      $��42 �6   �� 5      .+�5   
   �� -   5        $��UserCountText��5      $��1��6)   $��0��69   $
��Left=�6F   $	��Top=�6a   $��5,8,0,0q�6y   $
��Wrap=�6�   $	��287��6�   $��14(�6�   
���   �$��26��6�   �� 5   
   $��1��6   $
��Left=�6    $��63��6;   $��	328,4,0,0q�6G   $	��Top=�6Z   $��70��6r   $+��%../Resources/Images/chatroom_logo.png��6}    �� BitmapScalingMode$ HighQuality=�6�   $��2��6�   �� 5   
   $��0��6   $��1��6    $
��Left=�6-   $��16��6H   $��	48,10,0,0q�6T   $	��Top=�6g   $��16��6   $&�� ../Resources/Icons/user_icon.png��6�   $ HighQuality=�6�   �� 5   
   $��
_Username:��6   $��0��6%   $��1��65   $
��Left=�6B   $��24��6]   $	��Top=�6i   $��66��6�   $��61,5,0,0q�6�   
���   � J� RenderTransformOrigin$ 1.141,0.607&�6�   $��2��6�   �� 5    
   -   5        $��UsernameInputTextBox��5       	 �� 	MaxLength$	 15��6z   $��1��6�   $��1��6�   $
��Left=�6�   $��24��6�   $��5,6,0,0q�6�   
 �� TextWrapping$

 Wrap=�6�   $	��Top=�6�   $	��287��6
  
�������
���   ���6.   ��  �� Path$ _currentUsername� �� UpdateSourceTrigger$ PropertyChanged=�6)  �� 5!   
   $��0��6   $��2��6    $
��Left=�6-   $��14��6H   $��	48,12,0,0q�6T   $	��Top=�6g   $��14��6   $*��$../Resources/Icons/password_icon.png��6�   $ 	0.5,0.567&�6�   $ HighQuality=�6�   �� 5"   
   $��
_Password:��6   $��2��6%   $��0��62   $
��Left=�6B   $��28��6]   $	��Top=�6i   $��66��6�   $��62,5,0,0q�6�   
���   �$��2��6�   G� 5#   
   -   5        $��UserPasswordInputTextBox��5#       G� 	MaxLength$ 15��6X   $
��Left=�6g   $��24��6�   $��5,6,0,0q�6�   $	��Top=�6�   $	��287��6�   
�������
���   �$��1��6�   $��2��6�   �� 5$   
   $��0��6   $��3��6    $
��Left=�6-   $��15��6H   $��	47,11,0,0q�6T   $	��Top=�6g   $��15��6   $'��!../Resources/Icons/group_icon.png��6�   $ 	0.5,0.567&�6�   $ HighQuality=�6�   �� 5%   
   $��
_Group ID:��6   $��3��6%   $��0��62   $
��Left=�6B   $��28��6]   $	��Top=�6i   $��66��6�   $��63,5,0,0q�6�   
���   �$��2��6�   �� 5&   
   -   5        $��GroupInputTextBox��5&      
 � Tag$ Enter group ID��6+   $	 3��6�   $��1��6�   $��3��6�   $
��Left=�6�   $��24��6  $��5,6,0,0q�6  $

 Wrap=�6/  $	��Top=�6C  $	��287��6[  
�������
���   �{�6@   ��  �� ElementName$ Check_Box_Registration��$ 	IsChecked���6�   �� $ groupID�$ PropertyChanged=�6�  �� 5(   
   $��Choose Action:��6   $��0��6)   $��4��69   $
��Left=�6F   $��24��6a   $	��Top=�6m   $	��115��6�   $��5,1,0,0q�6�   
���   �$ 1.141,0.607&�6�   $
��Bold"�6�   &   System.Windows.Media.TextOptions   TextHintingMode$ Fixed=�6�   ��5)      
� 5*      .��5+      �  � Color$ 	#FFECECEC��6#   � 5,      $ 	#FFC2C2C0��6#    � Offset$ 1Q�65   5-      5/      �� 50   
   -   5        $��Check_Box_Registration��50      $ Registration��61   $��Registration��6D   $
��Left=�6[   $��4��6v   $��0��6�   $	��Top=�6�   $��14(�6�   $��SemiBold"�6�   $��	10,52,0,0q�6�   $��18��6�   $	��105��6	  6  �� 51   
   -   5        $��Check_Box_Login��51      $ Login��6*    d� 	IsChecked$
 trueY�66   $��Login��6G   $
��Left=�6W   $��	10,27,0,0q�6r   $��4��6�   $��0��6�   $	��Top=�6�   $��14(�6�   $��SemiBold"�6�   $��18��6�   $	��105��6  6  �� 52   
   -	   5        $��LoginButton��52      $��Login��6$   $��4��6�   $
��Left=�6�   $��40��6�   $��	53,18,0,0q�6  $	��Top=�6  $	��194��61  $��1��6=  ���	#FF6E6E6E$��1q�6e  
���   �$��SemiBold"�6�  y�64   �� $ Check_Box_Login��$ 	IsChecked� �� 	Converter# [   �� ConverterParameter$ Normal����53      
� 54      .��55      � $ 	#FF3F893D��6#   $ 1Q�65   � 56      $ 	#FF00B75B��6#   57      59      �� 5:   
   -
   5        $��RegisterButton��5:      $��Register��6'   $
��Left=�6�   $��40��6�   $��	53,18,0,0q�6  $��4��6  $	��Top=�6&  $	��194��6>  $��1��6J  ���	#FF6E6E6E$��1q�6r  
���   �$��SemiBold"�6�  y�6:   �� $ Check_Box_Registration��$ 	IsChecked�# [  $ Normal����5;      
� 5<      .��5=      � $ 	#FF5590B8��6#   $ 1Q�65   � 5>      $ 	#FF93C0EF��6#   5?      5A      �� 5B   
   -   5        $��1��5B   $   $��Right=�64   $��34��6P   $��	0,0,10,10q�6\   $��4��6o   $��Bottom=�6|   $��42��6�   ��6�   Z� ��5C      �� 5D       �� ImageSource$% ../Resources/Icons/shutdown.png��6    j� Stretch$ Uniform=�6K   $ HighQuality=�6]   5F      5G      5H      