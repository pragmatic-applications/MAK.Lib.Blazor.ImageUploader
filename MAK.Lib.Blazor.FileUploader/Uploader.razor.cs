using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using HttpServices;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace MAK.Lib.Blazor.FileUploader
{
    public partial class Uploader
    {
        [Inject] public HttpImageUploaderService HttpImageUploaderService { get; set; }

        public string? ImgUrl { get; set; }
        public List<string> Images { get; set; } = new();

        [Parameter] public EventCallback<string> OnChange { get; set; }
        [Parameter] public string? AcceptTypeList { get; set; }
        [Parameter] public string? ToImageType { get; set; }
        [Parameter] public string? Css { get; set; }
        [Parameter] public bool Multiple { get; set; }

        private async Task HandleSelected(InputFileChangeEventArgs e)
        {
            var imageFiles = e.GetMultipleFiles();

            foreach(var imageFile in imageFiles)
            {
                if(imageFile != null)
                {
                    var resizedFile = await imageFile.RequestImageFileAsync($"image/{ToImageType}", 300, 500);

                    using var stream = resizedFile.OpenReadStream(resizedFile.Size);
                    var content = new MultipartFormDataContent();
                    content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
                    content.Add(new StreamContent(stream, Convert.ToInt32(resizedFile.Size)), "image", imageFile.Name);

                    this.ImgUrl = await this.HttpImageUploaderService.UploadAsync(content);
                    await this.OnChange.InvokeAsync(this.ImgUrl);

                    if(this.Multiple)
                    {
                        this.Images.Add(this.ImgUrl);
                    }
                }
            }
        }
    }
}
