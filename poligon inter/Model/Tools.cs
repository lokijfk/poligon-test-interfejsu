using System.IO;
using System.Reflection;
using System.Security.Cryptography;


namespace poligon_inter.Model;
// zmienić na PathHelper
static internal class Tools
{
    //private static ModelPB model = null;
    public static string GetUserAppDataPath
    {
        get { 
        //string sProductName = Assembly.GetExecutingAssembly().GetName().Name.ToString();
        return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\" +
            GetProjectName;
        //sProductName;
        //return string.Empty;
        }
    }

    public static string GetProjectName
    {
        //string sProductName = Assembly.GetExecutingAssembly().GetName().Name.ToString();
        get {
            return Assembly.GetExecutingAssembly().GetName().Name.ToString();
        }
        
        //sProductName;
        //return string.Empty;
    }

    /// <summary>
    /// do wywalenia - przenieść do BrokeraINI
    /// Zwraca plik imi o podanej nazwie umieszczony w {user}\AppData\Local\{AppName}
    /// jeżeli nie ma pliko to tworzynowy
    /// Może zwrócić plik znajdujący się w aktualnym katalogu aplikacji o ile wcześniej został tam utworzony
    /// </summary>
    /// <param name="inis"></param>
    /// <returns>zwraca obiekt IniFile lub null</returns>
    static public IniFile? LoadIni(string inis)
    {
        IniFile ini = null;
        if (File.Exists(Directory.GetCurrentDirectory() + "\\" + inis))
        {
            ini = new IniFile(Directory.GetCurrentDirectory() + "\\" + inis);
            //MessageBox.Show("jest w current");
        }
        else
        {
            string localpath = GetUserAppDataPath;
            ini = new IniFile(localpath + "\\" + inis);
        }
        return ini;
    }

    static public IniFile? LoadIniP(string inis,string postfix= ".ini")
    {
        return Tools.LoadIni(inis + postfix);
    }

    static public IniFile? LoadIniProject()
    {
        return Tools.LoadIniP(Tools.GetProjectName);
    }

    //to będzie do wywalenia
    static public DBSQLite? GetDataBase(string inis = null)
    {
        //trzeba tworzyć katalog o ile go nie ma i nie ma potrzeby sprawdzania czy jest
        // jak wczesniej jest odczytywany ini to on tworz katalog
        if (inis == null) inis = GetProjectName;
        System.IO.Directory.CreateDirectory(GetUserAppDataPath);
        DBSQLite DB = new(GetUserAppDataPath, inis);
        return DB;
    }

    static public string CalculateMD5Sting(string input)
    {

        // Use input string to calculate MD5 hash
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            return Convert.ToHexString(hashBytes); // .NET 5 +
        }
    }
    static public string CalculateMD5(string filename)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(filename))// tu sprawdzanie zrobić sprawdzanie czy do pliku jest dostęp jak nie ma to wyjątek
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                //return Encoding.Default.GetString(md5.ComputeHash(stream));// tu wychodzą jakieś haszcze
                //return md5.ComputeHash(stream).ToString();
            }
        }
    }

    static private string Prdouble(double size)
    {
        double kb = 0.0;
        string og = string.Empty;
        if (size > 1000)
        {
            kb = size / 1024;
            og = " KB";
        }
        if (kb > 1000)
        {
            kb = kb / 1024;
            og = " MB";
        }
        if (kb > 1000)
        {
            kb = kb / 1024;
            og = " GB";
        }
        return kb.ToString("F2") + og;
        //return string.Empty;
    }
}

