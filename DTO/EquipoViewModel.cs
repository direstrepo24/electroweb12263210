using Electro.model.datatakemodel;
using Electro.model.Models.datatakemodel;

namespace electroweb.DTO
{
    public class EquipoViewModel
    {

        //Atributes
        private string conectado_rbt;
        private string medidorbt;


        public long Id{ get; set; }
        public string Codigo{ get; set; }
        public string Descripcion{ get; set; }
        public long Cantidad{ get; set; }

        public string ConectadoRbt
        {
            get
            {
                return conectado_rbt;
            }
            set
            {
                if (value=="False")
                {
                    conectado_rbt = "NO";
                }else{
                    conectado_rbt = "SI";
                }
            }
        }
       
        public string MedidorBt
        {
            get
            {
                return medidorbt;
            }
            set
            {
                if (value=="False")
                {
                    medidorbt = "NO";
                }else{
                    medidorbt = "SI";
                }
            }
        }




        public long Consumo{ get; set; }
        public string UnidadMedida{ get; set; }

        //Relaciones
        public long EmpresaId{ get; set; }
        public string NombreEmpresaEquipo{ get; set; }
        public long Ciudad_Id{ get; set; }

        public long? Ciudad_Empresa_Id{ get; set; }
        public long TipoEquipo_Id{ get; set; }
        public long Elemento_Id {get; set;}
        
        public  TipoEquipo TipoEquipo{ get; set; }
        
        public  Ciudad_Empresa Ciudad_Empresa{ get; set; }
    }
}