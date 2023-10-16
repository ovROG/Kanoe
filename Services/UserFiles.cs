namespace Kanoe2.Services
{
    public class UserFiles
    {
        public async Task GetLocalFile(HttpContext contex, string path)
        {
            string Path = Directory.GetCurrentDirectory() + path;

            if (File.Exists(Path))
            {
                await contex.Response.SendFileAsync(Path);
            }
            else
            {
                await contex.Response.WriteAsync("");
            }
        }
    }
}
