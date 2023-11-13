namespace Kanoe.Services
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

        public void ClearTempFolder()
        {
            try
            {
                Directory.Delete(Directory.GetCurrentDirectory() + @"\UserData\temp", true);
            }
            catch
            {
                Console.WriteLine("UNABLE TO CLEAR TEMP FOLDER");
            }
        }
    }
}
