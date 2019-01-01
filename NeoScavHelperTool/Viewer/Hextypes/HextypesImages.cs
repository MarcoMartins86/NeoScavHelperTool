using NeoScavModHelperTool;
using System;
using System.Collections.Generic;
using System.Globalization;
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

            //TODO: if for some reason in the future someone makes big hextypes images the random string will need to be written with rendered pixels measures instead of hard coded values
            BitmapSource tile = App.CopyImageRectWithDpi(image, rect, App.I.DpiX, App.I.DpiY);
            //this is a special tile, in-game it just randomizes the tile, so let's add text saying that
            if (hextypes_id == 4)
            {
                DrawingVisual visual = new DrawingVisual();                
                using (DrawingContext drawingContext = visual.RenderOpen())
                {                    
                    drawingContext.DrawImage(tile, new Rect(0, 0, tile.PixelWidth, tile.PixelHeight));
                    SolidColorBrush semiTransBrush = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0));
                    drawingContext.DrawRoundedRectangle(semiTransBrush, null, new Rect(10, 34, 80, 22), 5, 5);
                    FormattedText text = new FormattedText("Random", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Segoe UI"), 20.0, Brushes.Red);
                    drawingContext.DrawText(text, new Point(14,30));
                }
                RenderTargetBitmap mergedImage = new RenderTargetBitmap(tile.PixelWidth, tile.PixelHeight, image.DpiX, image.DpiY, PixelFormats.Pbgra32);
                mergedImage.Render(visual);
                tile = App.ConvertImageDpi(mergedImage, App.I.DpiX, App.I.DpiY);
            }

            if (need_upscale)
                return new TransformedBitmap(tile, new ScaleTransform(2, 2));
            else
                return tile;
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
