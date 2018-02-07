using AutoMapper;
using Electro.model;//.ViewModels;
using Electro.model.datatakemodel;
using Electro.model.Models.datatakemodel;
using electroweb.DTO;
using electroweb.Model_Response;

public class MappingProfile : Profile {
    public MappingProfile() {
        // Add as many of these lines as you need to map your objects
       
        //CreateMap<EmpresaViewModel, Empresa>();
        CreateMap<Empresa, EmpresaDto>();
         CreateMap<EmpresaDto, Empresa>();
         CreateMap<Elemento, ElementoViewModel>();
         CreateMap<ElementoViewModel, Elemento>();
         CreateMap<Foto, FotoViewModel>();
         CreateMap<FotoViewModel, Foto>();

        CreateMap<ResponseCiudadEmpresa, Ciudad_Empresa>();
        CreateMap<Ciudad_Empresa, ResponseCiudadEmpresa>();

        CreateMap<ResponseCiudad, Ciudad>();
        CreateMap<Ciudad, ResponseCiudad>();

         CreateMap<ResponseEmpresa, Empresa>();
        CreateMap<Empresa, ResponseEmpresa>();


        CreateMap<EquipoViewModel, EquipoElemento>();
        CreateMap<EquipoElemento, EquipoViewModel>();

        
        CreateMap<ElementoCableViewModel, ElementoCable>();
        CreateMap<ElementoCable, ElementoCableViewModel>();


         CreateMap<ElementoReportViewModel, View_Cable_Report>();
        CreateMap<View_Cable_Report, ElementoReportViewModel>();

        CreateMap<ElementoReportViewModel, View_Elemento_Report>();
        CreateMap<View_Elemento_Report, ElementoReportViewModel>();

         CreateMap<ElementoReportViewModel, View_Novedad_Elemento_Report>();
        CreateMap<View_Novedad_Elemento_Report, ElementoReportViewModel>();

        //Elemento Report

    
    }
}