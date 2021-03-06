/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    This software it's under the terms of the GNU Affero General Public License version 3.
    Please, refer to (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) for further licensing details.
 */

using System;
using System.Linq;
using System.Collections.Generic;
using DocumentPlagiarismChecker.Core;
using DocumentPlagiarismChecker.Scores;

namespace DocumentPlagiarismChecker.Comparators.DocumentWordCounter
{
    /// <summary>
    ///El comptador de paraules llegeix un parell de fitxers i compta quantes paraules i
    /// quantes vegades apareixen a cada fitxer, i després calcula quantes d'aquestes coincidències
    /// entre documents. Per tant, dos documents amb la mateixa quantitat de les mateixes paraules poden
    /// ser una còpia amb un alt nivell de demostrabilitat.
    /// </summary>
    /// <typeparam name="Document"></typeparam>
    internal class Comparator: Core.BaseComparator<Document>
    {
        /// <summary>
        /// Crea una instància nova per al comparador..
        /// </summary>
        /// <param name="fileLeftPath">El camí del fitxer del costat esquerre.</param>
        /// <param name="fileRightPath">El camí del fitxer del costat dret.</param>
        /// <param name="settings">La instància de configuració que utilitzarà el comparador.</param>
        /// <returns></returns>
        public Comparator(string fileLeftPath, string fileRightPath, Settings settings): base(fileLeftPath, fileRightPath, settings){
        }

        /// <summary>
        /// Compta quantes paraules i quantes vegades apareixen a cada document i comprova el percentatge de coincidència.
        /// </summary>
        /// <returns>Els resultats de la coincidència.</returns>
        public override ComparatorMatchingScore Run(){
            //Comtat de les   paraules aparences per a cada document (esquerra i dreta).
            Dictionary<string, int[]> counter = new Dictionary<string, int[]>();
            foreach(string word in this.Left.WordAppearances.Select(x => x.Key)){
                if(!counter.ContainsKey(word)) counter.Add(word, new int[]{0, 0});
                counter[word][0] += Left.WordAppearances[word];
            }

            foreach(string word in this.Right.WordAppearances.Select(x => x.Key)){
                if(!counter.ContainsKey(word)) counter.Add(word, new int[]{0, 0});
                counter[word][1] += Right.WordAppearances[word];
            }


            //Comptant aparences de paraules del fitxer de mostra, per tal d'ignorar les dels fitxers anteriors.
            if(this.Sample != null){
                 foreach(string word in this.Sample.WordAppearances.Select(x => x.Key)){
                    if(counter.ContainsKey(word)){
                        counter[word][0] = Math.Max(0, counter[word][0] - Sample.WordAppearances[word]);
                        counter[word][1] = Math.Max(0, counter[word][1] - Sample.WordAppearances[word]);

                        if(counter[word][0] == 0 && counter[word][1] == 0)
                            counter.Remove(word);
                    }
                }
            }

           //Definint les capçaleres dels resultats
            ComparatorMatchingScore cr = new ComparatorMatchingScore(this.Left.Name, this.Right.Name, "Document Word Counter", DisplayLevel.FULL);
            cr.DetailsCaption = new string[] { "Word", "Left count", "Right count", "Match" };
            cr.DetailsFormat = new string[]{"{0}", "{0}", "{0}", "{0:P2}"};

            //Calcula la coincidència per a cada paraula individual.
            foreach(string word in counter.Select(x => x.Key)){
                int left = counter[word][0];
                int right = counter[word][1];
                float match = (left == 0 || right == 0 ? 0 : (left < right ? (float)left / (float)right : (float)right / (float)left));

                cr.AddMatch(match);
                cr.DetailsData.Add(new object[]{word, left, right, match});
            }

            return cr;
        }
    }
}
