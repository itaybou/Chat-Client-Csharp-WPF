   M S B A M L   `   `   `  ����J  FMileStoneClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullk:clr-namespace:MileStoneClient.PresentationLayer.Converters,MileStoneClient.PresentationLayer.Converters  5      U/clr-namespace:MileStoneClient.PresentationLayer!MileStoneClient.PresentationLayer  5      g8clr-namespace:MileStoneClient.PresentationLayer.Controls*MileStoneClient.PresentationLayer.Controls  5      ;    5MileStoneClient.PresentationLayer.Controls.ChatBubble   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Q
converters:clr-namespace:MileStoneClient.PresentationLayer.Converters     5      Hpresentation/clr-namespace:MileStoneClient.PresentationLayer     5      Jlocal8clr-namespace:MileStoneClient.PresentationLayer.Controls     5      $
��Auto��5	      $
��Auto��6-   $	��200��6^   $��5q�6m   ��6:   �� *chatBubbleSize+  � 	Resources  5      J  DMileStoneClient.PresentationLayer.Converters.BooleanToColorConverter   BooleanToColorConverterN  HMileStoneClient.PresentationLayer.Converters.BooleanToAlignmentConverter  BooleanToAlignmentConverterP   JMileStoneClient.PresentationLayer.Converters.AdditionSubstractionConvertor ! AdditionSubstractionConvertor��%W   &	        &	      &	 &       5   
   6-     5   
   61     5   
   63   5      .��5      � $��0 0 0 0q�6   ��6   ��  �� Path$ userMessage� �� 	Converter# [  � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6    �� CornerRadius$ 10��6    �� Padding$ 11q�60   ��6=   �� $ userMessage� �� Mode$ TwoWay=�# [  ._�5      �  5      �� 5      $��* �6$   �� 5      $
��Auto �6$   �� 5      $
��Auto �6$   .+�5       �� $��Center=�6   $
��Left=�67   $��0��6R   $
��Wrap=�6_   
���   �$��SemiBold"�6�   {�. ��6s   �� $ fontSize�# [  �� ConverterParameter$ 1����6  �� *
username+6)  �� 5"      $��Center=�6   $��1��67   $
��Left=�6D   $
��Wrap=�6_   
���   �{�. $��0,3,0,0q�6�   ��6s   �� *
fontSize+��6�   �� *messageBody+6�   �� 5#      $��2��6?   ���	#FF5F5F5F$
��Wrap=�6c   $��Right=�6w    �� Padding$ 0q�6  $��0,7,0,0q�6  ��6   �� $ timeAndGroup���6�   �� $ fontSize�# [ $ -3��6  5$      5%      � 5'   
   $��1��6   	 � ColumnDefinitions	 5(      �� 5)      $��15 �6#   �� 5*      $��* �6#   �� 5+      $��15 �6#   .+�5-      F� $��1��6   
���   �
 K� ZIndex$
 1��6�   $��0 -1 0 0q�6�   +����        р�� ��� � -1    �        	$��0��6�   ��6#   �� $ userMessage�# [ ��6�   �� $ userMessage�$ TwoWay=�# [  5.      5/      50      