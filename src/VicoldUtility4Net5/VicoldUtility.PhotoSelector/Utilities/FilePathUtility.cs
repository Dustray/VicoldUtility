using System.Linq;
using VicoldUtility.PhotoSelector.Entities;

namespace VicoldUtility.PhotoSelector.Utilities
{
    internal static class FilePathUtility
    {
        private static string[] presetExtensionCanReview = { ".jpg", ".png", ".bmp" };
        private static string[] presetExtensionCannotReview = { ".cr3", ".raw", ".dng" };

        public static ImageFileType ExtensionImageType(string extension)
        {
            extension = extension.ToLower();
            if (presetExtensionCanReview.Count(v => v == extension) > 0)
            {
                return ImageFileType.CanReview;
            }
            else if (presetExtensionCannotReview.Count(v => v == extension) > 0)
            {
                return ImageFileType.CannotReview;
            }

            return ImageFileType.Unimage;
        }
    }
}
