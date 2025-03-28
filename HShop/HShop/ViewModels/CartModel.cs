namespace HShop.ViewModels
{
    public class CartModel
    {
        public int Quantity { get; set; }

        public double Total { get; set; }

        public List<CartItem> Items { get; set;}

    }
}
