using System.Collections.Generic;

namespace Shop.Models
{
    public class Cart
    {
        public int Id { get; set; } 
        public Dictionary<int, int> Items { get; set; } = new Dictionary<int, int>();
        public string UserId { get; set; }
    }
}
