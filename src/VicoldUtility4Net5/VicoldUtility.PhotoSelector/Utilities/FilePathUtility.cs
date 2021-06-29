using System.Linq;
using VicoldUtility.PhotoSelector.Entities;

namespace VicoldUtility.PhotoSelector.Utilities
{
    internal static class FilePathUtility
    {
        public static string[] PresetExtensionCanReview = { ".jpg", ".png", ".bmp" };
        public static string[] PresetExtensionCannotReview = { ".cr3", ".raw", ".dng" };

        public static ImageFileType ExtensionImageType(string extension)
        {
            extension = extension.ToLower();
            if (PresetExtensionCanReview.Count(v => v == extension) > 0)
            {
                return ImageFileType.CanReview;
            }
            else if (PresetExtensionCannotReview.Count(v => v == extension) > 0)
            {
                return ImageFileType.CannotReview;
            }

            return ImageFileType.Unimage;
        }
    }
}
