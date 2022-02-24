using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SixLabors.ImageSharp;
using System.IO;
using System.Threading.Tasks;
using SteganographyLibrary;
using SixLabors.ImageSharp.PixelFormats;
using System.Text;
using SixLabors.ImageSharp.Formats;
using SteganographyLibrary.Exceptions;
using System.Security.Cryptography;
using System;

namespace papyruscompendium.dev.Pages
{
	public class SteganographyModel : PageModel
	{
		[BindProperty]
		public string DecodedMessage { get; set; }

		[BindProperty]
		public string Message { get; set; }

		[BindProperty]
		public string Password { get; set; }

		[BindProperty]
		public IFormFile UploadedImage { get; set; }

		[BindProperty]
		public string UploadedImageSource { get; set; }

		public async Task OnPostAsync()
		{
			var action = Request.Query["Action"];
			if (string.IsNullOrEmpty(action) || UploadedImage is null)
			{
				return;
			}

			using var uploadStream = UploadedImage.OpenReadStream();
			var image = await Image.LoadAsync(uploadStream);

			if (action == "Encode")
			{
				try
				{
					var normalImage = new SteganographicImage(image.CloneAs<Rgba32>());
					var encodedImage = normalImage.EncodeDataInImage(Encoding.UTF8.GetBytes(Message), Password);
					UploadedImageSource = encodedImage.ToBase64String(SixLabors.ImageSharp.Formats.Png.PngFormat.Instance);
				}
				catch (CapacityException capacityException)
				{
					Message = capacityException.Message;
				}
				catch
				{
					UploadedImageSource = string.Empty;
					Message = "an error occured when encoding.";
				}
			}

			if (action == "Decode")
			{
				var encodedImage = new SteganographicImage(image.CloneAs<Rgba32>());
				try
				{
					var decodedData = encodedImage.GetEncodedDataFromImage(Password);
					DecodedMessage = Encoding.UTF8.GetString(decodedData);
				}
				catch (AesException AesException)
				{
					DecodedMessage = AesException.Message;
				}
				catch (CryptographicException cryptographicException)
				{
					DecodedMessage = cryptographicException.Message;
				}
				catch
				{
					DecodedMessage = "An error occured when decoding.";
					return;
				}

				UploadedImageSource = image.ToBase64String(SixLabors.ImageSharp.Formats.Png.PngFormat.Instance);
			}
		}

		public void OnGet()
		{

		}
	}
}