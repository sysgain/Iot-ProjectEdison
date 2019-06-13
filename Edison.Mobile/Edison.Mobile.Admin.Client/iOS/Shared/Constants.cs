﻿using System;
using UIKit;

namespace Edison.Mobile.Admin.Client.iOS.Shared
{
    public static class Constants
    {
        public static readonly string ClientId = "2373be1e-6d0b-4e38-9115-e0bd01dadd61";

        public static nfloat Padding = 16;
        public static nfloat CircleNumberSize = 32;

        public static class Color
        {
            public static UIColor White = UIColor.White;
            public static UIColor MidGray = new UIColor(136 / 255f, 134 / 255f, 160 / 255f, 1);
            public static UIColor BackgroundLightGray = new UIColor(249 / 255f, 249 / 255f, 250 / 255f, 1);
            public static UIColor DarkGray = new UIColor(62 / 255f, 61 / 255f, 74 / 255f, 1);
            public static UIColor DarkBlue = new UIColor(51 / 255f, 34 / 255f, 255 / 255f, 1);
            public static UIColor Green = new UIColor(40 / 255f, 203 / 255f, 78 / 255f, 1);
        }

        public static class Fonts
        {
            public static UIFont RubikOfSize(float size) => UIFont.FromName("Rubik-Regular", size);
            public static UIFont RubikMediumOfSize(float size) => UIFont.FromName("Rubik-Medium", size);

            public static class Size
            {
                public static float Eight = 8;
                public static float Ten = 10;
                public static float Twelve = 12;
                public static float Fourteen = 14;
                public static float Sixteen = 16;
                public static float Eighteen = 18;
                public static float TwentyFour = 24;
                public static float SeventyTwo = 72;
            }
        }

        public static class Assets
        {
            public static UIImage SensorsGray => UIImage.FromBundle("SensorsGray");
            public static UIImage SensorsBlue => UIImage.FromBundle("SensorsBlue");
            public static UIImage LoginLogo => UIImage.FromBundle("LoginLogo");
            public static UIImage Menu => UIImage.FromBundle("Menu");
            public static UIImage Plus => UIImage.FromBundle("Plus");
            public static UIImage Gear => UIImage.FromBundle("Gear");
            public static UIImage ArrowRight => UIImage.FromBundle("ArrowRight");
            public static UIImage ArrowLeft => UIImage.FromBundle("ArrowLeft");
            public static UIImage Power => UIImage.FromBundle("Power");
            public static UIImage Lines => UIImage.FromBundle("Lines");
            public static UIImage Check => UIImage.FromBundle("Check");
            public static UIImage CloseX => UIImage.FromBundle("CloseX");
        }
    }
}
