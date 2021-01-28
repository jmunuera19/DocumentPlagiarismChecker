/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    This software it's under the terms of the GNU Affero General Public License version 3.
    Please, refer to (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) for further licensing details.
 */

using System.Linq;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace DocumentPlagiarismChecker.Comparators.DocumentWordCounter
{
    /// <summary>
    /// Aquest document s'ha d'utilitzar amb el comparador de comptadors de paraules de documents i emmagatzema quantes paraules i quantes vegades apareixen dins d'un document.
    /// </summary>
    internal class Document: Core.BaseDocument
    {
        /// <summary>
        /// Conté les paraules (clau) i les aparences (valor).
        /// </summary>
        /// <value></value>
        public Dictionary<string, int> WordAppearances {get; set;}


        /// <summary>
        /// Carrega el contingut d'un fitxer PDF i compta quantes paraules i quantes vegades apareixen al document.        /// </summary>
        /// <param name="path">El camí del fitxer..</param>
        public Document(string path): base(path){
            //Consulteu les condicions prèvies
            if(!System.IO.Path.GetExtension(path).ToLower().Equals(".pdf"))
                throw new Exceptions.FileExtensionNotAllowed();

            //Atributs d'objecte Init.
            WordAppearances = new Dictionary<string, int>();

            //Llegiu el fitxer PDF i deseu el recompte de paraules.
            using (PdfReader reader = new PdfReader(path))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    string text = PdfTextExtractor.GetTextFromPage(reader, i).Replace("\n", "");
                    foreach(string word in text.Split(" ").Where(x => !string.IsNullOrEmpty(x.Trim()))){
                        if(!WordAppearances.ContainsKey(word))
                            WordAppearances.Add(word, 0);

                        WordAppearances[word]++;
                    }
                }
            }
        }
    }
}
