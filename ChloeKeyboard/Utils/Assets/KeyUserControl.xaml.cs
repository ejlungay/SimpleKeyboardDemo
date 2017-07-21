using System;
using System.Collections.Generic;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ChloeKeyboard.Utils.Assets
{
    public sealed partial class KeyUserControl : UserControl
    {
        public delegate void OnKeyClickEventHandler(object sender, string value);
        public event OnKeyClickEventHandler OnKeyClick;

        public delegate void OnDeleleteHoldEventHandler(bool still_holding);
        public event OnDeleleteHoldEventHandler OnDeleteHold;

        public delegate void OnDoubleShiftEventHandler(bool enable);
        public event OnDoubleShiftEventHandler OnDoubleShift;

        SolidColorBrush activeState;
        SolidColorBrush normalState;

        public bool is_action_button = false;
        public ActionButtons actionButton;

        private double fontSize;
        public KeyUserControl(double width, double height, string keyValue)
        {
            this.InitializeComponent();

            KeyboardUI.Instance.OnShift += Instance_OnShift;
            KeyboardUI.Instance.OnStyleChanged += Instance_OnStyleChanged;

            string[] iconLetters = new string[] { "shift", "delete", "space", "return", "123", ".com", "@" };
            var bounds = Window.Current.Bounds;
            this.Width = width;
            this.Height = height;

            btn.Width = width;
            btn.Height = height;
            //btnCOntent.Width = width;
            //btnCOntent.Height = height;

            textValue.Text = keyValue;
            textValue.FontSize = this.Height / 4;

            textValue.VerticalAlignment = VerticalAlignment.Top;
            textValue.HorizontalAlignment = HorizontalAlignment.Left;

            activeState = new SolidColorBrush() { Color = Colors.Red };
            normalState = new SolidColorBrush() { Color = Colors.White };

            if (iconLetters.Where(l => l.ToLower() == keyValue.ToLower()).ToList().Count <= 0)
            {
                letterIcon.Visibility = Visibility.Collapsed;
            }
            else
            {
                letterIcon.Width = width - 10;
                letterIcon.Height = height - 20;
                is_action_button = true;
                if (keyValue == "shift")
                {
                    letterIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/icon_shift.png"));
                    actionButton = ActionButtons.Shift;
                    textValue.Visibility = Visibility.Collapsed;
                }
                else if (keyValue == "delete")
                {
                    letterIcon.Source = new BitmapImage(new Uri("ms-appx:///Assets/icon_delete.png"));
                    actionButton = ActionButtons.Delete;
                    textValue.Visibility = Visibility.Collapsed;
                }
                else
                {
                    if (keyValue == "space")
                        actionButton = ActionButtons.Space;
                    if (keyValue == "return")
                        actionButton = ActionButtons.Return;
                    if (keyValue == "123")
                        actionButton = ActionButtons.Numpad;
                    if (keyValue == ".com")
                        actionButton = ActionButtons.COM;
                    if (keyValue == "@")
                        actionButton = ActionButtons.At;

                    letterIcon.Visibility = Visibility.Collapsed;
                    textValue.VerticalAlignment = VerticalAlignment.Center;
                    textValue.HorizontalAlignment = HorizontalAlignment.Center;
                    is_action_button = false;
                }
            }
        }
        public double CustomFontSize
        {
            get { return FontSize; }
            set { fontSize = value;
            }
        }
        private void Instance_OnStyleChanged(object sender, StyleData data)
        {
            SetStyle(data);
        }

        public void SetStyle(StyleData data)
        {
            if (data == null)
                throw new ArgumentNullException("Style data is null");

            btn.Background = data.KeyNormalState;
            activeState = data.KeyActiveState;
            normalState = data.KeyNormalState;
            textValue.Foreground = data.KeyboardBackground;
        }

        private void Instance_OnShift(bool enabled, ShiftType type)
        {
            if (!is_action_button)
            {
                textValue.Text = enabled ? textValue.Text.ToUpper() : textValue.Text.ToLower();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnKeyClick?.Invoke(sender, textValue.Text);
        }

        private void Button_Click(object sender, TappedRoutedEventArgs e)
        {
            OnKeyClick?.Invoke(this, textValue.Text);
        }

        private void btn_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            (sender as Grid).Background = activeState;
        }

        private void btn_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            (sender as Grid).Background = normalState;
        }

        private void btn_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (actionButton == ActionButtons.Delete)
            {
                OnDeleteHold?.Invoke(true);
            }
        }

        private void btn_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (actionButton == ActionButtons.Delete)
            {
                OnDeleteHold?.Invoke(true);
            }
        }

        private void btn_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (actionButton == ActionButtons.Shift)
            {
                OnDoubleShift?.Invoke(true);
            }
        }
    }
    public enum ActionButtons
    {
        Delete,
        Shift,
        Space,
        Numpad,
        Return,
        At,
        COM,
        Normal
    }
}
