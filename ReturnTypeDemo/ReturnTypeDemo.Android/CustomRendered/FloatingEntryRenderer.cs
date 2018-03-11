﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Java.Lang;
using ReturnTypeDemo.CustomView;
using ReturnTypeDemo.Droid.CustomRendered;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(FloatingEntry), typeof(FloatingEntryRenderer))]
namespace ReturnTypeDemo.Droid.CustomRendered
{
    using Application = Android.App.Application;
    using FormsAppCompat = Xamarin.Forms.Platform.Android.AppCompat;

    public class FloatingEntryRenderer : FormsAppCompat.ViewRenderer<FloatingEntry, TextInputLayout>,
         ITextWatcher, TextView.IOnEditorActionListener
    {
        private ColorStateList _defaultHintColor;
        private ColorStateList _defaultTextColor;

        public FloatingEntryRenderer()
        {
            AutoPackage = false;
        }

        private EditText EditText => Control.EditText;

        public bool OnEditorAction(TextView v, ImeAction actionId, KeyEvent e)
        {
            if ((actionId == ImeAction.Done) || ((actionId == ImeAction.ImeNull) && (e.KeyCode == Keycode.Enter)))
            {
                Control.ClearFocus();
                HideKeyboard();
                ((IEntryController)Element).SendCompleted();
            }
            return true;
        }

        public virtual void AfterTextChanged(IEditable s)
        {
        }

        public virtual void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {
        }

        public virtual void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
            if (string.IsNullOrWhiteSpace(Element.Text) && (s.Length() == 0)) return;
            ((IElementController)Element).SetValueFromRenderer(Entry.TextProperty, s.ToString());
        }

        protected override TextInputLayout CreateNativeControl()
        {
            var textInputLayout = new TextInputLayout(Context);
            var editText = new EditText(Context);
            textInputLayout.AddView(editText);
            return textInputLayout;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<FloatingEntry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
                EditText.FocusChange -= ControlOnFocusChange;

            if (e.NewElement != null)
            {
                var ctrl = CreateNativeControl();
                SetNativeControl(ctrl);

                if (!string.IsNullOrWhiteSpace(Element.AutomationId))
                    EditText.ContentDescription = Element.AutomationId;

                _defaultHintColor = EditText.HintTextColors;
                _defaultTextColor = EditText.TextColors;

                Focusable = true;
                Control.HintEnabled = true;
                Control.HintAnimationEnabled = true;
                EditText.ShowSoftInputOnFocus = true;

                // Subscribe
                EditText.FocusChange += ControlOnFocusChange;
                EditText.AddTextChangedListener(this);
                EditText.SetOnEditorActionListener(this);
                EditText.ImeOptions = ImeAction.Done;

                SetText();
                SetInputType();
                SetHintText();
                SetTextColor();
                SetHintTextColor();
                SetHorizontalTextAlignment();
                //SetErrorText();
                SetFont();
                SetFloatingHintEnabled();
                SetIsEnabled();

                //Control.ErrorEnabled = true;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Entry.PlaceholderProperty.PropertyName)
                SetHintText();
            // else if (e.PropertyName == FloatingEntry.ErrorTextProperty.PropertyName)
            //   SetErrorText();
            else if (e.PropertyName == Entry.TextColorProperty.PropertyName)
                SetTextColor();
            else if (e.PropertyName == Entry.IsPasswordProperty.PropertyName)
                SetInputType();
            else if (e.PropertyName == Entry.TextProperty.PropertyName)
                SetText();
            else if (e.PropertyName == Entry.PlaceholderColorProperty.PropertyName)
                SetHintTextColor();
            else if (e.PropertyName == InputView.KeyboardProperty.PropertyName)
                SetInputType();
            else if (e.PropertyName == Entry.HorizontalTextAlignmentProperty.PropertyName)
                SetHorizontalTextAlignment();
            else if (e.PropertyName == FloatingEntry.FloatingHintEnabledProperty.PropertyName)
                SetFloatingHintEnabled();
            else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
                SetIsEnabled();
            else if ((e.PropertyName == Entry.FontAttributesProperty.PropertyName) ||
                     (e.PropertyName == Entry.FontFamilyProperty.PropertyName) ||
                     (e.PropertyName == Entry.FontSizeProperty.PropertyName))
                SetFont();
        }

        private void ControlOnFocusChange(object sender, FocusChangeEventArgs args)
        {
            if (args.HasFocus)
            {
                var manager = (InputMethodManager)Application.Context.GetSystemService(Context.InputMethodService);

                EditText.PostDelayed(() =>
                {
                    EditText.RequestFocus();
                    manager.ShowSoftInput(EditText, 0);
                },
                    100);
            }

            //var isFocusedPropertyKey = Element.GetInternalField<BindablePropertyKey>("IsFocusedPropertyKey");
            //((IElementController)Element).SetValueFromRenderer(isFocusedPropertyKey, args.HasFocus);
        }

        private void SetText()
        {
            if (EditText.Text != Element.Text)
            {
                EditText.Text = Element.Text;
                if (EditText.IsFocused)
                    EditText.SetSelection(EditText.Text.Length);
            }
        }

        private void SetHintText()
        {
            Control.Hint = Element.Placeholder;
        }

        private void SetHintTextColor()
        {
            if (Element.PlaceholderColor == Color.Default)
                EditText.SetHintTextColor(_defaultHintColor);
            else
                EditText.SetHintTextColor(Element.PlaceholderColor.ToAndroid());
        }

        private void SetTextColor()
        {
            if (Element.TextColor == Color.Default)
                EditText.SetTextColor(_defaultTextColor);
            else
                EditText.SetTextColor(Element.TextColor.ToAndroid());
        }

        private void SetHorizontalTextAlignment()
        {
            switch (Element.HorizontalTextAlignment)
            {
                case Xamarin.Forms.TextAlignment.Center:
                    EditText.Gravity = GravityFlags.CenterHorizontal;
                    break;
                case Xamarin.Forms.TextAlignment.End:
                    EditText.Gravity = GravityFlags.Right;
                    break;
                default:
                    EditText.Gravity = GravityFlags.Left;
                    break;
            }
        }

        public void SetFloatingHintEnabled()
        {
            Control.HintEnabled = Element.FloatingHintEnabled;
        }

        protected void HideKeyboard()
        {
            var manager = (InputMethodManager)Application.Context.GetSystemService(Context.InputMethodService);
            manager.HideSoftInputFromWindow(EditText.WindowToken, 0);
        }

        private void SetFont()
        {   //We can use Extantion on Extation Folder
            //Control.Typeface = Element.ToTypeface();
            EditText.SetTextSize(ComplexUnitType.Sp, (float)Element.FontSize);
        }

        private void SetErrorText()
        {
            if (!string.IsNullOrEmpty(Element.ErrorText))
            {
                Control.ErrorEnabled = true;
                Control.Error = Element.ErrorText;
            }
            else
            {
                Control.Error = null;
                Control.ErrorEnabled = false;
            }
        }

        private void SetIsEnabled()
        {
            EditText.Enabled = Element.IsEnabled;
        }

        private void SetInputType()
        {
            EditText.InputType = Element.Keyboard.ToInputType();
            if (Element.IsPassword && ((EditText.InputType & InputTypes.ClassText) == InputTypes.ClassText))
            {
                EditText.InputType = EditText.InputType | InputTypes.TextVariationPassword;
            }
            if (Element.IsPassword && ((EditText.InputType & InputTypes.ClassNumber) == InputTypes.ClassNumber))
            {
                EditText.InputType = EditText.InputType | InputTypes.NumberVariationPassword;
            }
        }
    }
}