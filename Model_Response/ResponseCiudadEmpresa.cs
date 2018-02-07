namespace electroweb.Model_Response
{
    public class ResponseCiudadEmpresa
    {
        public long Id{ get; set; }
        //Relaciones
        public long Ciudad_Id {get; set; }
        public long Empresa_Id{ get; set; }

        public  ResponseCiudad Ciudad{ get; set; }
        
        public  ResponseEmpresa Empresa{ get; set; }
    }
}