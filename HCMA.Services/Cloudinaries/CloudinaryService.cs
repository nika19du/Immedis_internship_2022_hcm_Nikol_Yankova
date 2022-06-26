using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet;
using System.IO;
using CloudinaryDotNet.Actions;

namespace HCMA.Services.Cloudinaries
{
    public class CloudinaryService : ICloudinaryService
    {

        private readonly Cloudinary cloudinaryUtility;

        public CloudinaryService(Cloudinary cloudinaryUtility)
        {
            this.cloudinaryUtility = cloudinaryUtility;
        }

        public async Task<string> UploadPictureAsync(IFormFile pictureFile, string fileName)
        {
            if (pictureFile == null) return null;
            byte[] destinationData;

            using (var ms = new MemoryStream())
            {
                await pictureFile.CopyToAsync(ms);
                destinationData = ms.ToArray();
            }

            UploadResult uploadResult = null;

            using (var ms = new MemoryStream(destinationData))
            {
                ImageUploadParams uploadParams = new ImageUploadParams
                {
                    Folder = "HCMA_images",
                    File = new FileDescription(fileName, ms)
                };

                uploadResult = this.cloudinaryUtility.Upload(uploadParams);
            }
            return uploadResult?.SecureUri.AbsoluteUri;
        }
    }
}
