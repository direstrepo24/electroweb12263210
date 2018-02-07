using System;
using System.Collections.Generic;
using Electro.model.datatakemodel;

namespace electroweb.DTO
{
    public class ElementoViewModel
    {
        public long Id{ get; set; }
        public string CodigoApoyo{ get; set; }
        public long NumeroApoyo{ get; set; }
        public DateTime FechaLevantamiento{ get; set; }
        public string HoraInicio{ get; set; }
        public string HoraFin{ get; set; }
        public string ResistenciaMecanica{ get; set; }
        public long Retenidas{ get; set; }
        public double AlturaDisponible{ get; set; }


        //Arreglos Report
        public long EmpresaOperadora_Id{ get; set; }
        public string NombreEmpresaOperadora{ get; set; }

        public string Direccion{ get; set; }

        public string Coordenadas{ get; set; }

        public string NombreCiudad{ get; set; }

    
        public string Cantidad_Cable{ get; set; }

        public string ruta{get;set;}

        public long Ciudad_Id{ get; set; }

        //Cables
        public string Tipo_Cable{ get; set; }
        public string Detalle_Cable{ get; set; }

        public string SobreRbt{ get; set; }

        public string Tiene_Marquilla {get;set;}

        //Usuario
        public string Usuario {get;set;}
        
        public long Usuario_Id{ get; set; }

        //Properties Report Exel
        public string Tiene_Amplificador {get;set;}
        public string Tiene_Fuente {get;set;}
        public string Tiene_DistribuidorFibra {get;set;}
        public string ConectadoRbt{ get; set; }

         public string MedidorBt{ get; set; }

  

        public string Otro_Equipo{ get; set; }


        //Novedades
        public long Detalle_Tipo_Novedad_Id{ get; set; }
        public string Nombre_Detalle_Tipo_Novedad{ get; set; }
         public string Nombre_Tipo_Novedad{ get; set; }
        public string Descripcion_Novedad{ get; set; }
        public long Tipo_Novedad_id{ get; set; }

          
     
        //Equipos


        //Relaciones
        /* 
        public long Usuario_Id{ get; set; }
        public long Estado_id{ get; set; }
        public long Longitud_Elemento_Id{ get; set; }
        public long Material_Id{ get; set; }
        public long Proyecto_Id{ get; set; }
        public long Nivel_Tension_Id{ get; set; }
        public long Ciudad_Id{ get; set; }
*/
        //Relacion de 1
        public  Proyecto Proyecto{ get; set; }
      
        public  Material Material{ get; set; }
        public  LongitudElemento LongitudElemento{ get; set; }
        public  Estado Estado{ get; set; }
        public  NivelTensionElemento NivelTensionElemento{get;set;}

        public  List<FotoViewModel> Fotos{get;set;}

        public  List<ElementoCableViewModel> Cables{get;set;}

        public  List<EquipoViewModel> Equipos{get;set;}
        
    }
}