using System;

namespace electroweb.DTO
{
    public class FotoViewModel
    {
         
        public string Titulo{ get; set; }
        public string Descripcion { get; set; }
        public string Ruta { get; set; }
        public DateTime FechaCreacion{ get; set; }
        public string Hora{ get; set; }
        public byte[] ImageArray { get; set; }

        //Relaciones 
        public long? Novedad_Id { get; set; }   
        public long Elemento_Id { get; set; }  
    }
}