﻿using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Shell.Interop;

namespace VisualStudio.GitStashExtension.Services
{
    public class FileIconsService
    {
        private readonly IVsImageService2 _vsImageService;

        public FileIconsService(IVsImageService2 vsImageService)
        {
            _vsImageService = vsImageService;
        }

        public BitmapSource GetFileIcon(string fileExtension)
        {
            try
            {
                var image = _vsImageService.GetIconForFileEx(fileExtension, __VSUIDATAFORMAT.VSDF_WPF, out var _) as WpfPropertyValue;
                return image?.Value as BitmapSource;
            }
            catch (System.Exception)
            {
                var image = _vsImageService.GetIconForFileEx(".txt", __VSUIDATAFORMAT.VSDF_WPF, out var _) as WpfPropertyValue;
                return image?.Value as BitmapSource;
            }
        }

        public BitmapSource GetFolderIcon(bool isExpanded)
        {
            var folderImageMoniker = isExpanded ? KnownMonikers.FolderOpened : KnownMonikers.FolderClosed;

            var vsImage = _vsImageService.GetImage(folderImageMoniker,
                new ImageAttributes
                {
                    StructSize = Marshal.SizeOf(typeof(ImageAttributes)),
                    ImageType = (uint) _UIImageType.IT_Bitmap,
                    Format = (uint) _UIDataFormat.DF_WPF,
                    LogicalWidth = 16,
                    LogicalHeight = 16,
                    Background = 0xFFFFFFFF,
                    Flags = (uint) _ImageAttributesFlags.IAF_RequiredFlags |
                            unchecked((uint) _ImageAttributesFlags.IAF_Background)
                });
            var image = vsImage as WpfPropertyValue;

            return image?.Value as BitmapSource;
        }
    }
}
