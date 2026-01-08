using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FonYonetim.Concrete
{
    public class BistDolar
    {
        [Key]
        public int BistMarketID { get; set; }

        [DisplayName("Sembol")]
        public string BistMarketSymbol { get; set; }

        [DisplayName("Güncel Fiyat Dolar")]
        public double BistMarketPriceDolar { get; set; }

        [DisplayName("Hedef Fiyat Dolar")]
        public double BistMarketTargetPriceDolar { get; set; }

        [DisplayName("Hedef Fiyata Kalan Yüzde")]
        public double BistMarketTargetPricePercent { get; set; }
    }
}
