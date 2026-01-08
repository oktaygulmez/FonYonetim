using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FonYonetim.Concrete
{
    public class Nasdaq
    {
        [Key]
        public int NasdaqMarketID { get; set; }

        [DisplayName("Sembol")]
        public string NasdaqMarketSymbol { get; set; }

        [DisplayName("Güncel Fiyat")]
        public double NasdaqMarketPrice { get; set; }

        [DisplayName("Hedef Fiyat")]
        public double NasdaqMarketTargetPrice { get; set; }

        [DisplayName("Hedef Fiyata Kalan Yüzde")]
        public double NasdaqMarketTargetPricePercent { get; set; }

        public DateTime LastUpdateDate { get; set; }
    }
}
