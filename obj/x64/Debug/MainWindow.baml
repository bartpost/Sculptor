   M S B A M L   `   `   `  ����C  ?Sculptor, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:SculptorSculptor  5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R8clr-namespace:System.ComponentModel;assembly=WindowsBaseSystem.ComponentModel 5          Sculptor.MainWindow   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      p lTelerik.Windows.Controls.Navigation, Version=2019.1.116.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7p lTelerik.Windows.Controls.RibbonView, Version=2019.1.116.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7k gTelerik.Windows.Documents.Core, Version=2019.1.114.40, Culture=neutral, PublicKeyToken=5803cfa389c90ce7~ zTelerik.Windows.Documents.FormatProviders.OpenXml, Version=2019.1.116.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7k	 gTelerik.Windows.Controls.Input, Version=2019.1.116.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7q
 mTelerik.Windows.PersistenceFramework, Version=2019.1.116.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7n jTelerik.Windows.Controls.GridView, Version=2019.1.116.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7z vTelerik.Windows.Documents.FormatProviders.Rtf, Version=2019.1.116.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7a ]Telerik.Windows.Data, Version=2019.1.116.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7e aTelerik.Windows.Controls, Version=2019.1.116.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7f bTelerik.Windows.Documents, Version=2019.1.116.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7s oTelerik.Windows.Controls.RichTextBoxUI, Version=2019.1.116.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7{ wTelerik.Windows.Documents.FormatProviders.Xaml, Version=2019.1.116.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7Wtelerik1http://schemas.telerik.com/2008/xaml/presentation     	 
        5      :localclr-namespace:Sculptor     	 
        5      Zscm8clr-namespace:System.ComponentModel;assembly=WindowsBase     	 
        5      $
��1000��5      $
��2000��6     � Icon$  Sculptor.ico��6     � WindowStartupLocation$ CenterScreen=�64    � Title 5      ��  �� Path$ 	MainTitle� � 	Resources 5         Sculptor.BinaryToImage   binaryToImage  
FrameStyle �� 
TargetType �� Property �� Value �� 
TargetType	 �� Orientation
 �� Padding �� CornerRadius  sortedProjects �� Source �� SortDescriptions+  %System.ComponentModel.SortDescription   PropertyName   	Direction��%  &	        &	      &	 o      5   
   6   �� 5   
   "  �6#   .�5      ��  � �� 5      �� 5      "  �6&   .R�5      ]� $��5q�6$   .+�5      �� $��5q�6)   $	 
Horizontal=�64   $	��Top=�6M   5      �� 5      
�������$��0,0,0,0q�6=   $��2q�6N   $
 0q�6b   $ 0��6n   
�������._�5   "   �� 5      5      5      5       5!      �� 5$   
    66   �� *
Projects+ 5%        5&      $ 
LastOpened��6&   $ 
Descending=�6@   5(      5)      .��5+      �  � RowDefinitions 5,   
   .+�51   
   ,  &Telerik.Windows.Controls.RadRibbonView  -   5          Name$ SculptorRibbonView��51          ApplicationButtonContent$
 File��52       $	��Top=�53          ApplicationName$ Sculptor��54          TitleBarVisibility$ 	Collapsed=�55          IsBackstageOpen 56       �� *IsBackStageOpen+ �� Mode$ TwoWay=�   	Backstage 58      1  +Telerik.Windows.Controls.RadRibbonBackstage  59      -   5          Name$ xRibbonBackStage��59   -   7  1Telerik.Windows.Controls.RibbonView.KeyTipService   
AccessText$ F��6G   .@�5:      5  /Telerik.Windows.Controls.RadRibbonBackstageItem  -   5          Name$ 
NewProject��5:   J      IsSelectable . $��New Project��6^   $ N��6s   � 5;       � ColumnDefinitions 5<      �� 5=      $��* �6/   �� 5>      $��* �6/   .+�5@      �� .+�5A      �� $��Setup a new project��6(   5B      5C        5D      -   5        $ OpenProject��5D   5   $��Open Project��5E   5      	IsDefault . $ S��5G   5   .��5I      � .+�5J      �  5K   "   �� 5L   &   $��* �67   �� 5M   &   $��* �67   5O      �� 5P      .+�5Q   "   �� $��0 4 0 0q�6n   ��6/   ��  �� Source# [ ��5R   &   �� 5S   *   .R�5T   .   .  (Telerik.Windows.Controls.RadRibbonButton  $	��600��6F   $
��Left=�5U   F   C  =Telerik.Windows.Controls.RichTextBoxUI.RadRichTextBoxRibbonUI   ShouldCloseBackstage . ��5V   F   �� *" DataContext.SelectProjectCommand+  �� RelativeSource  �� ! �� AncestorType$! ItemsControlL���5W   F   �� *ID+.��5Y   2   �  5Z   6   �� 5[   :   $	��150 �6K   �� 5\   :   $��* �6K   �� 5]   :   $	��300 �6K   �� 5^   :   $��* �6K    5`   6   �� 5a   :   $��* �6H   �� 5b   :   $��* �6H   .+�5d   6   �� $��0��6<   $��2��6L   " �� BitmapScalingMode$
" Fant=�5f   <   $��32��5g   <   $��0,0,5,0q�5h   <   ��5e   <   �� *Logo+# �� 	Converter## [  5h   <   �� 5i   6   $��0��6@   $��1��6M   $��0,0,5,0q�6]   ��6n   �� *
ContractNo+�� 5j   6   $��0��6@   $��2��6M   $��0,0,0,0q�6]   ��6n   �� *ProjectName+�� 5k   6   $��1��6@   $��1��6M   $��2��6]   $��0,0,5,0q�6q   ��6�   �� *CustomerName+�� 5l   6   $��0��6@   $��3��6M   ��6]   �� *
LastOpened+$ �� StringFormat$$ dd-MM-yyyy hh:mm���� 5m   6   $��1��6@   $��4��6M   $��0,0,0,10q�6]   ��6o   �� *LastOpenedBy+5n   3   5o   /   5p   +   5r   #   5s      5t      5u        5v      -   5        $ CloseProject��5v   5    .  $��Close Project��5x   5   $ C��5z   5   %   Command% 5y   5   �� *!DataContext.CloseProjectCommand+  �� $! ItemsControlL�5z   5     5{      &   IsGroupSeparator& . 65     5|      -   5        $ Options��5|   5    . $��Option��5~   5   $ O��5   5     5�      -   5        $
 Exit��5�   5    .  $
��Exit��5�   5   $ X��5�   5   5�      '   QuickAccessToolBar' 5�      1	  +Telerik.Windows.Controls.QuickAccessToolBar	  5�      .@�5�        (   Text$
( Save��6.   )   Size$) Small=�5�      $ 1��6   5�      .@�5�      +
  %Telerik.Windows.Controls.RadRibbonTab
  $
��Edit��6#   * 
  
IsSelected* . .@�5�      -  'Telerik.Windows.Controls.RadRibbonGroup  $��Definitions��6)   .@�5�        -   5        +  Name$+ EditObjectTreeButton��5�   .   $��Objects��5�   .   $) Large=�5�   .   ,   
SmallImage$%, ../Resources/Images/Objects.png��5�   .   -   
LargeImage$%- ../Resources/Images/Objects.png��5�   .   .   
IsAutoSize. . {�. 5�   .     5�      -	   5        $!+ EditControlObjectTreeButton��5�   .   $��Control Object��5�   .   $) Large=�5�   .   $,, &../Resources/Images/ControlObjects.png��5�   .   $,- &../Resources/Images/ControlObjects.png��5�   .   . . {�. 5�   .     5�      -
   5        $+ EditTemplateButton��5�   .   $��	Templates��5�   .   $) Large=�5�   .   $&,  ../Resources/Images/Template.png��5�   .   $&-  ../Resources/Images/Template.png��5�   .   . . {�. 5�   .     5�      -   5        $+ EditPropertyButton��5�   .   $��
Properties��5�   .   $) Large=�5�   .   $&,  ../Resources/Images/Property.png��5�   .   $&-  ../Resources/Images/Property.png��5�   .   . . {�. 5�   .   5�        5�      $��Requirements��6)   .@�5�        -   5        $+ EditRequirementTreeButton��5�   .   $��Requirements��5�   .   $) Large=�5�   .   $,, &../Resources/Images/Requirements32.png��5�   .   $,- &../Resources/Images/Requirements32.png��5�   .   . . 5�   .   5�      5�      
  5�      $��Build��6#   $��Right=�62   $��52��6N   5�      5�       � 5�      -   5        /  �Name$/ 	MainFrame��5�      0  � NavigationUIVisibility$0 	Automatic=�6'   $��	0,120,0,0q�6J   #��[ 6]   �� 5�   
   5�      5�      