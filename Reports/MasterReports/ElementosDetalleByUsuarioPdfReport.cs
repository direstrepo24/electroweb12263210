using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Electro.model.datatakemodel;
using electroweb.DTO;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;
using PdfRpt.FluentInterface;

namespace electroweb.Reports.MasterReports
{
    public class ElementosDetalleByUsuarioPdfReport
    {
        public static byte[] CreateInMemoryPdfReport(string wwwroot,List<ElementoReportViewModel> ListElementos)
        {
			
            return CreateHtmlHeaderPdfReport(wwwroot,ListElementos,"","").GenerateAsByteArray(); // creating an in-memory PDF file
        }
       
		public static IPdfReportData CreateHtmlHeaderPdfReportStream(string wwwroot, Stream stream, List<ElementoReportViewModel> reportElementos,string date_start_report,string date_end_report)
        {
               return CreateHtmlHeaderPdfReport(wwwroot,reportElementos, date_start_report, date_end_report).Generate(data => data.AsPdfStream(stream, closeStream: false));
        }

        public  static PdfReport CreateHtmlHeaderPdfReport(String wwwroot,List<ElementoReportViewModel> reportElementos,string date_start_report,string date_end_report)
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
				 fonts.Color(System.Drawing.Color.White);
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
						///var encabezado_inferior= TestUtils.GetImagePath("reporte_inferior.png");
						//var encabezado_inferior= Path.Combine(wwwroot, "Images","reporte_inferior.png");
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
							proyecto = reportElementos.FirstOrDefault().Nombre_Empresa;
						}

						
						 //var dateNow = DateTime.Now.ToString("MM/dd/yyyy");
						  var dateNow = string.Format("{0} a {1}",date_start_report,date_end_report);
						//var encabezado= TestUtils.GetImagePath("logo_electrohuila.png");
						var encabezado= Path.Combine(wwwroot, "Images","logo_electrohuila.png");

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


						
						 //Details Elemento
						 var elemento_id = data.GetSafeStringValueOf<ElementoReportViewModel>(x => x.Elemento_Id);
						 var detalle_elemento= reportElementos.Where(a=>a.Elemento_Id==long.Parse(elemento_id)).FirstOrDefault();


						 var id_usuario= long.Parse(data.GetSafeStringValueOf<ElementoReportViewModel>(x => x.Usuario_Id));
						 var id_ciudad= long.Parse(data.GetSafeStringValueOf<ElementoReportViewModel>(x => x.Ciudad_Id));

						
						 long ocupacion_total=detalle_elemento.Cables.AsEnumerable().Sum(a=>a.Cantidad);
						 
						 var Usuario = data.GetSafeStringValueOf<ElementoReportViewModel>(x => x.Usuario);
						 var ciudad = data.GetSafeStringValueOf<ElementoReportViewModel>(x => x.Ciudad);
                        	
						var resumen_poste= string.Format(@"<table style='width: 100%;font-size:7pt;font-family:tahoma;'>
																		<tr>
																			<th ><strong>Ciudad:</strong></th>
																			<td width='50%' align='center'>{0}</td>
																			<td ><strong>Numero Apoyo</strong></td>
																			<td align='center'>{1}</td>
																		</tr>
																		<tr>
																			<th ><strong>Usuario:</strong></th>
																			<td align='center'>{2}</td>
																			<th ><strong>Ocupaciones:</strong></th>
																			<td align='center'>{3}</td>
																		</tr>
																		</table>",ciudad,elemento_id,Usuario,ocupacion_total);
																		




						

																		var posteHeader=string.Format(@"
																			{0}
																			<table  style=' width: 100%; font-size:7pt;font-family:tahoma;'>
																				<tr >
																					<td align='center'>
																			<span style='   color:#e26912; font-weight: bold;'>DETALLE POSTE</span></td>
																					
																				</tr>
																			</table>
																			<table border='1'  style=' width: 100%; font-size:5pt;font-family:tahoma;'>												
																				<tr align='center'>
																					<td width='5%'><strong>Numero Apoyo</strong></td>
																					<td width='5%'><strong>Codigo Apoyo</strong></td>
																					<td width='5%'><strong>Long. Poste</strong></td>
																					<td width='10%'><strong>Estado</strong></td>
																					<td width='10%'><strong>Nivel Tension</strong></td>
																					<td width='10%'><strong>Altura Disponible</strong></td>
																					<td width='10%'><strong>Resistencia Mecanica</strong></td>
																					<td width='10%'><strong>Material</strong></td>
																					<td width='10%'><strong>Retenidas</strong></td>
																					<td width='10%'><strong>Direccion</strong></td>
																					<td width='20%'><strong>Coordenadas</strong></td>
																				</tr>
																				<tr  align='center'>
																					<td width='5%'>{1}</td>
																					<td width='5%'>{2}</td>
																					<td width='5%'>{3}</td>
																					<td width='10%'>{4}</td>
																					<td width='10%'>{5}</td>
																					<td width='10%'>{6}</td>
																					<td width='10%'>{7}</td>
																					<td width='10%'>{8}</td>
																					<td width='10%'>{9}</td>
																					<td width='10%'>{10}</td>
																					<td style='font-size:4pt;font-family:tahoma;' width='20%'>{11}</td>
																				</tr>
																			</table>",resumen_poste,detalle_elemento.Id,detalle_elemento.CodigoApoyo,detalle_elemento.Longitud,detalle_elemento.Nombre_Estado,detalle_elemento.Valor_Nivel_Tension,detalle_elemento.AlturaDisponible,detalle_elemento.ResistenciaMecanica,detalle_elemento.Nombre_Material,detalle_elemento.Retenidas,detalle_elemento.Direccion_Elemento,detalle_elemento.Coordenadas_Elemento);




													var listCables= detalle_elemento.Cables;
													var tablacables= string.Format(@"
													<table  style='width: 100%; font-size:7pt;font-family:tahoma;'>
														<tr>
															<td align='center'>
													<span style='   color:#e26912; font-weight: bold;'>CABLES</span></td>
														</tr>
													</table>
													<table border='1'  style=' width: 100%; font-size:5pt;font-family:tahoma;'>											
														<tr align='center'>
															<td><strong>Num.</strong></td>
															<td><strong>Operador</strong></td>
															<td><strong>Tipo</strong></td>
															<td><strong>Detalle</strong></td>
															<td><strong>Nivel Ocupacion</strong></td>
															<td><strong>Esta el cable sobre RBT ?</strong></td>
															<td><strong>Cable cuenta con marquilla ?</strong></td>
														</tr>");
														var number_row_cable=0;
														foreach(var item in listCables){
															number_row_cable=number_row_cable+1;
															tablacables += @"<tr  align='center'>
																		<td>"+number_row_cable+"</td>"+
																		"<td>"+item.NombreEmpresaCable+"</td>"+
																		"<td>"+item.TipoCable+"</td>"+
																		"<td>"+item.DetalleCable+"</td>"+
																		"<td>"+item.Cantidad+"</td>"+
																		"<td>"+item.SobreRbt+"</td>"+
																		"<td>"+item.Tiene_Marquilla+"</td>"+
																		"</tr>";
														}

														tablacables += @"</table>";


													var listequipos=detalle_elemento.Equipos;
													var tablaequipos= string.Format(@"
													<table  style='width: 100%; font-size:7pt;font-family:tahoma;'>
														<tr >
															<td align='center'>
													<span style='   color:#e26912; font-weight: bold;'>EQUIPOS</span></td>
														</tr>
													</table>
													<table border='1'  style=' width: 100%; font-size:5pt;font-family:tahoma;'>											
														<tr align='center'>
															<td><strong>Num.</strong></td>
															<td><strong>Operador</strong></td>
															<td><strong>Tipo</strong></td>
															<td><strong>Conectado red Electrica</strong></td>
															<td><strong>Medidor</strong></td>
															
														</tr>");
														var number_row_equipo=0;
														foreach(var equipos in listequipos){
															number_row_equipo=number_row_equipo+1;
															tablaequipos += @"<tr  align='center'>
																		<td>"+number_row_equipo+"</td>"+
																		"<td>"+equipos.NombreEmpresaEquipo+"</td>"+
																		"<td>"+equipos.TipoEquipo.Nombre+"</td>"+
																		"<td>"+equipos.ConectadoRbt+"</td>"+
																		"<td>"+equipos.MedidorBt+"</td>"+
																		"</tr>";
														}

														tablaequipos += @"</table>";



														var listfotos=detalle_elemento.Fotos;
														var tablafotos=string.Format(@"
															<table  style='width: 100%; font-size:7pt;font-family:tahoma;'>
																<tr >
																		<td align='center'>
																		<span style='   color:#e26912; font-weight: bold;'>FOTOS</span></td>
																</tr>
															</table>
															<table border='1'  style='  font-size:5pt;font-family:tahoma;'>											
																
																");

														var countfotos=listfotos.Count;
														var i =0;
														var recuadro_empty= Path.Combine(wwwroot, "Images","recuaadro.png");

														if(countfotos>4){
															    
																
																foreach(var foto in listfotos){
																	if(foto.Ruta.ToUpper().Contains("Foto Nula".ToUpper())){
																		foto.Ruta="/Images/recuaadro.png";
																	}
																	
																	i=i+1;
																	//Fila 1
																	if(i==1){
																		tablafotos += @"<tr align='center'>";
																		tablafotos += string.Format(@"<td><img  width='120'  src='http://181.60.56.39:89{0}' /><p>Titulo: {1}</p><p>Descripcion: {2}</p></td>", foto.Ruta,foto.Titulo,foto.Descripcion );
																	}else if(i==2){
																		tablafotos += string.Format(@"<td><img  width='120'  src='http://181.60.56.39:89{0}' /><p>Titulo: {1}</p><p>Descripcion: {2}</p></td>", foto.Ruta,foto.Titulo,foto.Descripcion );
																	}else if(i==3){
																		tablafotos += string.Format(@"<td><img  width='120'  src='http://181.60.56.39:89{0}' /><p>Titulo: {1}</p><p>Descripcion: {2}</p></td>", foto.Ruta,foto.Titulo,foto.Descripcion );
																	}else if(i==4){
																		tablafotos += string.Format(@"<td><img  width='120'  src='http://181.60.56.39:89{0}' /><p>Titulo: {1}</p><p>Descripcion: {2}</p></td>", foto.Ruta,foto.Titulo,foto.Descripcion );
																		tablafotos += @"</tr>";
																	}

																	//Fila 2
																	else if(i==5){
																		tablafotos += @"<tr align='center'>";
																		tablafotos += string.Format(@"<td><img  width='120'  src='http://181.60.56.39:89{0}' /><p>Titulo: {1}</p><p>Descripcion: {2}</p></td>", foto.Ruta,foto.Titulo,foto.Descripcion );
																		if(countfotos==5){
																		  tablafotos +=string.Format(@"<td>{0}</td>","");
																		  tablafotos +=string.Format(@"<td>{0}</td>","");
																		  tablafotos +=string.Format(@"<td>{0}</td>","");
																		  tablafotos += @"</tr>";
																		}
																	}else if(i==6){
																			tablafotos += string.Format(@"<td><img  width='120'  src='http://181.60.56.39:89{0}' /><p>Titulo: {1}</p><p>Descripcion: {2}</p></td>", foto.Ruta,foto.Titulo,foto.Descripcion );
																			if(countfotos==6){
																		  	//	tablafotos +=string.Format(@"<td><img  width='120'  src='{0}' /><p>Titulo: {1}</p><p>Descripcion: {2}</p></td>",recuadro_empty,"","");
																		  		tablafotos +=string.Format(@"<td>{0}</td>","");
																		        tablafotos +=string.Format(@"<td>{0}</td>","");
																				tablafotos += @"</tr>";
																			}
																	}else if(i==7){
																			tablafotos += string.Format(@"<td><img  width='120'  src='http://181.60.56.39:89{0}' /><p>Titulo: {1}</p><p>Descripcion: {2}</p></td>", foto.Ruta,foto.Titulo,foto.Descripcion );
																			if(countfotos==7){
																		  		tablafotos +=string.Format(@"<td>{0}</td>","");
																				tablafotos += @"</tr>";
																			}
																	}else if(i==8){
																			tablafotos += string.Format(@"<td><img  width='120'  src='http://181.60.56.39:89{0}' /><p>Titulo: {1}</p><p>Descripcion: {2}</p></td>", foto.Ruta,foto.Titulo,foto.Descripcion );
																			tablafotos += @"</tr>";
																	}
																}

														}else{
														    tablafotos += @"<tr align='center'>";
															foreach(var foto in listfotos){
																if(foto.Ruta.ToUpper().Contains("Foto Nula".ToUpper())){
																	foto.Ruta="/Images/recuaadro.png";
																}
																		
																i=i+1;
																if(i==1){
																	   tablafotos += string.Format(@"<td><img  width='120'  src='http://181.60.56.39:89{0}' /><p>Titulo: {1}</p><p>Descripcion: {2}</p></td>", foto.Ruta,foto.Titulo,foto.Descripcion );
																}else if(i==2){
																		tablafotos += string.Format(@"<td><img  width='120'  src='http://181.60.56.39:89{0}' /><p>Titulo: {1}</p><p>Descripcion: {2}</p></td>", foto.Ruta,foto.Titulo,foto.Descripcion );
																}else if(i==3){
																		tablafotos += string.Format(@"<td><img  width='120'  src='http://181.60.56.39:89{0}' /><p>Titulo: {1}</p><p>Descripcion: {2}</p></td>", foto.Ruta,foto.Titulo,foto.Descripcion );
																}else if(i==4){
																		tablafotos += string.Format(@"<td><img  width='120'  src='http://181.60.56.39:89{0}' /><p>Titulo: {1}</p><p>Descripcion: {2}</p></td>", foto.Ruta,foto.Titulo,foto.Descripcion );
																}
															}
															tablafotos += @"</tr>";
														}
														
														tablafotos += @"</table>";

												return string.Format(@"{0}{1}{2}{3}",posteHeader,tablacables,tablaequipos,tablafotos);



																	
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
					 column.Width(0);
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
					 column.PropertyName<ElementoReportViewModel>(x => x.Usuario);
					 column.CellsHorizontalAlignment(HorizontalAlignment.Center);
					 column.Order(2);
					 column.Width(10);
					 column.HeaderCell("Usuario");
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
					 column.Order(3);
					 column.Width(10);
					 column.HeaderCell("Elemento_Id");
					 column.Group(
					 (val1, val2) =>
					 {
						 //return (int)val1 == (int)val2;
						 return val1.ToString() == val2.ToString();
					 });
				 });

				 columns.AddColumn(column =>
				 {
					 column.PropertyName<ElementoReportViewModel>(x => x.CodigoApoyo);
					 column.CellsHorizontalAlignment(HorizontalAlignment.Center);
					 column.IsVisible(false);
					 column.Order(4);
					 column.Width(8);
					 column.HeaderCell("Codigo Apoyo");
				 });

				 columns.AddColumn(column =>
				 {
					 column.PropertyName<ElementoReportViewModel>(x => x.Longitud);
					 column.CellsHorizontalAlignment(HorizontalAlignment.Center);
					 column.IsVisible(false);
					 column.Order(5);
					 column.Width(6);
					 column.HeaderCell("Long. Poste(M)");
				 });

				 columns.AddColumn(column =>
				 {
					 column.PropertyName<ElementoReportViewModel>(x => x.Nombre_Estado);
					 column.CellsHorizontalAlignment(HorizontalAlignment.Center);
					 column.IsVisible(false);
					 column.Order(6);
					 column.Width(6);
					 column.HeaderCell("Estado");
				 });

				 columns.AddColumn(column =>
				 {
					 column.PropertyName<ElementoReportViewModel>(x => x.Valor_Nivel_Tension);
					 column.CellsHorizontalAlignment(HorizontalAlignment.Center);
					 column.IsVisible(false);
					 column.Order(7);
					 column.Width(6);
					 column.HeaderCell("Nivel Tension");
				 });

				  columns.AddColumn(column =>
				 {
					 column.PropertyName<ElementoReportViewModel>(x => x.AlturaDisponible);
					 column.CellsHorizontalAlignment(HorizontalAlignment.Center);
					 column.IsVisible(false);
					 column.Order(8);
					 column.Width(7);
					 column.HeaderCell("Altura Disponible");
				 });

				columns.AddColumn(column =>
				 {
					 column.PropertyName<ElementoReportViewModel>(x => x.ResistenciaMecanica);
					 column.CellsHorizontalAlignment(HorizontalAlignment.Center);
					 column.IsVisible(false);
					 column.Order(9);
					 column.Width(8);
					 column.HeaderCell("Resistencia Mecanica");
				 });

				 columns.AddColumn(column =>
				 {
					 column.PropertyName<ElementoReportViewModel>(x => x.Nombre_Material);
					 column.CellsHorizontalAlignment(HorizontalAlignment.Center);
					 column.IsVisible(false);
					 column.Order(10);
					 column.Width(7);
					 column.HeaderCell("Material");
				 });

				 columns.AddColumn(column =>
				 {
					 column.PropertyName<ElementoReportViewModel>(x => x.Retenidas);
					 column.CellsHorizontalAlignment(HorizontalAlignment.Center);
					 column.IsVisible(false);
					 column.Order(11);
					 column.Width(7);
					 column.HeaderCell("Retenidas");
				 });

				  columns.AddColumn(column =>
				 {
					 column.PropertyName<ElementoReportViewModel>(x => x.Direccion_Elemento);
					 column.CellsHorizontalAlignment(HorizontalAlignment.Center);
					 column.IsVisible(false);
					 column.Order(12);
					 column.Width(12);
					 column.HeaderCell("Direccion");
				 });
				columns.AddColumn(column =>
				 {
					 column.PropertyName<ElementoReportViewModel>(x => x.Coordenadas_Elemento);
					 column.CellsHorizontalAlignment(HorizontalAlignment.Center);
					 column.IsVisible(false);
					 column.Order(13);
					 column.Width(16);
					 column.HeaderCell("Coordenadas");
				 });

				 	columns.AddColumn(column =>
				 {
					 column.PropertyName<ElementoReportViewModel>(x => x.HoraInicio);
					 column.CellsHorizontalAlignment(HorizontalAlignment.Center);
					 column.IsVisible(false);
					 column.Order(14);
					 column.Width(8);
					 column.HeaderCell("Hora Inicio");
				 });
				 columns.AddColumn(column =>
				 {
					 column.PropertyName<ElementoReportViewModel>(x => x.HoraFin);
					 column.CellsHorizontalAlignment(HorizontalAlignment.Center);
					 column.IsVisible(false);
					 column.Order(15);
					 column.Width(8);
					 column.HeaderCell("Hora Fin");
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