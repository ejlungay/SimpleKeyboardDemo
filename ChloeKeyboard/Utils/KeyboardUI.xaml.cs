using ChloeKeyboard.Utils.Assets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace ChloeKeyboard.Utils
{
    public sealed partial class KeyboardUI : Page
    {
        /// <summary>
        /// Occurs when the keyboard request a close event
        /// </summary>
        public event EventHandler CloseRequested;
        // the target element of the keyboard
        // this could be a textbox, textblock
        private UIElement targetElement;
        /// <summary>
        /// An event handler for <see cref="OnSubmitEventHandler"/> event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="key"></param>
        /// <param name="submitData"></param>
        public delegate void OnSubmitEventHandler(object sender, VirtualKey key, object submitData);
        /// <summary>
        /// Occurs when the user pressed the enter button/key
        /// </summary>
        public event OnSubmitEventHandler OnSubmit;
        /// <summary>
        /// An event handler for <see cref="OnShiftEventHandler"/> event
        /// </summary>
        /// <param name="shiftEnabled"></param>
        /// <param name="type"></param>
        public delegate void OnShiftEventHandler(bool shiftEnabled, ShiftType type);
        /// <summary>
        /// Occurs when the user pressed the shift key. This event will then be interpreted by the key to update its value-case.
        /// </summary>
        public event OnShiftEventHandler OnShift;
        /// <summary>
        /// An event handler for <see cref="OnStyleChangedEventHandler"/> event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public delegate void OnStyleChangedEventHandler(object sender, StyleData data);
        /// <summary>
        /// Occurs when the style has changed
        /// </summary>
        public event OnStyleChangedEventHandler OnStyleChanged;
        /// <summary>
        /// The current working state of the class
        /// </summary>
        public static KeyboardUI Instance;

        public double CustomFontSize; // just for fonts

        /// <summary>
        /// Creates a new keyboard and applies the default keyboard style. The size will be generated base on the actual size of the window.
        /// </summary>
        public KeyboardUI()
        {
            this.InitializeComponent();
            Instance = this;
            var bounds = Window.Current.Bounds;
            this.Height = bounds.Height;
            this.Width = bounds.Width;

            CustomFontSize = this.FontSize;

            SetSizes(this.Height, this.Width);

            SetStyle(new StyleData());
            InitKeyboard();
        }
        public KeyboardUI(StyleData styleData)
        {
            this.InitializeComponent();
            Instance = this;
            var bounds = Window.Current.Bounds;
            this.Height = bounds.Height;
            this.Width = bounds.Width;

            CustomFontSize = this.FontSize;

            SetSizes(this.Height, this.Width);

            SetStyle(styleData);
            InitKeyboard();
        }
        /// <summary>
        /// Creates a new keyboard with the specified width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public KeyboardUI(double width, double height)
        {
            this.InitializeComponent();
            Instance = this;
            this.Width = width;
            this.Height = height;

            CustomFontSize = this.FontSize;

            SetSizes(width, height);

            InitKeyboard();
            SetStyle(new StyleData());
        }
        /// <summary>
        /// Creates a keyboard with the specified targetted element, keyboard width and keyboard height.
        /// </summary>
        /// <param name="targetElement"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public KeyboardUI(UIElement targetElement, double width, double height)
        {
            this.InitializeComponent();
            Instance = this;
            this.targetElement = targetElement;
            this.Width = width;
            this.Height = height;

            CustomFontSize = this.FontSize;

            SetSizes(width, height);

            InitKeyboard();
            SetStyle(new StyleData());
        }
        void SetSizes(double width, double height)
        {
            row1.Width = width - 60;
            row1.Height = height / 4;

            row2.Width = width - 60;
            row2.Height = height / 4;

            row3.Width = width - 60;
            row3.Height = height / 4;

            row4.Width = width - 60;
            row4.Height = height / 4;
        }
        void ValidateElement()
        {
            if (targetElement.GetType() != typeof(TextBox))
            {
                throw new ArgumentException("Unacceptable element");
            }
        }
        public UIElement TargetElement
        {
            get { return targetElement; }
            set { targetElement = value; }
        }
        void InitKeyboard()
        {
            // adding keys to row 1
            string[] row1Data = new string[] { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P" };
            foreach (var letter in row1Data)
            {
                var c_w = row1.Width;
                var c_h = row1.Height;

                var k_w = (c_w / row1Data.Length) - 10;
                var k_h = c_h - 20;

                KeyUserControl key = new KeyUserControl(k_w, k_h, letter.ToLower());
                key.Margin = new Thickness(5, 0, 5, 0);
                key.OnKeyClick += Key_OnKeyClick;

                row1.Children.Add(key);
            }

            string[] row2Data = new string[] { "A", "S", "D", "F", "G", "H", "J", "K", "L" };
            foreach (var letter in row2Data)
            {
                var c_w = row2.Width - 60;
                var c_h = row2.Height - 20;

                var k_w = (c_w / row2Data.Length) - 10;
                var k_h = c_h;

                KeyUserControl key = new KeyUserControl(k_w, k_h, letter.ToLower());
                key.Margin = new Thickness(5, 0, 5, 0);
                key.OnKeyClick += Key_OnKeyClick;

                row2.Children.Add(key);
            }

            string[] row3Data = new string[] { "SHIFT", "Z", "X", "C", "V", "B", "N", "M", "Delete" };
            foreach (var letter in row3Data)
            {
                var c_w = row3.Width - 50;
                var c_h = row3.Height - 20;

                var k_w = letter == "SHIFT" || letter == "Delete" ? (c_w / row3Data.Length) + 10 : (c_w / row3Data.Length) - 10;
                var k_h = c_h;

                KeyUserControl key = new KeyUserControl(k_w, k_h, letter.ToLower());
                key.Margin = new Thickness(5, 0, 5, 0);
                key.OnKeyClick += Key_OnKeyClick;
                key.OnDeleteHold += Key_OnDeleteHold;
                key.OnDoubleShift += Key_OnDoubleShift;

                row3.Children.Add(key);
            }

            string[] row4Data = new string[] { "123", "@", "Space", ".com", "return" };
            foreach (var letter in row4Data)
            {
                var c_w = row4.Width - 50;
                var c_h = row4.Height - 20;

                var k_w = c_w / row4Data.Length - 10;
                var k_h = c_h;

                if (letter == "Space")
                    k_w += 40;

                KeyUserControl key = new KeyUserControl(k_w, k_h, letter.ToLower());
                key.Margin = new Thickness(5, 0, 5, 0);
                key.OnKeyClick += Key_OnKeyClick;

                row4.Children.Add(key);
            }
        }

        private void Key_OnDoubleShift(bool enable)
        {
            stype = ShiftType.Permanent;
        }

        private void Key_OnDeleteHold(bool still_holding)
        {
            if (still_holding)
                (targetElement as TextBox).Text = "";
        }

        bool shift = false;
        ShiftType stype;
        private void Key_OnKeyClick(object sender, string value)
        {
            ValidateElement();

            if (sender != null)
            {
                var src = sender as KeyUserControl;
                if (src.is_action_button)
                {
                    if (src.actionButton == ActionButtons.Shift)
                    {
                        if (shift)
                        {
                            if (stype == ShiftType.Permanent)
                            {
                                shift = false;
                                stype = ShiftType.Disable;
                                OnShift?.Invoke(false, ShiftType.Disable);
                            }
                        }
                        else
                        {
                            shift = true;
                            OnShift?.Invoke(true, ShiftType.Single);
                        }
                    }
                    else if (src.actionButton == ActionButtons.Delete)
                    {
                        var txt = (targetElement as TextBox).Text;
                        if (txt.Length > 0)
                        {
                            (targetElement as TextBox).Text = txt.Remove(txt.Length - 1, 1);
                        }
                    }
                }
                else
                {
                    if (shift)
                    {
                        (targetElement as TextBox).Text += value.ToUpper();
                        if (stype == ShiftType.Single)
                        {
                            shift = false;
                            OnShift?.Invoke(false, ShiftType.Disable);
                        }
                    }
                    else
                    {
                        if (src.actionButton == ActionButtons.Space)
                        {
                            (targetElement as TextBox).Text += " ";
                        }
                        else if (src.actionButton == ActionButtons.Numpad)
                        {
                            return;
                        }
                        else if (src.actionButton == ActionButtons.Return)
                        {
                            OnSubmit?.Invoke(src, VirtualKey.Enter, (targetElement as TextBox).Text);
                        }
                        else
                        {
                            (targetElement as TextBox).Text += value;
                        }
                    }
                }
                (targetElement as TextBox).Select((targetElement as TextBox).Text.Length, 0);
            }
        }

        void Close()
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Set custom style to a keyboard
        /// </summary>
        /// <param name="data"></param>
        public void SetStyle(StyleData data)
        {
            if (data == null)
                throw new ArgumentNullException("Style is null");

            root.Background = data.KeyboardBackground;
            OnStyleChanged?.Invoke(this, data);
        }
    }
    public enum ShiftType
    {
        Single,
        Permanent,
        Disable
    }
    public class StyleData
    {
        public SolidColorBrush KeyboardBackground { get; set; } = new SolidColorBrush() { Color = Colors.Black };
        public SolidColorBrush KeyNormalState { get; set; } = new SolidColorBrush() { Color = Colors.White };
        public SolidColorBrush KeyActiveState { get; set; } = new SolidColorBrush() { Color = Colors.Red };
        public SolidColorBrush KeyForeground{ get; set; } = new SolidColorBrush() { Color = Colors.Black };
    }
}
