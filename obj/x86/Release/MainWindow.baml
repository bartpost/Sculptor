   M S B A M L   `   `   `  ���� k  gTelerik.Windows.Controls.Input, Version=2018.3.911.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7�bclr-namespace:Telerik.Windows.Controls.ColorEditor.Effects;assembly=Telerik.Windows.Controls.Input,Telerik.Windows.Controls.ColorEditor.Effects   Sculptor   Sculptor.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     n jTelerik.Windows.Controls.GridView, Version=2018.3.911.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7a ]Telerik.Windows.Data, Version=2018.3.911.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7e aTelerik.Windows.Controls, Version=2018.3.911.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7p	 lTelerik.Windows.Controls.Navigation, Version=2018.3.911.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7p
 lTelerik.Windows.Controls.RibbonView, Version=2018.3.911.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7Itelerik1http://schemas.telerik.com/2008/xaml/presentation      	 
 zEffectsbclr-namespace:Telerik.Windows.Controls.ColorEditor.Effects;assembly=Telerik.Windows.Controls.Input      	 
   � Title$  Sculptor��$	��600��$	��800�� � Icon$ Sculptor.ico��.��� .+�, 
 &Telerik.Windows.Controls.RadRibbonView  -     Name$ xRibbonView��   ApplicationButtonContent$
 File��$	��Top=�   ApplicationName$ Sculptor��   TitleBarVisibility$ 	Collapsed=�   IsApplicationMenuOpen .    IsBackstageOpen .     	Backstage 1 
 +Telerik.Windows.Controls.RadRibbonBackstage  -   	  Name$	 xRibbonBackStage��7 
 1Telerik.Windows.Controls.RibbonView.KeyTipService
   
AccessText$
 F��.@�5 
 /Telerik.Windows.Controls.RadRibbonBackstageItem  -     Name$ btnNewProject��   IsSelectable .  $��New Project��$
 N��  -   $ btnOpenProject�� .  $��Open Project��$
 S��  -   $ btnCloseProject�� .  $��Close Project��$
 C��  -   $ 
btnOptions�� .  $��Option��$
 O��  -   $ btnExit�� .  $
��Exit��$
 X��   QuickAccessToolBar 1 
 +Telerik.Windows.Controls.QuickAccessToolBar  .@�+ 
 %Telerik.Windows.Controls.RadRibbonTab  $
��Edit��   
IsSelected . .@�- 
 'Telerik.Windows.Controls.RadRibbonGroup  $��Definitions��.@�. 
 (Telerik.Windows.Controls.RadRibbonButton  -     Name$ btnEditObjectTree��$��Objects��   Size$ Medium=�   
SmallImage$  Resources\Icons\Object.bmp��   
LargeImage$  Resources\Icons\Object.bmp��   
IsAutoSize .   -	   $ btnEditClassTree��$��Classes��$ Medium=�$ Resources\Icons\Class.bmp��$ Resources\Icons\Class.bmp�� .   -
   $ btnEditAttribute��$��
Attributes��$ Medium=�$# Resources\Icons\Attribute.bmp��$# Resources\Icons\Attribute.bmp�� .   $��Requirements��  $��Build�� � -     �Name$
 Main��  � NavigationUIVisibility$ Hidden=�$��0,120,0.286,0.429q�