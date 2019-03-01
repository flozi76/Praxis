using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Duftfinder.Web.Helpers
{
	/// <summary>
	///     Is helper for all kind of conversions.
	/// </summary>
	/// <author>Anna Krebs</author>
	public class ConversionHelper
	{
		/// <summary>
		///     Resizes the uploaded picture & generates a base 64 string.
		/// </summary>
		/// <param name="uploadFile"></param>
		/// <author>Anna Krebs</author>
		/// <returns></returns>
		public async Task<string> ResizeAndGenerateBase64StringForPicture(IFormFile uploadFile)
		{
			// Resize uploaded picture.
			//Image picture = new WebImage(uploadFile.InputStream);
			IImageDecoder decoder = new JpegDecoder();
			var picture = Image.Load(uploadFile.OpenReadStream(), decoder);
			//if (picture.Width > 700)
			//{
			picture.Mutate(x =>
					x.Resize(700, 400)
				//.Grayscale()
			); //  .Resize(700, 400, true);
			//}

			Stream outStream = new MemoryStream();
			picture.Save(outStream, new JpegEncoder());
			await outStream.FlushAsync();
			outStream.Position = 0;

			// Get bytes from the resized picture.
			var binaryReader = new BinaryReader(outStream);
			var pictureAsBytes = binaryReader.ReadBytes((int) outStream.Length);

			// Convert bytes of picture data to base 64 string.
			var pictureDataAsString = Convert.ToBase64String(pictureAsBytes);

			return pictureDataAsString;
		}
	}
}