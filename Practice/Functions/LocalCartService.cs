using System.Collections.Generic;

public static class LocalCartService
{
    private static Dictionary<int, int> _localCart = new Dictionary<int, int>(); // productId -> amount

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
    }

    public static Dictionary<int, int> GetLocalCart()
    {
        return new Dictionary<int, int>(_localCart);
    }

    public static void ClearLocalCart()
    {
        _localCart.Clear();
    }
}