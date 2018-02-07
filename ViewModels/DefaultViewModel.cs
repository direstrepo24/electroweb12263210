using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;
using DotVVM.BusinessPack;
using Electro.model.Repository;
using Electro.model.datatakemodel;
using System.Globalization;
using System.Web;

namespace electroweb.ViewModels
{
    public class DefaultViewModel : BaseViewModel
    {
        private readonly IEstadoRepository _IestadoRepository;
         private readonly IElementoRepository _elementoRepository;
         private readonly IPerdidaRepository _perdidaRepository;
        public string Title { get; set; }

        #region 
        public string TotalPostes {get; set;}
        public string CiudadesInventariadas {get; set;}
        public string LongitudPostes {get; set;}

        //Perdidas
        public string ConexionesIlicitas {get; set;}
        public string LamparaEncendida {get; set;}
        public string LamparaAdicional {get; set;}
        public string RequierePoda {get; set;}

         

        #endregion


        public DefaultViewModel(IEstadoRepository IestadoRepository,
        IElementoRepository elementoRepository,
        IPerdidaRepository perdidaRepository)
        {
            _elementoRepository=elementoRepository;
            _IestadoRepository=IestadoRepository;
            _perdidaRepository= perdidaRepository;
            Title = "Panel Administrativo ELECTROHUILA";
            loadInit();
        }

      
        public bool IsEnabled { get; set; }
        public bool IsVisible { get; set; } = true;

        public void ShowButton()
        {
            
            var result= _IestadoRepository.GetAll();
            if(result.FirstOrDefault().Nombre!=null){
                    Title= result.FirstOrDefault().Nombre;
                    Title="Panal Admin";

            }
            
            IsEnabled = false;
            IsVisible = true;
            
        }

        public void HideButton()
        {
            IsVisible = false;
            IsEnabled = true;
        }

        private  void loadInit()
        {
           var countElementsRegister= elementosList().Result;
           var count= countElementsRegister.ToList().Count;
           // TotalPostes=count.ToString("N0");
            TotalPostes= string.Format("{0} postes",countElementsRegister.ToList().Count.ToString("N0"));

            var ciudadades= countElementsRegister.ToList().GroupBy(a=>a.Ciudad_Id);
            var i=0;
            foreach(var item in ciudadades){
                i=i+1;
                if(i==ciudadades.Count()){
                    CiudadesInventariadas+=string.Format("{0}",item.FirstOrDefault().Ciudad.Nombre);
                }else if(i>=1){
                    CiudadesInventariadas+=string.Format("{0},",item.FirstOrDefault().Ciudad.Nombre);
                }
            }
            //Cables.AsEnumerable().Sum(a=>a.Cantidad)
            var perdidas= perdidasList().Result;
            var conexiones_ilicitas= perdidas.ToList().Where(a=>a.Tipo_Perdida_Id==3 ).ToList();
            var lamparasencendidas= perdidas.ToList().Where(a=>a.Tipo_Perdida_Id==2).ToList();
            var requieren_poda= perdidas.ToList().Where(a=>a.Tipo_Perdida_Id==4).ToList();
            var lampara_adicional= perdidas.ToList().Where(a=>a.Tipo_Perdida_Id==1 ).ToList();

            var lampara_adicional_count_cant_0=lampara_adicional.Where(a=>a.Cantidad==0).Count();
            var conexiones_ilicitas_count_cant_0=conexiones_ilicitas.Where(a=>a.Cantidad==0).Count();
            var lamparasencendidas_count_cant_0=lamparasencendidas.Where(a=>a.Cantidad==0).Count();

          


            ConexionesIlicitas=string.Format("{0}",conexiones_ilicitas.AsEnumerable().Sum(a=>a.Cantidad)+conexiones_ilicitas_count_cant_0);
            LamparaEncendida=string.Format("{0}",lamparasencendidas.AsEnumerable().Sum(a=>a.Cantidad)+lamparasencendidas_count_cant_0);
            LamparaAdicional=string.Format("{0}",lampara_adicional.AsEnumerable().Sum(a=>a.Cantidad)+lampara_adicional_count_cant_0);

            RequierePoda=string.Format("{0}",requieren_poda.Count);

        }

        private async Task<IQueryable<Elemento>> elementosList(){
            IEnumerable<Elemento> elementos=Enumerable.Empty<Elemento>();
            elementos=await _elementoRepository.AllIncludingAsyncWhere(a=>a.Ciudad_Id!=658 && a.Ciudad.departmentoId==17 && a.Is_Enabled_Data==true,b=>b.Ciudad);
            return  elementos.AsQueryable();

        }

         private async Task<IQueryable<Perdida>> perdidasList(){
            IEnumerable<Perdida> elementos=Enumerable.Empty<Perdida>();
            elementos=await _perdidaRepository.AllIncludingAsyncWhere(a=>a.Response_Checked==true && a.Elemento.Ciudad_Id!=658 && a.Elemento.Ciudad.departmentoId==17  && a.Elemento.Is_Enabled_Data==true,b=>b.Elemento.Ciudad, c=>c.Elemento);
            return  elementos.AsQueryable();
        }
    }
}
