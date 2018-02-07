using System;
using System.Collections.Generic;
using System.Linq;


namespace electroweb.DTO
{
    public class ElementoReportViewModel
    {

       // Atributes
       private  string tiene_marquilla;
       private  string sobreRbt;

      
    
        //Elemento Cable Properties
        public long Id { get; set; }

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

      //  public string Tiene_Marquilla { get; set; }
      //  public string SobreRbt { get; set; }

        public long Cantidad_Cable { get; set; }
        public long Empresa_Id { get; set; }
        public long DetalleTipocable_Id { get; set; }
        public long Ciudad_Empresa_Id { get; set; }

        //Cable

        public long Cable_Id { get; set; }

        public string Nombre_Cable { get; set; }

         //Tipo Cable
        public string Sigla_Cable { get; set; }

        public long Tipo_Cable_Id { get; set; }
        public string Nombre_Tipo_Cable { get; set; }

        //Empresa
        public string Nombre_Empresa { get; set; }

        public bool Empresa_Is_Operadora { get; set; }

        public string Nit_Empresa { get; set; }

        public string Direccion_Empresa { get; set; }

    
        ///Elemento Properties
        public long Elemento_Id { get; set; }

        public string CodigoApoyo { get; set; }

        public long NumeroApoyo { get; set; }

        public DateTime FechaLevantamiento { get; set; }

         public string FechaLevantamientoFormat { get; set; }

        public string HoraInicio { get; set; }
        
        public string Imei_Device { get; set; }

        public DateTime? Fecha_Sincronizacion {get; set;}
        public string Hora_Sincronizacion {get; set;}
        public string HoraFin { get; set; }

        public string ResistenciaMecanica { get; set; }
        public long Retenidas { get; set; }
        public double AlturaDisponible { get; set; }
        
        //Relaciones
        public long Ciudad_Id { get; set; }

        public long Usuario_Id { get; set; }

        public long Estado_id { get; set; }

        public long Longitud_Elemento_Id { get; set; }


        public long Material_Id { get; set; }

        public long Proyecto_Id { get; set; }

        public long Nivel_Tension_Id { get; set; }

        //Properties Others Tables
        public string Nombre_Estado { get; set; }
        public string Sigla_Estado { get; set; }
        public string Nombre_Material { get; set; }

        public string Sigla_Material { get; set; }

        public double Longitud { get; set; }

        public string Nombre_Nivel_Tension { get; set; }

        public string Sigla_Nivel_Tension { get; set; }

        public long Valor_Nivel_Tension { get; set; }

        public string Nombre_Proyecto { get; set; }

        public string Nombre_Usuario { get; set; }

         public string Usuario { get; set; }

        public string Apellido_Usuario { get; set; }

        public string Cedula_Usuario { get; set; }

        public string Ciudad { get; set; }

        public string Departamento { get; set; }

        public long Departamento_Id { get; set; }




        //Novedades
        //Properties Novedad
        public long Novedad_Id { get; set; }
        public string Nombre_Tipo_Novedad { get; set; }
        public string Nombre_Detalle_Tipo_Novedad { get; set; }
        public long Tipo_Novedad_id{ get; set; }
        public string Descripcion_Novedad { get; set; }
        public long Detalle_Tipo_Novedad_Id { get; set; }




          //Properties Report Exel
        public string Tiene_Amplificador {get;set;}
        public string Tiene_Fuente {get;set;}
        public string Tiene_DistribuidorFibra {get;set;}
        public string ConectadoRbt{ get; set; }

         public string MedidorBt{ get; set; }

         public string Otro_Equipo{ get; set; }

         //localizacion
        public string Coordenadas_Elemento { get; set; }

        public string Direccion_Elemento { get; set; }

        public decimal Latitud_Elemento { get; set; }

        public decimal Longitud_Elemento { get; set; }

        public string Direccion_Gps { get; set; }


        
        public  List<FotoViewModel> Fotos{get;set;}
        public  List<ElementoCableViewModel> Cables{get;set;}

        public  List<EquipoViewModel> Equipos{get;set;}
    }
}