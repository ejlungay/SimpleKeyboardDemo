using ChloeKeyboard.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ChloeKeyboard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Popup popup;
        public MainPage()
        {
            this.InitializeComponent();

            // Simple keyboard App
            // this demo includes creation of Custom events such as 
            // creating a custom event that will trigger when the user pressed the enter key
            // To view app code directly, click on any constructors and press F12

            // The constructor of the class
            KeyboardUI keyboard = new KeyboardUI(target, 600, 400);
            keyboard.CloseRequested += Keyboard_CloseRequested; // this is a custom event that will trigger when the keyboard request a close event
            keyboard.OnSubmit += Keyboard_OnSubmit; // this is a custom keyboard event that will be triggered when the user pressed the enter  button/key

            // this is a simple example on applying custom themes on runtime
            // such as changing button color, active state, normal state, etc.
            keyboard.SetStyle(new StyleData()
            {
                KeyActiveState = new SolidColorBrush() { Color = Colors.Black },
                KeyboardBackground = new SolidColorBrush() { Color = Colors.Gray },
                KeyForeground = new SolidColorBrush() { Color = Colors.Red },
                KeyNormalState = new SolidColorBrush() { Color = Colors.White }
            });

            // this is the modal equivalent in xaml
            popup = new Popup();
            popup.Child = keyboard;
            root.Children.Add(popup);
            popup.HorizontalAlignment = HorizontalAlignment.Stretch;
            popup.IsOpen = true;
        }

        private void Keyboard_OnSubmit(object sender, Windows.System.VirtualKey key, object submitData)
        {
            Debug.WriteLine(submitData);
        }

        private void Keyboard_CloseRequested(object sender, EventArgs e)
        {

        }

        private void KeyboardUI_OnShift(bool shiftEnabled)
        {

        }
    }
}
