using System.Text;

namespace HShop.Helpers
{
    public class MyUtil
    {

        public static string UpLoadHinh(IFormFile Hinh, string folder)
        {
            try
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", folder, Hinh.FileName);
                using (var myFile = new FileStream(fullPath, FileMode.CreateNew))
                {
                    Hinh.CopyTo(myFile);
                }
                return Hinh.FileName;
            }
            catch (Exception e)
            {
                return String.Empty;
            }
        }
        public static string GenerateRandomKey(int length = 5)
        {
            var pattern = @"qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM,./;'[]!@#$%^&*()";
            var sb = new StringBuilder();
            var rd = new Random();
            for(int i = 0; i < length; i++) 
            {
                sb.Append(pattern[rd.Next(0, pattern.Length)]);
            }
            return sb.ToString();
        }
    }
}
