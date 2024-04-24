using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace CapstoneProject_TailTales.Models
{
    public class Provincia
    {
        public string Nome { get; set; }
        public string Sigla { get; set; }
        public string Regione { get; set; }
    }

    public class RegioniProvince
    {
        public static Tuple<List<string>, Dictionary<string, List<string>>> GetRegioniEProvince()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "jsonFiles", "province-italia.json");
            string jsonContent = File.ReadAllText(filePath);
            List<Provincia> provinceItaliane = JsonConvert.DeserializeObject<List<Provincia>>(jsonContent);

            List<string> regioniUniche = provinceItaliane.Select(p => p.Regione).Distinct().ToList();

            Dictionary<string, List<string>> provincePerRegione = new Dictionary<string, List<string>>();
            foreach (var regione in regioniUniche)
            {
                var provinceRegione = provinceItaliane.Where(p => p.Regione == regione).Select(p => p.Nome).ToList();
                provincePerRegione.Add(regione, provinceRegione);
            }

            return Tuple.Create(regioniUniche, provincePerRegione);
        }
    }

}