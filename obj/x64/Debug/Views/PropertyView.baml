   M S B A M L   `   `   `  ����c  _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity  5      C ?Sculptor, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:SculptorSculptor 5      #clr-namespace:SculptorSculptor 5         Sculptor.PropertyView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      %vmclr-namespace:Sculptor     5      (localclr-namespace:Sculptor     5      p lTelerik.Windows.Controls.Navigation, Version=2019.1.116.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7p lTelerik.Windows.Controls.RibbonView, Version=2019.1.116.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7k gTelerik.Windows.Documents.Core, Version=2019.1.114.40, Culture=neutral, PublicKeyToken=5803cfa389c90ce7~	 zTelerik.Windows.Documents.FormatProviders.OpenXml, Version=2019.1.116.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7k
 gTelerik.Windows.Controls.Input, Version=2019.1.116.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7q mTelerik.Windows.PersistenceFramework, Version=2019.1.116.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7n jTelerik.Windows.Controls.GridView, Version=2019.1.116.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7z vTelerik.Windows.Documents.FormatProviders.Rtf, Version=2019.1.116.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7a ]Telerik.Windows.Data, Version=2019.1.116.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7e aTelerik.Windows.Controls, Version=2019.1.116.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7f bTelerik.Windows.Documents, Version=2019.1.116.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7s oTelerik.Windows.Controls.RichTextBoxUI, Version=2019.1.116.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7{ wTelerik.Windows.Documents.FormatProviders.Xaml, Version=2019.1.116.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7Wtelerik1http://schemas.telerik.com/2008/xaml/presentation    	 
         5	        � 	Resources  5        Sculptor.Type_IDToImage   type_IDToImage  aspectCBEditorStyle �� 
TargetType5  /Telerik.Windows.Controls.GridViewComboBoxColumn �� Property �� Value � ColumnDefinitions��%�  &	        &	        5   
   6   �� 5   
   "  6,   .�5      ��  �@ ItemTemplate 5      �� 5      .R�5      �  5      �� 5   "   $
��Auto �63   �� 5   "   $
��Auto �63   �� 5   "   $
��Auto �63   .+�5      �� $��0��6D   ��6(   �� *
AspectName+6D   �� 5      $	�� - ��6(   $��1��63   �� 5      $��2��6F   ��6(   �� *Description+6F   5      5      5!      5"      5#      .��5%      �  5&   
   �� 5'      $��2* �6   �� 5(      $��2* �6   �� 5)      $��* �6   .+�5,   
   ,  &Telerik.Windows.Controls.RadTabControl  $��0��6    .@�5-      )  #Telerik.Windows.Controls.RadTabItem  $��25��6!   $��80��6-   $��10,0,0,0q�6;   $��
Properties��6M   .��5.      .  (Telerik.Windows.Controls.RadTreeListView  -   5          Name$ PropertyTreeListView��5.   *   ;  5Telerik.Windows.Controls.GridView.GridViewDataControl   SelectionMode$ Extended=�53   )      CanUserDeleteRows .    AutoGenerateColumns .  $	��300��56      	   CanUserFreezeColumns	 .  
   RowIndicatorVisibility$
 	Collapsed=�58         ColumnWidth?  9Telerik.Windows.Controls.GridView.GridViewLengthConverter$ * 59         IsDragDropEnabled .    IsDragTooltipEnabled . +  %Sculptor.TreeListViewDragDropBehavior   	IsEnabled . $��Stretch=�5=   )   $��Stretch=�5>         	RowHeight$ 22Q�5?   )   *	  $Telerik.Windows.Controls.DataControl 	  ItemsSource 5/   )   �� *
Properties+ 	  SelectedItem 50   )   �� *SelectedItem+ �� Mode$ TwoWay=�   IsExpandedBinding 51   )   �� *
IsExpanded+$ TwoWay=�   IsBusy 52   )   �� *IsBusy+.
   (System.Windows.Interactivity.Interaction 
  	Behaviors 5A      )  #Sculptor.TreeListViewSelectBehavior  5B         SelectedItems 6;   �� *SelectedItems+   Columns 5E      1  +Telerik.Windows.Controls.GridViewDataColumn  5F      6  0Telerik.Windows.Controls.GridViewBoundColumnBase   Header$
 Name��5G   )   -  'Telerik.Windows.Controls.GridViewColumn   Width$
 Auto 5H   )      DataMemberBinding 5F   5   �� *PropertyName+   CellTemplate 5J      �� 5K   "   .R�5L   &   ��  �� Orientation$ 
Horizontal=�61   .+�5M   *   �� $��propertyTypeButton��61   $��Property��5O   1   ��5N   1   ��  �� Path$# DataContext.ChangeTypeCommand� �� RelativeSource ��  �� Mode$ FindAncestor=�  �� AncestorType$  UserControlL���5P   .   �� 5Q   2   ! �� 
TargetType"! ��6B   .R�5R   6   �� $��propertyTypeImage��6<   $��0,0,10,0q�5T   5   " �� BitmapScalingMode$
" Fant=�5U   5   $
��Fill=�5V   5   $��16��5W   5   $��16��5X   5   ��5S   5   �� *PropertyType_ID+# �� 	Converter## [  5X   5   5Y   3   5[   +   �� 5\   *   ��64   �� *PropertyName+$ �� ValidatesOnDataErrors$ . % �� UpdateSourceTrigger$% PropertyChanged=�& � ToolTip& 6�   �� *Description+5]   '   5^   #   5`        5b      $ Description��65   $ * 5d   )    5c   )   �� *Description+5d   )     5f      $ Aspect��69   #��[ 5g   )   $
 Auto 5h   )   '   DisplayMemberPath$' 
AspectName��5k   )   (   SelectedValueMemberPath$( 
AspectName��5l   )   )   SortMemberPath$) 
AspectName��5m   )   *   EditTriggers$* 	CellClick=�5n   )   +   IsComboBoxEditable+ . )  #Telerik.Windows.Controls.TextSearch,   TextPath$, 
AspectName��5p       5i   )   �� *Aspect+-   ItemsSource- 5j   )   �� $ 	CBAspects�$ OneWay=�5p        5s      $ 
Attribute1��69   $
 Auto 5t   )   $' 	Attribute��5w   )   $( 	Attribute��5x   )   $) 	Attribute��5y   )   $* 	CellClick=�5z   )   + . $, 	Attribute��5|       5u   )   �� *
Attribute1+- 5v   )   �� $ CBAttributes�5}        5      $ 
Attribute2��69   $
 Auto 5�   )   $' 	Attribute��5�   )   $( 	Attribute��5�   )   $) 	Attribute��5�   )   $* 	CellClick=�5�   )   + . $, 	Attribute��5�       5�   )   �� *
Attribute2+- 5�   )   �� $ CBAttributes�5�        5�      $ 
Attribute3��69   $
 Auto 5�   )   $' 	Attribute��5�   )   $( 	Attribute��5�   )   $) 	Attribute��5�   )   $* 	CellClick=�5�   )   + . $, 	Attribute��5�       5�   )   �� *
Attribute3+- 5�   )   �� $ CBAttributes�5�        5�      $ Value��65   $
 Auto 5�   )    5�   )   �� *Value+5�   )     5�      $ 
Is Deleted��65   .   	IsVisible. .  $ * 5�   )    5�   )   �� *	IsDeleted+5�   )   /   ChildTableDefinitions/ 5�      :  4Telerik.Windows.Controls.TreeListViewTableDefinition  5�      0   ItemsSource0 6>   �� *ChildProperties+-  'Telerik.Windows.Controls.RadContextMenu1   ContextMenu1 5�        5�      -   5        2  Name$2 PropertyTreeContextMenu��5�   1   .@�5�      *  $Telerik.Windows.Controls.RadMenuItem  $��New Sibling��62   3   Command3 6G   �� *AddSiblingCommand+  5�      $��	New Child��62   3 6E   �� *AddChildCommand+  5�      $��Delete��62   3 6B   �� *DeleteCommand+  5�      4   IsSeparator4 . 62     5�      $��Refresh��62   3 6C   �� *RefreshCommand+  5�      $
��Save��62   3 6@   �� *SaveCommand+  5�      4 . 62     5�      $��Load tree state��62   3 6K   �� *LoadTreeStateCommand+  5�      $��Save tree state��62   3 6K   �� *SaveTreeStateCommand+5�      5�      5�      5�        5�   
   $��1��6    .@�5�        $��25��6!   $��80��6-   $��10,0,0,0q�6;   $��Aspects��6M   .��5�        -   5        $ AspectListView��5�   *    . $ Extended=�5�   )    .  .  $	��300��5�      	 .  $
 	Collapsed=�5�      $ * 5�       .  . $��4,0,0,0q�5�   )   $��Stretch=�5�   )   $��Stretch=�5�      $ 22Q�5�   )    5�   )   �� *	Aspects+ 5�   )   �� *SelectedItem+$ TwoWay=� 5�   )   �� *IsBusy+ 5�        5�      $
 Name��5�   1   $
 Auto 5�   1    5�   5   �� *
AspectName+ 5�      �� 5�   "   .R�5�   &   �� $ 
Horizontal=�61   .+�5�   *   �� $��objectTypeImage��60   $A��;D:\Tools\VS\Sculptor\Sculptor\Resources\Images\Aspect32.png��5�   -   $��0,0,10,0q�5�   -   $
" Fant=�5�   -   $
��Fill=�5�   -   $��16��5�   -   $��16��5�   -   �� 5�   *   ��64   �� *
AspectName+5�   '   5�   #   5�        5�      $ Description��65   $
 Auto 5�   1    5�   1   �� *Description+5�   1     5�      5   Header$5 Hard IO��61   $
 Auto 5�   -   6   
IsReadOnly6 .  $* 	CellClick=�5�   -    5�      �� 5�   "   .R�5�   &   �� $��Center=�5�   -   7 d� 	IsChecked7 5�   /   �� *HardIO+$% PropertyChanged=�5�   -   5�   #   5�        5�      $5 External IO��61   $
 Auto 5�   -   6 .  $* 	CellClick=�5�   -    5�      �� 5�   "   .R�5�   &   �� $��Center=�5�   -   7 5�   /   �� *ExtIO+$% PropertyChanged=�5�   -   5�   #   5�        5�      $5 PLC Tag��61   $
 Auto 5   -   6 .  $* 	CellClick=�5  -    5     �� 5  "   .R�5  &   �� $��Center=�5  -   7 5  /   �� *PLCTag+$% PropertyChanged=�5  -   5  #   5	       5     $5 	SCADA Tag��61   $
 Auto 5  -   6 .  $* 	CellClick=�5  -    5     �� 5  "   .R�5  &   �� $��Center=�5  -   7 5  /   �� *
SCADATag+$% PropertyChanged=�5  -   5  #   5       5     $5 	Alarm Tag��61   $
 Auto 5  -   6 .  $* 	CellClick=�5  -    5     �� 5  "   .R�5  &   �� $��Center=�5  -   7 5  /   �� *
AlarmTag+$% PropertyChanged=�5  -   5  #   5!       5#     $5 	Trend Tag��61   $
 Auto 5$  -   6 .  $* 	CellClick=�5&  -    5'     �� 5(  "   .R�5)  &   �� $��Center=�5*  -   7 5)  /   �� *
TrendTag+$% PropertyChanged=�5*  -   5+  #   5-       5.     $
 Note��65   $ * 50  1   8   TextWrapping$8 WrapWithOverflow=�51  1    5/  1   �� *Note+51  1     52     $ 	IsDeleted��65   . .  $
 Auto 55  1    53  1   �� *	IsDeleted+55  1   1 59       5:     -   5        $2 AspectContextMenu��5:  1   .@�5;       $	��New��62   3 6?   �� *
AddCommand+  5<     $��Delete��62   3 6B   �� *DeleteCommand+  5=     4 . 62     5>     $��Refresh��62   3 6C   �� *RefreshCommand+  5?     $
��Save��62   3 6@   �� *SaveCommand+5@     5B     5C     5D       5F  
   $��2��6    .@�5G       $��25��6!   $��80��6-   $��10,0,0,0q�6;   $��
Attributes��6M   .��5H       -   5        $ AttributeListView��5H  *    . $ Extended=�5M  )    .  .  $	��300��5P     	 .  $
 	Collapsed=�5R     $ * 5S      .  . $��4,0,0,0q�5V  )   $��Stretch=�5W  )   $��Stretch=�5X     $ 22Q�5Y  )    5I  )   �� *
Attributes+ 5J  )   �� *SelectedItem+$ TwoWay=� 5K  )   �� *IsBusy+ 5a       5b     $
 Name��5c  )   $
 Auto 5d  )    5b  5   �� *	Attribute+ 5e     �� 5f  "   .R�5g  &   �� $ 
Horizontal=�61   .+�5h  *   �� $��objectTypeImage��60   $D��>D:\Tools\VS\Sculptor\Sculptor\Resources\Images\Attribute32.png��5i  -   $��0,0,10,0q�5j  -   $
" Fant=�5k  -   $
��Fill=�5l  -   $��16��5m  -   $��16��5n  -   �� 5o  *   ��64   �� *	Attribute+5p  '   5q  #   5s       5u     $ Description��65   $ * 5w  1    5v  1   �� *Description+5w  1     5x     $ 	IsDeleted��65   . .  $ * 5{  1    5y  1   �� *	IsDeleted+5{  1   1 5       5�     -   5        $2 AttributeContextMenu��5�  1   .@�5�       $	��New��62   3 6?   �� *
AddCommand+  5�     $��Delete��62   3 6B   �� *DeleteCommand+  5�     4 . 62     5�     $��Refresh��62   3 6C   �� *RefreshCommand+  5�     $
��Save��62   3 6@   �� *SaveCommand+5�     5�     5�     5�     �� 5�  
   $��1��6   $
��Left=�6'   $��0��6B   �� 5�  
   $��2��6   $
��Left=�6'   $��0��6B   5�     5�     