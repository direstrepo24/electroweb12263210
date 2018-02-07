using Electro.model.datatakemodel;
using Electro.model.Models.datatakemodel;

namespace electroweb.DTO
{
    public class ElementoCableViewModel
    {

        ///Atributes
        
       private  string tiene_marquilla;
       private  string sobreRbt;


        public long Id{ get; set; }
        public string Codigo{ get; set; }
        public long Cantidad{ get; set; }
        public string SobreRbt
        {
            get
            {
                return sobreRbt;
            }
            set
            {
                if (value=="False")
                {
                    sobreRbt = "NO";
                }else{
                    sobreRbt = "SI";
                }
                

            }
        }

       

        public string Tiene_Marquilla
        {
            get
            {
                return tiene_marquilla;
            }
            set
            {
                if (value=="False")
                {
                    tiene_marquilla = "NO";
                }else{
                    tiene_marquilla = "SI";
                }
            }
        }

        //Relaciones
        public long Empresa_Id{ get; set; }
        public string NombreEmpresaCable{ get; set; }
        
        public long Ciudad_Id{ get; set; }
        public long? Ciudad_Empresa_Id{ get; set; }

        public long DetalleTipocable_Id{ get; set; }
        public string TipoCable{ get; set; }
        public string DetalleCable{ get; set; }


        public long Elemento_Id{ get; set; }

         public  DetalleTipoCable DetalleTipoCable{ get; set; }
       
     
        public  Ciudad_Empresa Ciudad_Empresa{ get; set; }
    }
}