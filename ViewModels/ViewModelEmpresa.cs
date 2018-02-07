using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DotVVM.BusinessPack.Controls;
using DotVVM.BusinessPack.Export.Csv;
using DotVVM.Framework.Controls;
using Electro.model.DataContext;
using Electro.model.datatakemodel;
using Electro.model.Repository;
using electroweb.DTO;

namespace electroweb.ViewModels
{
    public class ViewModelEmpresa:BaseViewModel
    {
        // var context = serviceProvider.GetRequiredService<MyAppContext>();
          // private readonly MyAppContext context;
             private readonly IEmpresaRepository _IEmpresaRepository;
            public bool IsEditing { get; set; }
            public GridViewUserSettings UserSettings { get; set; }
            public BpGridViewDataSet<EmpresaDto> Empresas { get; set; }
            public BpGridViewDataSet<Empresa> Empresaexport { get; set; }
            public bool IsInserting { get; set; }
            private readonly IMapper _mapper;
         public ViewModelEmpresa(IMapper mapper, IEmpresaRepository IEmpresaRepository){
                //Mapper
                 _mapper = mapper;
                _IEmpresaRepository=IEmpresaRepository;
               // context=ctx;
        }
        public override Task Init()
        {
            Empresas = new BpGridViewDataSet<EmpresaDto> {
                OnLoadingData = GetData,
                RowEditOptions = new RowEditOptions {
                    PrimaryKeyPropertyName = nameof(Empresa.Id),
                    EditRowId = -1
                }
            };


            Empresas.SetSortExpression(nameof(Empresa.Id));
           UserSettings = new GridViewUserSettings {
                EnableUserSettings = true,
                
                ColumnsSettings = new List<GridViewColumnSetting> {
                    new GridViewColumnSetting {
                        ColumnName = "Id",
                        DisplayOrder = 0,
                        ColumnWidth = 50
                    },
                    new GridViewColumnSetting {
                        ColumnName = "Nombre",
                        DisplayOrder = 1,
                        ColumnWidth = 400
                    },
                    new GridViewColumnSetting {
                        ColumnName = "Direccion",
                        DisplayOrder = 2
                    },
                    new GridViewColumnSetting {
                        ColumnName = "Telefono",
                        DisplayOrder = 3
                    },
                    new GridViewColumnSetting {
                        ColumnName = "Nit",
                        DisplayOrder = 4
                    },
                    new GridViewColumnSetting {
                        ColumnName = "Is_Operadora",
                        DisplayOrder = 5
                    },
                     new GridViewColumnSetting {
                        ColumnName = "Edit",
                        
                        DisplayOrder = 6
                    },
                  
                    
                }
            };
            return base.Init();
        }

        public void EditEmpresa(EmpresaDto empresa)
        {
            Empresas.RowEditOptions.EditRowId = empresa.Id;
            IsEditing = true;
        }
       /* public async Task<EmpresaDto> getEmpresa(EmpresaDto empresa){
         
            
            var modeedit=await _IEmpresaRepository.GetSingleAsync(m=>m.Id==empresa.Id);
            var model = _mapper.Map<Empresa, EmpresaDto>(modeedit);
            return model;
        }*/
        public async Task UpdateEmpresa(EmpresaDto empresa)
        {
           // var modeledit =await getEmpresa(empresa);
           //context.Empresa.Update(empresaedit);
           // var model = _mapper.Map<EmpresaDto, Empresa>(modeledit);
           // model=modeledit;
            var modeedit=await _IEmpresaRepository.GetSingleAsync(m=>m.Id==empresa.Id);
           //var model = _mapper.Map<Empresa, EmpresaDto>(modeedit);
           if(modeedit!=null){
             
                modeedit.Nit=empresa.Nit;
                modeedit.Nombre=empresa.Nombre;
                modeedit.Telefono=empresa.Telefono;
                modeedit.Direccion=empresa.Direccion;
               // lecturaedit.Id=id;*/
               
               await  _IEmpresaRepository.EditAsync(modeedit);
               
                }
            CancelEdit();
        }

        private void CancelEdit()
        {
            Empresas.RowEditOptions.EditRowId = -1;
            IsEditing = false;
        }

        public void CancelEditEmpresa()
        {
            CancelEdit();
            Empresas.RequestRefresh(true);
        }

        public GridViewDataSetLoadedData<EmpresaDto> GetData(IGridViewDataSetLoadOptions gridViewDataSetOptions)
        {
            var queryable = GetQueryable(15).Result;
            return queryable.GetDataFromQueryable(gridViewDataSetOptions);
        }
         public GridViewDataSetLoadedData<Empresa> GetData2(IGridViewDataSetLoadOptions gridViewDataSetOptions)
        {
            var queryable2 = GetQueryable2(15).Result;
            return queryable2.GetDataFromQueryable(gridViewDataSetOptions);
        }

        private async Task<IQueryable<EmpresaDto>> GetQueryable(int size)
        {
          //  var numbers = new List<Empresa>();
            // for (var i = 0; i < size; i++)
            // {
            //     numbers.Add(new Customer { Id = i + 1, Name = $"Customer {i + 1}", BirthDate = DateTime.Now.AddYears(-i), Orders = i });
            // }
            var empresa= await _IEmpresaRepository.GetAllAsync();
             var model = _mapper.Map<IEnumerable<Empresa>, IEnumerable<EmpresaDto>>(empresa);
        
            //empresa.as
            return  model.AsQueryable();
        }
        private async Task<IQueryable<Empresa>> GetQueryable2(int size)
        {
          //  var numbers = new List<Empresa>();
            // for (var i = 0; i < size; i++)
            // {
            //     numbers.Add(new Customer { Id = i + 1, Name = $"Customer {i + 1}", BirthDate = DateTime.Now.AddYears(-i), Orders = i });
            // }
            var empresa= await _IEmpresaRepository.GetAllAsync();
            // var model = _mapper.Map<IEnumerable<Empresa>, IEnumerable<EmpresaDto>>(empresa);
        
            //empresa.as
            return  empresa.AsQueryable();
        }
        //exportar datos a excel
         public void Export()
        {
             
                     var exporter = new GridViewExportCsv<EmpresaDto>(new GridViewExportCsvSettings<EmpresaDto> { Separator = ";" });
                var gridView = Context.View.FindControlByClientId<DotVVM.BusinessPack.Controls.GridView>("data", true);
              
                using (var file = exporter.Export(gridView,  this.Empresas))
                {
                   // Context.ReturnFile(file, "Report.csv", "text/csv");
                Context.ReturnFile(file, "export.pdf", "application/pdf");
                }
        }
        public void InsertNewCustomer()
        {
            Empresas.RowInsertOptions.InsertedItem = new EmpresaDto {
                Id = Empresas.Items.Max(c => c.Id) + 1,
                Telefono="0"
            };
            IsInserting = true;
        }

        public void CancelInsertNewCustomer()
        {
            Empresas.RowInsertOptions.InsertedItem = null;
            IsInserting = false;
        }

        public void SaveNewCustomer()
        {
            // Save inserted item to database
            Empresas.Items.Add(Empresas.RowInsertOptions.InsertedItem);
            CancelInsertNewCustomer();
        }
        
    }
}