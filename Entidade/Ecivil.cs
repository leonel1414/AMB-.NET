using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidade
{
    public class Ecivil
    {
        public int Codeciv { get; set; }
        public string Desceciv { get; set; }

        public Ecivil(int Codeciv, string Desceciv)
        {
            this.Codeciv = Codeciv;
            this.Desceciv = Desceciv;
        }
    }
}
