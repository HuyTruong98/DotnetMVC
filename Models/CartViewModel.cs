// Models/ViewModels/CartViewModel.cs
using System.Collections.Generic;
using System.Linq;

namespace OnlineStoreMVC.Models.ViewModels
{
  public class CartViewModel
  {
    public List<CartLineItem> Items { get; set; } = new();

    public decimal SubTotal => Items.Sum(i => i.SubTotal);

    public decimal ShippingFee => SubTotal < 300000 ? 20000 : 0;

    public decimal GrandTotal => SubTotal + ShippingFee;
  }

}