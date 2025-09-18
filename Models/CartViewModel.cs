// Models/ViewModels/CartViewModel.cs
using System.Collections.Generic;
using System.Linq;

namespace OnlineStoreMVC.Models.ViewModels
{
  public class CartViewModel
  {
    public List<CartLineItem> Items { get; set; } = new List<CartLineItem>();

    public decimal Total => Items.Sum(item => item.SubTotal);
  }
}