using System.Collections.Generic;

namespace Shop.Models
{
    public class Cart
    {
        public Dictionary<int, int> Items { get; set; } = new Dictionary<int, int>();
    }
}
