using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

public static class LocalCartService
{
    private static Dictionary<int, int> _localCart = new Dictionary<int, int>(); // productId -> amount
    private static string _filePath = "local_cart.xml";

    static LocalCartService()
    {
        LoadLocalCart();
    }

    public static void AddToLocalCart(int productId, int amount)
    {
        if (_localCart.ContainsKey(productId))
        {
            _localCart[productId] += amount;
        }
        else
        {
            _localCart[productId] = amount;
        }

        SaveLocalCart();
    }

    public static Dictionary<int, int> GetLocalCart()
    {
        return new Dictionary<int, int>(_localCart);
    }

    public static void ClearLocalCart()
    {
        _localCart.Clear();
        SaveLocalCart();
    }

    public static void RemoveFromLocalCart(int productId)
    {
        if (_localCart.ContainsKey(productId))
        {
            _localCart.Remove(productId);
            SaveLocalCart();
        }
    }

    private static void SaveLocalCart()
    {
        try
        {
            var serializer = new XmlSerializer(typeof(List<LocalCartItem>));
            var items = new List<LocalCartItem>();

            foreach (var kvp in _localCart)
            {
                items.Add(new LocalCartItem { ProductId = kvp.Key, Amount = kvp.Value });
            }

            using (var writer = new StreamWriter(_filePath))
            {
                serializer.Serialize(writer, items);
            }
        }
        catch
        {
            // Игнорируем ошибки сохранения
        }
    }

    private static void LoadLocalCart()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                var serializer = new XmlSerializer(typeof(List<LocalCartItem>));

                using (var reader = new StreamReader(_filePath))
                {
                    var items = (List<LocalCartItem>)serializer.Deserialize(reader);

                    _localCart.Clear();
                    foreach (var item in items)
                    {
                        _localCart[item.ProductId] = item.Amount;
                    }
                }
            }
        }
        catch
        {
            // Игнорируем ошибки загрузки
        }
    }

    [Serializable]
    public class LocalCartItem
    {
        public int ProductId { get; set; }
        public int Amount { get; set; }
    }
}