
using DotVVM.BusinessPack.Export.Csv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotVVM.BusinessPack.Controls;
using DotVVM.Framework.Controls;
using DotVVM.Framework.ViewModel;
namespace electroweb.ViewModels
{
    public class ViewModelCustomer: BaseViewModel
    {
         public bool IsEditing { get; set; }
         public GridViewUserSettings UserSettings { get; set; }
        public BpGridViewDataSet<Customer> Customers { get; set; }

       
        public override Task Init()
        {
            Customers = new BpGridViewDataSet<Customer> {
                OnLoadingData = GetData,
                RowEditOptions = new RowEditOptions {
                    PrimaryKeyPropertyName = nameof(Customer.Id),
                    EditRowId = -1
                }
            };
            Customers.SetSortExpression(nameof(Customer.Id));
            UserSettings = new GridViewUserSettings {
                EnableUserSettings = true,
                ColumnsSettings = new List<GridViewColumnSetting> {
                    new GridViewColumnSetting {
                        ColumnName = "CustomerId",
                        DisplayOrder = 0,
                        ColumnWidth = 50
                    },
                    new GridViewColumnSetting {
                        ColumnName = "CustomerName",
                        DisplayOrder = 1,
                        ColumnWidth = 400
                    },
                    new GridViewColumnSetting {
                        ColumnName = "CustomerBirthdate",
                        DisplayOrder = 2
                    },
                    new GridViewColumnSetting {
                        ColumnName = "CustomerOrders",
                        DisplayOrder = 3
                    }
                }
            };
            return base.Init();
        }

        public void EditCustomer(Customer customer)
        {
            Customers.RowEditOptions.EditRowId = customer.Id;
            IsEditing = true;
        }

        public void UpdateCustomer(Customer customer)
        {
            // Submit customer changes to your database..
            CancelEdit();
        }

        private void CancelEdit()
        {
            Customers.RowEditOptions.EditRowId = -1;
            IsEditing = false;
        }

        public void CancelEditCustomer()
        {
            CancelEdit();
            Customers.RequestRefresh(true);
        }

        public GridViewDataSetLoadedData<Customer> GetData(IGridViewDataSetLoadOptions gridViewDataSetOptions)
        {
            var queryable = GetQueryable(15);
            return queryable.GetDataFromQueryable(gridViewDataSetOptions);
        }

        private IQueryable<Customer> GetQueryable(int size)
        {
            var numbers = new List<Customer>();
            for (var i = 0; i < size; i++)
            {
                numbers.Add(new Customer { Id = i + 1, Name = $"Customer {i + 1}", BirthDate = DateTime.Now.AddYears(-i), Orders = i });
            }
            return numbers.AsQueryable();
        }
         public void Export()
        {
                var exporter = new GridViewExportCsv<Customer>(new GridViewExportCsvSettings<Customer> { Separator = ";" });
                var gridView = Context.View.FindControlByClientId<DotVVM.BusinessPack.Controls.GridView>("data", true);

                using (var file = exporter.Export(gridView,  Customers))
                {
                    Context.ReturnFile(file, "Report.csv", "text/csv");
                //Context.ReturnFile(file, "export.pdf", "application/pdf");
                }
        }

    }
}