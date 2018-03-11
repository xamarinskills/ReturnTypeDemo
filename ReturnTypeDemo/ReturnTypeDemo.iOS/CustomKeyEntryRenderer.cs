using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using ReturnTypeDemo;
using ReturnTypeDemo.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomKeyEntry), typeof(CustomKeyEntryRenderer))]
namespace ReturnTypeDemo.iOS
{
    public class CustomKeyEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            CustomKeyEntry entry = (CustomKeyEntry)this.Element;
            if (this.Control != null)
            {
                if (entry != null)
                {
                    SetReturnType(entry);
                    Control.ShouldReturn += (UITextField tf) =>
                    {
                        entry.InvokeCompleted();
                        return true;
                    };
                }
            }
        }

        private void SetReturnType(CustomKeyEntry entry)
        {
            ReturnType type = entry.ReturnType;
            switch (type)
            {
                case ReturnType.Go:
                    Control.ReturnKeyType = UIReturnKeyType.Go;
                    break;
                case ReturnType.Next:
                    Control.ReturnKeyType = UIReturnKeyType.Next;
                    break;
                case ReturnType.Send:
                    Control.ReturnKeyType = UIReturnKeyType.Send;
                    break;
                case ReturnType.Search:
                    Control.ReturnKeyType = UIReturnKeyType.Search;
                    break;
                case ReturnType.Done:
                    Control.ReturnKeyType = UIReturnKeyType.Done;
                    break;
                default:
                    Control.ReturnKeyType = UIReturnKeyType.Default;
                    break;
            }
        }
    }
}