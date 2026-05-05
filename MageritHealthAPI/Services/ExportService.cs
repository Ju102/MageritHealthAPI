using MageritHealthAPI.Repositories.Interfaces;
using MageritHealthAPI.Services.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MageritHealth.Services
{
    public class ExportService
    {
        private readonly IAnaliticasRepository analiticasRepository;
        private readonly IPrescripcionesRepository prescripcionesRepository;
        private readonly ICitasRepository citasRepository;

        public ExportService(IAnaliticasRepository analiticasRepository, IPrescripcionesRepository prescripcionesRepo, ICitasRepository citasRepo)
        {
            this.analiticasRepository = analiticasRepository;
            this.prescripcionesRepository = prescripcionesRepo;
            this.citasRepository = citasRepo;

            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> GenerarInformeAnaliticaPdfAsync(int idAnalitica)
        {
            var mediciones = await this.analiticasRepository.GetListaMedicionesByIdAnaliticaAsync(idAnalitica);

            var analitica = mediciones.FirstOrDefault()?.Analitica;
            var fecha = analitica?.FechaAnalitica.ToString("dd/MM/yyyy") ?? "Desconocida";

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Magerit Health").FontSize(20).SemiBold().FontColor("#0f4c81");
                            col.Item().Text("Informe de Resultados de Laboratorio").FontSize(14).FontColor(Colors.Grey.Darken2);
                            col.Item().PaddingTop(5).Text($"Fecha de la prueba: {fecha}");
                        });

                        // row.ConstantItem(100).Height(50).Placeholder();
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Parámetro").SemiBold().FontColor(Colors.White);
                                header.Cell().Element(CellStyle).Text("Resultado").SemiBold().FontColor(Colors.White);
                                header.Cell().Element(CellStyle).Text("Rango Ref.").SemiBold().FontColor(Colors.White);
                                header.Cell().Element(CellStyle).Text("Unidades").SemiBold().FontColor(Colors.White);

                                IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black).Background("#0f4c81").PaddingHorizontal(5);
                                }
                            });

                            foreach (var med in mediciones)
                            {
                                var tipo = med.TipoMedicion;
                                bool fueraDeRango = med.ValorMedicion < tipo.ValorMinimo || med.ValorMedicion > tipo.ValorMaximo;
                                string colorTexto = fueraDeRango ? Colors.Red.Medium : Colors.Black;

                                table.Cell().Element(BlockStyle).Text(tipo.NombreMedicion);

                                var textResult = table.Cell().Element(BlockStyle).Text(med.ValorMedicion.ToString("0.00")).FontColor(colorTexto);
                                if (fueraDeRango) textResult.SemiBold();

                                table.Cell().Element(BlockStyle).Text($"{tipo.ValorMinimo} - {tipo.ValorMaximo}");
                                table.Cell().Element(BlockStyle).Text(tipo.UnidadMedicion);

                                IContainer BlockStyle(IContainer container)
                                {
                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).PaddingHorizontal(5);
                                }
                            }
                        });

                        col.Item().PaddingTop(25).Text("Observaciones:").SemiBold();
                        col.Item().Text(analitica?.Notas ?? "Sin observaciones.").FontColor(Colors.Grey.Darken3);
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }

        public async Task<byte[]> GenerarInformeCitaPdfAsync(int idCita)
        {
            var cita = await citasRepository.FindCitaByIdAsync(idCita);

            if (cita == null)
            {
                return null;
            }

            var paciente = cita.Paciente;
            var doctor = cita.Doctor;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial).FontColor(Colors.Black));

                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Magerit Health").FontSize(24).SemiBold().FontColor("#0f4c81");
                            row.ConstantItem(150).AlignRight().Text("INFORME CLÍNICO").FontSize(14).FontColor(Colors.Grey.Medium);
                        });

                        col.Item().PaddingTop(15).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                        col.Item().PaddingTop(10).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("DATOS DEL PACIENTE").FontSize(9).FontColor(Colors.Grey.Darken1).SemiBold();
                                c.Item().Text($"{paciente.Nombre} {paciente.Apellido1} {paciente.Apellido2}").FontSize(12).SemiBold();
                                c.Item().Text($"DNI: {paciente.Dni}");
                                c.Item().Text($"Nº Asegurado: {paciente.NumeroAsegurado ?? "N/A"}");
                            });

                            row.RelativeItem().AlignRight().Column(c =>
                            {
                                c.Item().Text("DETALLES DE LA CONSULTA").FontSize(9).FontColor(Colors.Grey.Darken1).SemiBold();
                                c.Item().Text($"Fecha: {cita.FechaHora.ToString("dd/MM/yyyy HH:mm")}").SemiBold();
                                c.Item().Text($"Dr/a. {doctor.Nombre} {doctor.Apellido1}");
                                c.Item().Text($"{doctor.Especialidad?.NombreEspecialidad ?? "Especialista"} (Col: {doctor.NumeroColegiado})").FontSize(10).FontColor(Colors.Grey.Darken2);
                            });
                        });

                        col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                    });

                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        col.Item().PaddingBottom(15).Column(c =>
                        {
                            c.Item().Text("Motivo de la Visita:").FontSize(12).SemiBold().FontColor("#0f4c81");
                            c.Item().PaddingTop(5).Background("#f8f9fa").Padding(10).Text(cita.Motivo);
                        });

                        col.Item().PaddingBottom(15).Column(c =>
                        {
                            c.Item().Text("Juicio Clínico / Observaciones:").FontSize(12).SemiBold().FontColor("#0f4c81");

                            string notas = string.IsNullOrWhiteSpace(cita.Notas)
                                ? "El especialista no ha registrado observaciones adicionales en esta consulta."
                                : cita.Notas;

                            c.Item().PaddingTop(5).Text(notas);
                        });
                    });

                    page.Footer().Column(col =>
                    {
                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        col.Item().PaddingTop(10).Row(row =>
                        {
                            row.RelativeItem().Text(text =>
                            {
                                text.DefaultTextStyle(style => style.FontSize(8).FontColor(Colors.Grey.Medium));

                                text.Span("Magerit Health").SemiBold();
                                text.Span(" | Documento generado el " + System.DateTime.Now.ToString("dd/MM/yyyy"));
                            });

                            row.RelativeItem().AlignRight().Text(text =>
                            {
                                text.DefaultTextStyle(style => style.FontSize(8).FontColor(Colors.Grey.Medium));

                                text.Span("Página ");
                                text.CurrentPageNumber();
                                text.Span(" de ");
                                text.TotalPages();
                            });
                        });
                    });
                });
            });

            return document.GeneratePdf();
        }

        //public async Task<byte[]> GenerarRecetasPorCitaPdfAsync(int idCita)
        //{
        //    var prescripciones = await this.prescripcionesRepository.GetListaPrescripcionesByIdCitaAsync(idCita);

        //    if (prescripciones == null || !prescripciones.Any())
        //    {
        //        return null;
        //    }

        //    var cita = prescripciones.First().Cita;
        //    var paciente = cita.Paciente;
        //    var doctor = cita.Doctor;

        //    var document = Document.Create(container =>
        //    {
        //        container.Page(page =>
        //        {
        //            page.Size(PageSizes.A5);
        //            page.Margin(1.5f, Unit.Centimetre);
        //            page.PageColor(Colors.White);
        //            page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));

        //            page.Header().Column(col =>
        //            {
        //                col.Item().Row(row =>
        //                {
        //                    row.RelativeItem().Text("Magerit Health").FontSize(18).SemiBold().FontColor("#0f4c81");
        //                    row.ConstantItem(100).AlignRight().Text("RECETA MÉDICA").FontSize(12).FontColor(Colors.Grey.Medium);
        //                });

        //                col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

        //                col.Item().PaddingTop(5).Row(row =>
        //                {
        //                    row.RelativeItem().Column(c =>
        //                    {
        //                        c.Item().Text($"Dr./a. {doctor.Nombre} {doctor.Apellido1} {doctor.Apellido2}").SemiBold();
        //                        c.Item().Text($"Nº Colegiado: {doctor.NumeroColegiado}").FontSize(9).FontColor(Colors.Grey.Darken2);
        //                        c.Item().Text($"Especialidad: {doctor.Especialidad?.NombreEspecialidad}").FontSize(9).FontColor(Colors.Grey.Darken2);
        //                    });
        //                    row.RelativeItem().AlignRight().Column(c =>
        //                    {
        //                        c.Item().Text($"Fecha: {cita.FechaHora.ToString("dd/MM/yyyy")}").SemiBold();
        //                    });
        //                });

        //                col.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

        //                col.Item().PaddingTop(5).Column(c =>
        //                {
        //                    c.Item().Text("DATOS DEL PACIENTE").FontSize(8).FontColor(Colors.Grey.Medium).SemiBold();
        //                    c.Item().Text($"{paciente.Nombre} {paciente.Apellido1} {paciente.Apellido2}").FontSize(11).SemiBold();
        //                    c.Item().Text($"DNI: {paciente.Dni} | Nº Asegurado: {paciente.NumeroAsegurado ?? "N/A"}").FontSize(9);
        //                });

        //                col.Item().Height(15);
        //            });

        //            page.Content().PaddingVertical(10).Column(col =>
        //            {
        //                col.Item().PaddingBottom(10).Text("PLAN DE MEDICACIÓN").FontSize(12).SemiBold().FontColor("#0f4c81");

        //                foreach (var prescripcion in prescripciones)
        //                {
        //                    var med = prescripcion.Medicamento;

        //                    col.Item().PaddingBottom(15).Background("#f8f9fa").Border(1).BorderColor(Colors.Grey.Lighten3).Padding(10).Column(medCol =>
        //                    {
        //                        medCol.Item().Row(row =>
        //                        {
        //                            row.RelativeItem().Text(text =>
        //                            {
        //                                text.Span($"{med.NombreComercial} {med.Concentracion} ").FontSize(11).SemiBold().FontColor(Colors.Black);
        //                                text.Span($"({med.Formato})").FontSize(9).FontColor(Colors.Grey.Darken2).Italic();
        //                            });
        //                        });

        //                        medCol.Item().PaddingBottom(5).Text(med.PrincipioActivo).FontSize(8).FontColor(Colors.Grey.Medium);

        //                        medCol.Item().Text(text =>
        //                        {
        //                            text.Span("Pauta: ").SemiBold();
        //                            text.Span(prescripcion.Instrucciones);
        //                        });

        //                        medCol.Item().PaddingTop(5).Text(text =>
        //                        {
        //                            text.Span("Duración: ").FontSize(9).SemiBold();
        //                            text.Span($"Del {prescripcion.FechaInicio.ToString("dd/MM/yyyy")} al {prescripcion.FechaFin.ToString("dd/MM/yyyy")}").FontSize(9);
        //                        });
        //                    });
        //                }
        //            });

        //            page.Footer().Column(col =>
        //            {
        //                col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
        //                col.Item().PaddingTop(5).Row(row =>
        //                {
        //                    row.RelativeItem().Text("Firma del Facultativo:").FontSize(8).FontColor(Colors.Grey.Darken1);
        //                });

        //                col.Item().Height(40);

        //                col.Item().AlignCenter().Text("Documento generado electrónicamente por Magerit Health. Válido para dispensación en farmacias.").FontSize(7).FontColor(Colors.Grey.Medium).Italic();
        //            });
        //        });
        //    });

        //    return document.GeneratePdf();
        //}
    }
}