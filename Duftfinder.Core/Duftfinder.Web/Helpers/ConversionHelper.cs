using System;

namespace Duftfinder.Web.Helpers
{
    /// <summary>
    /// Is helper for all kind of conversions.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class ConversionHelper
    {
        /// <summary>
        /// Resizes the uploaded picture & generates a base 64 string.
        /// </summary>
        /// <param name="uploadFile"></param>
        /// <author>Anna Krebs</author>
        /// <returns></returns>
        public string ResizeAndGenerateBase64StringForPicture(HttpPostedFileBase uploadFile)
        {
            // Resize uploaded picture.
            WebImage picture = new WebImage(uploadFile.InputStream);
            if (picture.Width > 700)
            {
                picture.Resize(700, 400, true);
            }

            // Get bytes from the resized picture.
            byte[] pictureAsBytes = picture.GetBytes();

            // Convert bytes of picture data to base 64 string.
            string pictureDataAsString = Convert.ToBase64String(pictureAsBytes);

            return pictureDataAsString;
        }
    }
}