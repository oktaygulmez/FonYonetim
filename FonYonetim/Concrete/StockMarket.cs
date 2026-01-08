using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace FonYonetim.Concrete
{
    public class StockMarket
    {
        [Key]
        public int StockMarketID { get; set; }

        [DisplayName("Sembol")]
        public string StockMarketSymbol { get; set; }
       
        [DisplayName("Güncel Fiyat")]
        public double StockMarketPrice { get; set; }

        [DisplayName("Hedef Fiyat")]
        public double StockMarketTargetPrice { get; set; }

        [DisplayName("Hedef Fiyata Kalan Yüzde")]
        public double StockMarketTargetPricePercent { get; set; }
    }
}
