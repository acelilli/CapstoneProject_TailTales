using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace CapstoneProject_TailTales.Models
{
    public class Razza
    {
        public string Nome { get; set; }
        public string Specie { get; set; }
    }

    public class SpecieRazze
    {

        public static Tuple<List<string>, Dictionary<string, List<string>>> GetSpecieRazze()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "jsonFiles", "AllPets.json");
            string jsonContent = File.ReadAllText(filePath);
            List<Razza> razze = JsonConvert.DeserializeObject<List<Razza>>(jsonContent);

            List<string> specieUniche = razze.Select(r => r.Specie).Distinct().ToList();

            Dictionary<string, List<string>> razzePerSpecie = new Dictionary<string, List<string>>();
            foreach (var specie in specieUniche)
            {
                var razzeDellaSpecie = razze.Where(r => r.Specie == specie).Select(r => r.Nome).ToList();
                razzePerSpecie.Add(specie, razzeDellaSpecie);
            }

            return Tuple.Create(specieUniche, razzePerSpecie);
        }


    }
}