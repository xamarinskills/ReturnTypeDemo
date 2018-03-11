using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ReturnTypeDemo.FloatingEntryView
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FloatingEntry : ContentView
	{
		public FloatingEntry ()
		{
			InitializeComponent ();
            EntryField.BindingContext = this;
            EntryField.PlaceholderColor = Color.White;
            EntryField.Focused += async (s, a) =>
            {
                HiddenBottomBorder.BackgroundColor = AccentColor;
                EntryField.TextColor = HiddenLabel.TextColor = AccentColor;

                HiddenLabel.IsVisible = true;
                if (string.IsNullOrEmpty(EntryField.Text))
                {
                    // animate both at the same time
                    await Task.WhenAll(
                        HiddenBottomBorder.LayoutTo(new Rectangle(BottomBorder.X, BottomBorder.Y, BottomBorder.Width, BottomBorder.Height + 2), 200),
                        HiddenLabel.FadeTo(1, 120),
                        HiddenLabel.TranslateTo(HiddenLabel.TranslationX - 13, EntryField.Y - EntryField.Height + 4, 200, Easing.BounceIn)
                     );
                    EntryField.Placeholder = null;
                }
                else
                {
                    await HiddenBottomBorder.LayoutTo(new Rectangle(BottomBorder.X, BottomBorder.Y, BottomBorder.Width, BottomBorder.Height), 200);
                }
            };
            EntryField.Unfocused += async (s, a) =>
            {

                if (string.IsNullOrEmpty(EntryField.Text))
                {
                    // animate both at the same time
                    await Task.WhenAll(
                        HiddenBottomBorder.LayoutTo(new Rectangle(BottomBorder.X, BottomBorder.Y, 0, BottomBorder.Height + 2), 200),
                        HiddenLabel.FadeTo(0, 180),
                        HiddenLabel.TranslateTo(HiddenLabel.TranslationX + 13, EntryField.Y, 50, Easing.BounceIn)
                     );
                    EntryField.Placeholder = Placeholder;
                }
                else
                {
                    await HiddenBottomBorder.LayoutTo(new Rectangle(BottomBorder.X, BottomBorder.Y, 0, BottomBorder.Height), 100);
                }
                //HiddenLabel.TextColor = Color.Gray;
            };
        }
        public static BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(FloatingEntry), defaultBindingMode: BindingMode.TwoWay);
        public static BindableProperty PlaceholderProperty =
            BindableProperty.Create(nameof(Placeholder),
                typeof(string), typeof(FloatingEntry),
                defaultBindingMode: BindingMode.TwoWay,
                propertyChanged: (bindable, oldVal, newval) =>
                {
                    var matEntry = (FloatingEntry)bindable;
                    matEntry.EntryField.Placeholder = (string)newval;
                    matEntry.EntryField.PlaceholderColor = Color.Gray;
                    matEntry.HiddenLabel.Text = (string)newval;
                });

        public static BindableProperty IsPasswordProperty = BindableProperty.Create(nameof(IsPassword), typeof(bool), typeof(FloatingEntry), defaultValue: false, propertyChanged: (bindable, oldVal, newVal) =>
        {
            var matEntry = (FloatingEntry)bindable;
            matEntry.EntryField.IsPassword = (bool)newVal;
        });
        public static BindableProperty KeyboardProperty = BindableProperty.Create(nameof(Keyboard), typeof(Keyboard), typeof(FloatingEntry), defaultValue: Keyboard.Default, propertyChanged: (bindable, oldVal, newVal) =>
        {
            var matEntry = (FloatingEntry)bindable;
            matEntry.EntryField.Keyboard = (Keyboard)newVal;
        });
        public static BindableProperty AccentColorProperty =
            BindableProperty.Create(nameof(AccentColor),
                typeof(Color),
                typeof(FloatingEntry),
                defaultValue: Color.Accent);

        public Color AccentColor
        {
            get
            {
                return (Color)GetValue(AccentColorProperty);
            }
            set
            {
                SetValue(AccentColorProperty, value);
            }
        }
        public Keyboard Keyboard
        {
            get
            {
                return (Keyboard)GetValue(KeyboardProperty);
            }
            set
            {
                SetValue(KeyboardProperty, value);
            }
        }

        public bool IsPassword
        {
            get
            {
                return (bool)GetValue(IsPasswordProperty);
            }
            set
            {
                SetValue(IsPasswordProperty, value);
            }
        }

        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        public string Placeholder
        {
            get
            {
                return (string)GetValue(PlaceholderProperty);
            }
            set
            {
                SetValue(PlaceholderProperty, value);
            }
        }

    }

}