using MageritHealthAPI.Models;

namespace MageritHealthAPI.Services
{
    public class SimuladorLaboratorioService
    {
        private readonly Random random = new Random();

        public List<Medicion> GenerarMedicionesCasuales(List<TipoMedicion> tiposMedicion, int idAnalitica)
        {
            var mediciones = new List<Medicion>();

            foreach (var tipo in tiposMedicion.Where(t => t.Activo))
            {
                var medicion = new Medicion
                {
                    IdTipoMedicion = tipo.IdTipoMedicion,
                    IdAnalitica = idAnalitica,
                    ValorMedicion = GenerarValorRealista(tipo.ValorMinimo, tipo.ValorMaximo)
                };

                mediciones.Add(medicion);
            }

            return mediciones;
        }

        private decimal GenerarValorRealista(decimal minRef, decimal maxRef)
        {
            if (minRef >= maxRef) return minRef;

            decimal amplitud = maxRef - minRef;
            int probabilidad = random.Next(1, 101); // 1 a 100
            double factorExtra;
            decimal resultado;

            // --- MODELO DE PROBABILIDAD CLÍNICA ---

            if (probabilidad <= 75)
            {
                // 75% Probabilidad: NORMAL (Dentro del rango, centrado)
                double normalDist = 0.1 + (random.NextDouble() * 0.8); // 0.1 a 0.9
                resultado = minRef + (amplitud * (decimal)normalDist);
            }
            else if (probabilidad <= 85)
            {
                // 10% Probabilidad: ALTO
                factorExtra = random.NextDouble() * 0.25; // 0% a 25% extra
                resultado = maxRef + (amplitud * (decimal)factorExtra);
            }
            else if (probabilidad <= 95)
            {
                // 10% Probabilidad: BAJO
                factorExtra = random.NextDouble() * 0.25; // 0% a 25% menos
                resultado = minRef - (amplitud * (decimal)factorExtra);
            }
            else
            {
                // 5% Probabilidad: CRÍTICO
                bool esCriticoAlto = random.Next(0, 2) == 1;
                factorExtra = 0.5 + (random.NextDouble() * 0.5); // 50% a 100% de desviación

                if (esCriticoAlto)
                    resultado = maxRef + (amplitud * (decimal)factorExtra);
                else
                    resultado = minRef - (amplitud * (decimal)factorExtra);
            }

            // Asegurar que no hay valores negativos
            if (resultado < 0) resultado = Math.Abs(resultado) * 0.1m;

            return Math.Round(resultado, 2);
        }
    }
}