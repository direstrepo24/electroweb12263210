using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using electroweb.DTO;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;
using PdfRpt.FluentInterface;

namespace electroweb.Reports.MasterReports
{
    public class ElementosPdfReport
    {


        public static byte[] CreateInMemoryPdfReport(string wwwroot,List<ElementoReportViewModel> ListElementos)
        {
            return CreateHtmlHeaderPdfReport(wwwroot,ListElementos,"","").GenerateAsByteArray(); // creating an in-memory PDF file
        }
       
		public static IPdfReportData CreateHtmlHeaderPdfReportStream(string wwwroot, Stream stream, List<ElementoReportViewModel> reportElementos, string date_start_report,string date_end_report)
        {
               return CreateHtmlHeaderPdfReport(wwwroot,reportElementos, date_start_report, date_end_report).Generate(data => data.AsPdfStream(stream, closeStream: false));
        }

        public  static PdfReport CreateHtmlHeaderPdfReport(String wwwroot,List<ElementoReportViewModel> reportElementos, string date_start_report,string date_end_report)
		{
			return new PdfReport().DocumentPreferences(doc =>
			{
				doc.RunDirection(PdfRunDirection.LeftToRight);
				
				doc.Orientation(PageOrientation.Portrait);
				doc.PageSize(PdfPageSize.A4);
				doc.DocumentMetadata(new DocumentMetadata { Author = "Vahid", Application = "PdfRpt", Keywords = "Test", Subject = "Test Rpt", Title = "Reporte de inventario" });
				doc.Compression(new CompressionSettings
				{
					EnableCompression = true,
					EnableFullCompression = true
				});
			})
			 .DefaultFonts(fonts =>
			 {
				fonts.Path(Path.Combine(wwwroot, "fonts", "verdana.ttf"),
                        Path.Combine(wwwroot, "fonts", "tahoma.ttf"));
				 fonts.Size(5);
				 fonts.Color(System.Drawing.Color.Black);

				 
			 })
			 .PagesFooter(footer =>
			 {
				 
				 footer.HtmlFooter(rptFooter =>
				 {
					 rptFooter.PageFooterProperties(new FooterBasicProperties
					 {
						 RunDirection = PdfRunDirection.LeftToRight,
						 ShowBorder = false,
						 PdfFont = footer.PdfFont,
						 TotalPagesCountTemplateHeight = 50,
						 TotalPagesCountTemplateWidth = 100
					 });
					 rptFooter.AddPageFooter(pageFooter =>
					 {
					    //var encabezado_inferior= Path.Combine(wwwroot, "Images","reporte_inferior.png");
						///var encabezado_inferior= TestUtils.GetImagePath("reporte_inferior.png");
						//var image = string.Format("<img  width='450'  src='{0}' />", encabezado_inferior);
						 // TotalPagesNumber is a custom image.
						 var page = string.Format("Page {0} Of <img src='TotalPagesNumber' />", pageFooter.CurrentPageNumber);
						 var date = DateTime.Now.ToString("MM/dd/yyyy");
						 /*return string.Format(@"<table style='margin-top:-20px !important;font-size:9pt;font-family:tahoma;'>
						                                <tr>
															<td align='center'>{0}</td>
													    </tr>
														<tr>
															<td width='50%' align='center'>{1}</td>
															<td width='50%' align='center'>{2}</td>
														 </tr>
												</table>",image, page, date);*/
						        return string.Format(@"<table  style='width: 100%;font-size:5pt;font-family:tahoma;' >
														<tr style=' font-weight: bold;' >
															<th style='color:#e26912;'>CONDICION</th>
															<th style='color:#e26912;'>MATERIAL</th>
															<th style='color:#e26912;'>ESTADO</th>
															<th style='color:#e26912;'>PROPIEDAD</th>
															<th style='color:#e26912;'>TIPO DE RED BT</th>
															<th style='color:#e26912;'>TIPO CABLE COMUNICACION</th>
														</tr>
														<tbody>
															<tr>
																<td>
																<span style='color:#e26912; font-weight: bold;'>E</span>  ENCONTRADO <br />
																<span style='color:#e26912; font-weight: bold;'>I </span>  INSTALADO<br />
																<span style='color:#e26912; font-weight: bold;'>R </span> RETIRADO<br />
																<span style='color:#e26912; font-weight: bold;'>C</span> CAMBIO
																</td>
																<td>
																<span style='color:#e26912; font-weight: bold;'>CO</span>  CONCRETO<br />
																<span style='color:#e26912; font-weight: bold;'>MA</span>   POSTE MADERA<br />
																<span style='color:#e26912; font-weight: bold;'>TO </span>  TORRE METAL<br />
																<span style='color:#e26912; font-weight: bold;'>TU </span> TUBO
																</td>
																<td>
																<span style='color:#e26912; font-weight: bold;'>B:  </span> BUENO<br />
																<span style='color:#e26912; font-weight: bold;'>M:</span> MALO
																</td>
																<td>
																<span style='color:#e26912; font-weight: bold;'>EH: </span>  Electrohuila<br />
																<span style='color:#e26912; font-weight: bold;'>GO:</span>   Gobernacion<br />
																<span style='color:#e26912; font-weight: bold;'>MC: </span>  Municipio
																</td>
																<td>
																<span style='color:#e26912; font-weight: bold;'>ACSR:</span>  Aluminio Desnudo<br />
																<span style='color:#e26912; font-weight: bold;'>ASC: </span> Aluminio Aislado<br />
																<span style='color:#e26912; font-weight: bold;'>TRE: </span>  Trenzado
																</td>
																<td>RG6<br />RG11<br />.500</td>
															</tr>
															<tr >
																<td colspan='3'>{0}</td>
																<td colspan='3' >{1}</td>
															</tr>
														</tbody>
													</table>", page, date);
					 });
				 });
			 })
			 .PagesHeader(header =>
			 {
				 header.HtmlHeader(rptHeader =>
				 {
					 rptHeader.PageHeaderProperties(new HeaderBasicProperties
					 {
						 RunDirection = PdfRunDirection.LeftToRight,
						 ShowBorder = true,
                         PdfFont = header.PdfFont
                     });
					 rptHeader.AddPageHeader(pageHeader =>
					 {
						//var message = "Reporte de cable de operadores.";
						//var photo = "http://54.86.105.4/Fotos/d978a65c-5071-4d21-9aac-8a3b81032ba7.jpg";

						var proyecto ="";
						if( reportElementos.Count>0){
							proyecto = reportElementos.FirstOrDefault().Nombre_Proyecto;
						}
						// var dateNow = DateTime.Now.ToString("MM/dd/yyyy");
						 var dateNow = string.Format("{0} a {1}",date_start_report,date_end_report);


						var encabezado= Path.Combine(wwwroot, "Images","logo_electrohuila.png");
						///var encabezado= TestUtils.GetImagePath("logo_electrohuila.png");
						var image = string.Format("<img  width='130'  src='{0}' />", encabezado);
						/* return string.Format(@"<table style='width: 100%;font-size:9pt;font-family:tahoma;'>
													<tr>
														<td align='center'>{0}</td>
													</tr>
													<!--<tr>
														<td align='center'>{1}</td>-->
													</tr>
												</table>", image, message);*/
												return string.Format(@"<table style='width: 100%;font-size:7pt;font-family:tahoma;'>
													<tr>
													<td rowspan='2' align='center'> 
															{0}
													</td>
													<td colspan='2' align='center'>
															<span style='padding:0px !important; color:#e26912; font-weight: bold;'>  
															INVENTARIO CABLE OPERADORES 
															</span>
													</td>
													</tr>
													<tr>
													<td> 
															<span style='color:#e26912; font-weight: bold;'>  FECHA(DD/MM/AAAA):  </span>{1}<br />
															<span style='color:#e26912; font-weight: bold;'>  CIUDAD DE EJECUCUION:  </span><br />
															<span style='color:#e26912; font-weight: bold;'>  PROYECTO:  </span>{2}<br />
													</td>
													<td> 
															<span style='color:#e26912; font-weight: bold;'> EMPRESA OPERADORA  </span><br />
															<span style='color:#e26912; font-weight: bold;'>  ORDEN DE TRABAJO  </span>01<br />
															<span style='color:#e26912; font-weight: bold;'>  VERSION:  </span>01<br />
													</td>
													</tr>
											</table>
										", image, dateNow,proyecto);



					 });

					 rptHeader.GroupHeaderProperties(new HeaderBasicProperties
					 {
						 RunDirection = PdfRunDirection.LeftToRight,
						 ShowBorder = true,
						 SpacingBeforeTable = 10f,
                         PdfFont = header.PdfFont
                     });
					 
                     
                     rptHeader.AddGroupHeader(groupHeader =>
					 {
						 var data = groupHeader.NewGroupInfo;


						 var id_empresa= long.Parse(data.GetSafeStringValueOf<ElementoReportViewModel>(x => x.Empresa_Id));
						
						 var postes = reportElementos.Where(a=>a.Empresa_Id==id_empresa).GroupBy(a=>a.Elemento_Id);
						 var postesLongitud = reportElementos.Where(a=>a.Empresa_Id==id_empresa);
						 var ocupacion_total= postesLongitud.AsEnumerable().Sum(a=>a.Cantidad_Cable);


						 var totallongitud6= postesLongitud.Where(a=>a.Longitud==6).AsEnumerable().Sum(a=>a.Cantidad_Cable);
						 var totallongitud8= postesLongitud.Where(a=>a.Longitud==8).AsEnumerable().Sum(a=>a.Cantidad_Cable);
						 var totallongitud12=postesLongitud.Where(a=>a.Longitud==12).AsEnumerable().Sum(a=>a.Cantidad_Cable);
						 var totallongitud14= postesLongitud.Where(a=>a.Longitud==14).AsEnumerable().Sum(a=>a.Cantidad_Cable);

						 var totallongitud10=postesLongitud.Where(a=>a.Longitud==10).AsEnumerable().Sum(a=>a.Cantidad_Cable);
						 var totallongitud16= postesLongitud.Where(a=>a.Longitud==16).AsEnumerable().Sum(a=>a.Cantidad_Cable);

						 var empresa = data.GetSafeStringValueOf<ElementoReportViewModel>(x => x.Nombre_Empresa);
						 var ciudad = data.GetSafeStringValueOf<ElementoReportViewModel>(x => x.Ciudad);
						 /*return string.Format(@"<table style='width: 100%; font-size:9pt;font-family:tahoma;'>
						 									<tr>
																<td style='width:25%;border-bottom-width:0.2; border-bottom-color:red;border-bottom-style:solid'>Ciudad:</td>
																<td style='width:75%'>{0}</td>
															</tr>
															
															<tr>
																<td style='width:25%;border-bottom-width:0.2; border-bottom-color:red;border-bottom-style:solid'>Empresa:</td>
																<td style='width:75%'>{1}</td>
															</tr>
												</table>",
												ciudad,empresa);*/
												
												return string.Format(@"<table style='width: 100%;font-size:7pt;font-family:tahoma;'>
																		<tr >
																			<th ><strong>Ciudad:</strong></th>
																			<td width='50%' align='center'>{0}</td>
																			<td ><strong>Postes</strong></td>
																			<td align='center'>{1}</td>
																			<td rowspan='2'><strong>Ocupaciones por longitud</strong></td>
																			<td rowspan='2' style='width: 100%;font-size:5pt;font-family:tahoma;'
																			>
																			<strong>6 m:  </strong> {4}
																			<br><strong>8 m:  </strong> {5}
																			<br><strong>10 m:  </strong> {6}
																			<br> <strong>12 m: </strong> {7}
																			<br> <strong>14 m: </strong> {8} 
																			<br> <strong>16 m: </strong> {9} 
																			</td>
																		</tr>
																		<tr>
																			<th ><strong>Operador:</strong></th>
																			<td align='center'>{2}</td>
																			<th ><strong>Ocupaciones:</strong></th>
																			<td align='center'>{3}</td>
																		
																		</tr>
																		</table>",ciudad,postes.Count(),empresa,ocupacion_total,totallongitud6,totallongitud8,totallongitud10,totallongitud12,totallongitud14,totallongitud16);
																		

																		/* 
																		return string.Format(@"<table style='width: 100%;font-size:7pt;font-family:tahoma;%'>
																			<tr>
																				<td><strong>Ciudad</strong></td>
																				<td>{0}</td>
																				<td><strong>Cable Operador</strong></td>
																				<td>{1}</td>
																			</tr>
																			</table>",ciudad,empresa);*/						
					 });
				 });
			 })
			 .MainTableTemplate(template =>
			 {
				 template.BasicTemplate(BasicTemplate.SilverTemplate);
			 })
			 .MainTablePreferences(table =>
			 {
				 table.ColumnsWidthsType(TableColumnWidthType.Relative);
				 table.GroupsPreferences(new GroupsPreferences
				 {
					 GroupType = GroupType.HideGroupingColumns,
					 
					 RepeatHeaderRowPerGroup = true,
					 ShowOneGroupPerPage = true,
					 
					 SpacingBeforeAllGroupsSummary = 5f,
					 NewGroupAvailableSpacingThreshold = 150
				 });

				//table.NumberOfDataRowsPerPage(35);
			 })
			 .MainTableDataSource(dataSource =>
			 {
				 var listOfRows = new List<ElementoReportViewModel>();
				
				 listOfRows=reportElementos;

				 listOfRows = listOfRows.OrderBy(x => x.Ciudad).ThenBy(x => x.Nombre_Empresa).ToList();
                 /// listOfRows = listOfRows.OrderBy(x => x.Department).ThenBy(x => x.Age).ToList();
				 dataSource.StronglyTypedList(listOfRows);
			 })
			 .MainTableSummarySettings(summarySettings =>
			 {
				//summarySettings.OverallSummarySettings("Summary");
              ///  summarySettings.PreviousPageSummarySettings("Previous Page Summary");
              //  summarySettings.PageSummarySettings("Page Summary");
			 })
			 .MainTableColumns(columns =>
			 {
				 columns.AddColumn(column =>
				 {
					 column.PropertyName("rowNo");
					 column.IsRowNumber(true);
					 column.CellsHorizontalAlignment(HorizontalAlignment.Center);
					 column.IsVisible(true);
					 column.Order(0);
					 column.Width(5);
					 
					 column.HeaderCell("#");
				 });

				 columns.AddColumn(column =>
				 {
					 column.PropertyName<ElementoReportViewModel>(x => x.Ciudad);
					 column.CellsHorizontalAlignment(HorizontalAlignment.Center);
					 column.Order(1);
					 column.Width(10);
					 column.HeaderCell("Ciudad");
					 column.Group(
					 (val1, val2) =>
					 {
						 return val1.ToString() == val2.ToString();
					 });
				 });

				 columns.AddColumn(column =>
				 {
					 column.PropertyName<ElementoReportViewModel>(x => x.Nombre_Empresa);
					 column.CellsHorizontalAlignment(HorizontalAlignment.Center);
					 column.Order(2);
					 column.Width(10);
					 column.HeaderCell("NombreEmpresaOperadora");
					 column.Group(
					 (val1, val2) =>
					 {
						 //return (int)val1 == (int)val2;
						 return val1.ToString() == val2.ToString();
					 });
				 });

				columns.AddColumn(column =>
				 {
					 column.PropertyName<ElementoReportViewModel>(x => x.Elemento_Id);
					 column.CellsHorizontalAlignment(HorizontalAlignment.Center);
					 column.IsVisible(true);
					 column.Order(3);
					 column.Width(10);
					 column.HeaderCell("Numero Apoyo");
				 });

				 columns.AddColumn(column =>
				 {
					 column.PropertyName<ElementoReportViewModel>(x => x.CodigoApoyo);
					 column.CellsHorizontalAlignment(HorizontalAlignment.Center);
					 column.IsVisible(true);
					 column.Order(4);
					 column.Width(10);
					 column.HeaderCell("Codigo Apoyo");
				 });

				 columns.AddColumn(column =>
				 {
					 column.PropertyName<ElementoReportViewModel>(x => x.Longitud);
					 column.CellsHorizontalAlignment(HorizontalAlignment.Center);
					 column.IsVisible(true);
					 column.Order(5);
					 column.Width(6);
					 column.HeaderCell("Long. Poste(M)");
				 });

				 columns.AddColumn(column =>
				 {
					 column.PropertyName<ElementoReportViewModel>(x => x.Nombre_Estado);
					 column.CellsHorizontalAlignment(HorizontalAlignment.Center);
					 column.IsVisible(true);
					 column.Order(6);
					 column.Width(6);
					 column.HeaderCell("Estado");
				 });

				 columns.AddColumn(column =>
				 {
					 column.PropertyName<ElementoReportViewModel>(x => x.Valor_Nivel_Tension);
					 column.CellsHorizontalAlignment(HorizontalAlignment.Center);
					 column.IsVisible(true);
					 column.Order(7);
					 column.Width(7);
					 column.HeaderCell("Nivel Tension");
				 });

				  columns.AddColumn(column =>
				 {
					 column.PropertyName<ElementoReportViewModel>(x => x.AlturaDisponible);
					 column.CellsHorizontalAlignment(HorizontalAlignment.Center);
					 column.IsVisible(true);
					 column.Order(8);
					 column.Width(8);
					 column.HeaderCell("Altura Disponible");
				 });

				columns.AddColumn(column =>
				 {
					 column.PropertyName<ElementoReportViewModel>(x => x.ResistenciaMecanica);
					 column.CellsHorizontalAlignment(HorizontalAlignment.Center);
					 column.IsVisible(true);
					 column.Order(9);
					 column.Width(10);
					 column.HeaderCell("Resistencia Mecanica");
				 });

				 columns.AddColumn(column =>
				 {
					 column.PropertyName<ElementoReportViewModel>(x => x.Nombre_Material);
					 column.CellsHorizontalAlignment(HorizontalAlignment.Center);
					 column.IsVisible(true);
					 column.Order(10);
					 column.Width(8);
					 column.HeaderCell("Material");
				 });

				 columns.AddColumn(column =>
				 {
					 column.PropertyName<ElementoReportViewModel>(x => x.Retenidas);
					 column.CellsHorizontalAlignment(HorizontalAlignment.Center);
					 column.IsVisible(true);
					 column.Order(11);
					 column.Width(8);
					 column.HeaderCell("Retenidas");
				 });

				  columns.AddColumn(column =>
				 {
					 column.PropertyName<ElementoReportViewModel>(x => x.Direccion_Elemento);
					 column.CellsHorizontalAlignment(HorizontalAlignment.Center);
					 column.IsVisible(true);
					 column.Order(12);
					 column.Width(12);
					 column.HeaderCell("Direccion");
				 });

				columns.AddColumn(column =>
				 {
					 column.PropertyName<ElementoReportViewModel>(x => x.Nombre_Cable);
					 column.CellsHorizontalAlignment(HorizontalAlignment.Center);
					 column.IsVisible(true);
					 column.Order(13);
					 column.Width(9);
					 column.HeaderCell("Cable");
				 });

				columns.AddColumn(column =>
				 {
					 column.PropertyName<ElementoReportViewModel>(x => x.Cantidad_Cable);
					 column.CellsHorizontalAlignment(HorizontalAlignment.Center);
					 column.IsVisible(true);
					 column.Order(14);
					 column.Width(9);
					 column.HeaderCell("Ocupaciones");
				 });

				 columns.AddColumn(column =>
				 {
					 column.PropertyName<ElementoReportViewModel>(x => x.Coordenadas_Elemento);
					 column.CellsHorizontalAlignment(HorizontalAlignment.Center);
					 column.IsVisible(true);
					 column.Order(15);
					 column.Width(16);
					 column.HeaderCell("Coordenadas");
				 });


			


				/* columns.AddColumn(column =>
				 {
					 column.PropertyName<Employee>(x => x.Salary);
					 column.CellsHorizontalAlignment(HorizontalAlignment.Center);
					 column.IsVisible(true);
					 column.Order(6);
					 column.Width(15);
					 column.HeaderCell("Salary");
					 column.ColumnItemsTemplate(template =>
					 {
						 template.TextBlock();
						 template.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
															? string.Empty : string.Format("{0:n0}", obj));
					 });
					 column.AggregateFunction(aggregateFunction =>
					 {
						 aggregateFunction.NumericAggregateFunction(AggregateFunction.Sum);
						 aggregateFunction.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
															? string.Empty : string.Format("{0:n0}", obj));
					 });
				 });*/
			 })

             /* 
              .MainTableEvents(events =>
			 {
				 events.DataSourceIsEmpty(message: "There is no data available to display.");
			 })
			 .Export(export =>
			 {
				 export.ToExcel();
			 })
			 .Generate(data => data.AsPdfFile(TestUtils.GetOutputFileName()));*/

			.MainTableEvents(events =>
                {
                    events.DataSourceIsEmpty(message: "There is no data available to display.");
				/* 
				events.CellCreated(args =>
                {
                    args.Cell.BasicProperties.CellPadding = 4f;
                });
                events.PageTableAdded(args =>
                {
                    var taxTableaddd = new PdfGrid(3);  // Create a clone of the MainTable's structure
                    taxTableaddd.RunDirection = 3;
                    taxTableaddd.SetWidths(new float[] { 3, 3, 3 });
                    taxTableaddd.WidthPercentage = 100f;
                    taxTableaddd.SpacingBefore = 10f;

                    taxTableaddd.AddSimpleRow(
                        (data, cellProperties) =>
                        {
                            data.Value = " Enuar";
                            cellProperties.ShowBorder = true;
                            cellProperties.PdfFont = args.PdfFont;
                        },
                        (data, cellProperties) =>
                        {
                            data.Value = " Muñoz";
                            cellProperties.ShowBorder = true;
                            cellProperties.PdfFont = args.PdfFont;
                        },
                        (data, cellProperties) =>
                        {
                            data.Value = " Castillo";
                            cellProperties.ShowBorder = true;
                            cellProperties.PdfFont = args.PdfFont;
                        });
                    args.PdfDoc.Add(taxTableaddd);
                });*/
				
				
                })
                .Export(export =>
                {
                    export.ToExcel();
                    
                });
		} 
    }
}