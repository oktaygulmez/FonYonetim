using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FonYonetim.Concrete
{
    public class Bist
    {
        [Key]
        public int BistMarketID { get; set; }

        [DisplayName("Sembol")]
        public string BistMarketSymbol { get; set; }

        [DisplayName("Güncel Fiyat")]
        public double BistMarketPrice { get; set; }

        [DisplayName("Hedef Fiyat")]
        public double BistMarketTargetPrice { get; set; }

        [DisplayName("Hedef Fiyata Kalan Yüzde")]
        public double BistMarketTargetPricePercent { get; set; }
    }
}
