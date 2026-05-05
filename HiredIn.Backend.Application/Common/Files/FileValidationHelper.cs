namespace HiredIn.Backend.Application.Common.Files
{
    public static class FileValidationHelper
    {
        private static readonly string[] ImageContentTypes =
        {
            "image/jpeg",
            "image/png",
            "image/webp"
        };

        private static readonly string[] ResumeContentTypes =
        {
            "application/pdf",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
        };

        public static void ValidateImage(string contentType, long size)
        {
            const long maxSize = 5 * 1024 * 1024;

            if (!ImageContentTypes.Contains(contentType))
                throw new InvalidOperationException("Only jpeg, png and webp images are allowed.");

            if (size > maxSize)
                throw new InvalidOperationException("Image size must be less than 5 MB.");
        }

        public static void ValidateResumeFile(string contentType, long size)
        {
            const long maxSize = 10 * 1024 * 1024;

            if (!ResumeContentTypes.Contains(contentType))
                throw new InvalidOperationException("Only pdf and docx files are allowed.");

            if (size > maxSize)
                throw new InvalidOperationException("Resume file size must be less than 10 MB.");
        }
    }
}