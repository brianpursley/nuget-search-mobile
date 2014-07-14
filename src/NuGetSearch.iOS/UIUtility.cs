using System;
using MonoTouch.UIKit;

namespace NuGetSearch.IOS
{
    public static class UIUtility
    {
        public static UILabel CreateLabel(string fontName, float fontSize, UIColor color, UITextAlignment textAlignment)
        {
            return new UILabel()
            {
                Font = UIFont.FromName(fontName, fontSize),
                TextColor = color,
                TextAlignment = textAlignment
            };
        }
    }
}

