using System.Drawing.Imaging;


namespace JA_projekt_sem5 {
    public partial class Form1 : Form {
        private static Bitmap ConvertImageToBitmap(string imagePath) {
            using (Image image = Image.FromFile(imagePath)) {
                return new Bitmap(image);
            }
        }

        private static void DisplayImage(Bitmap picture, PictureBox picBox) {
            Bitmap img = picture;
            if (img.Width > picBox.Width || img.Height > picBox.Height) {
                img = ResizeImage(img, picBox.Width, picBox.Height);
            }

            picBox.Image = img;
            picBox.SizeMode = PictureBoxSizeMode.CenterImage;
        }

        private static Bitmap ResizeImage(Bitmap img, int maxWidth, int maxHeight) {

            float ratioX = (float)maxWidth / img.Width;
            float ratioY = (float)maxHeight / img.Height;
            float ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(img.Width * ratio);
            int newHeight = (int)(img.Height * ratio);

            Bitmap newImage = new Bitmap(newWidth, newHeight);
            using (Graphics graphics = Graphics.FromImage(newImage)) {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(img, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }

        public void SaveBitmapAsBmp(Bitmap bitmap, string filePath) {
            try {
                bitmap.Save(filePath, ImageFormat.Bmp);
                labelStatus.Text = $"Image was saved at: {filePath}";
            } catch (Exception ex) {
                labelStatus.Text = $"Image saving error: {ex.Message}";
            }
        }
    }
}
