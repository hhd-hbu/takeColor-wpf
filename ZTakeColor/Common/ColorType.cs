using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZTakeColor.Common
{
    public partial class ColorType: ObservableObject
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        [ObservableProperty]
        private string _code;
        public int CodeType { get; set; }
    }
}
