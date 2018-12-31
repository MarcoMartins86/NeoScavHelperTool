using NeoScavModHelperTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NeoScavHelperTool.Viewer.Hextypes
{
    public class HextypesImages
    {
        // Morning
        private BitmapSource _summerMorning;
        public BitmapSource SummerMorning => _summerMorning;
        private BitmapSource _summerMorningHighlighted;
        public BitmapSource SummerMorningHighlighted => _summerMorningHighlighted;
        // Day
        private BitmapSource _summerDay;
        public BitmapSource SummerDay => _summerDay;
        private BitmapSource _summerDayHighlighted;
        public BitmapSource SummerDayHighlighted => _summerDayHighlighted;
        // Dusk
        private BitmapSource _summerDusk;
        public BitmapSource SummerDusk => _summerDusk;
        private BitmapSource _summerDuskHighlighted;
        public BitmapSource SummerDuskHighlighted => _summerDuskHighlighted;
        // Night
        private BitmapSource _summerNight;
        public BitmapSource SummerNight => _summerNight;
        private BitmapSource _summerNightHighlighted;
        public BitmapSource SummerNightHighlighted => _summerNightHighlighted;

        private BitmapSource LoadHextypesTile(int hextypes_id, BitmapImage image, bool need_upscale, bool is_highlighted, int tile_width, int tile_height)
        {
            Int32Rect rect = new Int32Rect();

            //Hextypes images are 6 per 13 where odd rows are normal highlighted images and even the normal ones
            int nRowIndex = ((hextypes_id - 1) / 13) * 2;
            int nColumnIndex = (hextypes_id - 1) % 13;
            if (is_highlighted == false)
                nRowIndex += 1;                       
            rect.X = nColumnIndex * tile_width;
            rect.Y = nRowIndex * tile_height;
            rect.Width = tile_width;
            rect.Height = tile_height;

            if (need_upscale)
                return new TransformedBitmap(App.CopyImageRectWithDpi(image, rect, App.I.DpiX, App.I.DpiY), new ScaleTransform(2, 2));
            else
                return App.CopyImageRectWithDpi(image, rect, App.I.DpiX, App.I.DpiY);
        }

        private void InitBitmapSource(int hextypes_id, bool big_gui, string image_name, ref BitmapSource normal, ref BitmapSource highlighted)
        {
            string strImagePath = App.DB.GetImagePathFromMemory(image_name, "0_images", big_gui);
            bool bNeedToUpscale = false;
            int nTileWidth = 100;
            int nTileHeight = 76;
            if (string.IsNullOrEmpty(strImagePath) && big_gui)
            {
                // if big image doesn't exist let's use the small one and upscale it
                strImagePath = App.DB.GetImagePathFromMemory(image_name, "0_images", false);
                bNeedToUpscale = true;
            }
            // Altough this cannot be tested for now since game does not have big hextypes images, I will leave this here just in case 
            if (big_gui && !bNeedToUpscale)
            {
                nTileWidth = 200;
                nTileHeight = 152;
            }
            // Now fetch the image with all tiles and load the wanted tile
            BitmapImage bmpAllTiles = new BitmapImage(new Uri(strImagePath));
            normal = LoadHextypesTile(hextypes_id, bmpAllTiles, bNeedToUpscale, false, nTileWidth, nTileHeight);
            highlighted = LoadHextypesTile(hextypes_id, bmpAllTiles, bNeedToUpscale, true, nTileWidth, nTileHeight);
            // They are read only for now on, this also makes the images generated on background thread to work on the GUI thread
            normal.Freeze();
            highlighted.Freeze();
        }

        public HextypesImages(int hextypes_id, bool big_gui)
        {
            InitBitmapSource(hextypes_id, big_gui, "HexSheetSummerMorning", ref _summerMorning, ref _summerMorningHighlighted);
            InitBitmapSource(hextypes_id, big_gui, "HexSheetSummerDay", ref _summerDay, ref _summerDayHighlighted);
            InitBitmapSource(hextypes_id, big_gui, "HexSheetSummerDusk", ref _summerDusk, ref _summerDuskHighlighted);
            InitBitmapSource(hextypes_id, big_gui, "HexSheetSummerNight", ref _summerNight, ref _summerNightHighlighted);
        }
    }
}
