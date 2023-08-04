namespace GrpcServicePiter
{
    static public class Settings
    {
        static public string GetConnectString(string path = "setting.txt")
        {
            string connect = "Host=localhost;Username=postgres;Password=12345;Database=PiterDB5"; // Второй вариант: "Server=localhost;port=643;Database=PiterDB3;Username=postgres;Password=12345;"
            if (string.IsNullOrEmpty(path))
                return connect;

            // чтение
            using (StreamReader reader = new StreamReader(path))
            {
                string text = reader.ReadLine();
                connect = string.IsNullOrWhiteSpace(text) ? connect : text;
            }
            // выход
            return connect;
        }
    }
}
