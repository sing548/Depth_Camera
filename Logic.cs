namespace Depth_Camera
{
    public class Logic
    {
        public List<KeyValuePair<string, int>> CalculateVolumesForFolder(string folder)
        {
            string[] files = Directory.GetFiles(folder);
            //BMP wird zu ByteArray um darin über die einzelnen Werte zu iterieren
            Dictionary<string, byte[]> byteArraysDict = new Dictionary<string, byte[]>();
            //Die Ergebnisse kommen werden als einfacher Integer Wert ausgegeben.
            List<KeyValuePair<string, int>> volumeDictMean = new List<KeyValuePair<string, int>>();

            //Nicht elegant, aber für eine Beispielaufgabe ist es ok
            foreach (string file in files)
            {
                if (file.EndsWith(".bmp"))
                {
                    byte[]? bytes;
                    bytes = ProcessFile(file);
                    var fileName = file.Substring(file.LastIndexOf("\\") + 1);
                    byteArraysDict.Add(fileName, bytes == null ? new byte[0] : bytes);
                }
            }

            foreach (var emptyKey in byteArraysDict.Keys)
            {
                //Auch hier für den Zweck einer Beispielaufgabe direkt auf dem String gearbeitet
                if (emptyKey.Contains("Leer"))
                {
                    string fullKey = emptyKey.Replace("Leer", "");

                    //Wenn sowohl Bild mit Element, als auch Vergleichsbild ohne Element gefuden werden, wird berechnet
                    if (byteArraysDict.ContainsKey(fullKey))
                    {
                        int volumeMean = ProcessDifferenceBytesMean(byteArraysDict[fullKey], byteArraysDict[emptyKey]);
                        volumeDictMean.Add(new KeyValuePair<string, int>(fullKey, volumeMean));
                    }
                }
            }

            return volumeDictMean;
        }

        private byte[]? ProcessFile(string file)
        {
            byte[]? bytes = null;

            ImageConverter converter = new ImageConverter();

            using (var image = new Bitmap(file))
            {

                if (image != null)
                {
                    bytes = (byte[]?)converter.ConvertTo(image, typeof(byte[]));
                }
            }

            return bytes;
        }

        private int ProcessDifferenceBytesMean(byte[] fullBytes, byte[] emptyBytes)
        {
            int diff = 0;

            //Berechnen der durchschnittlichen Höhe des Untergrundes für das leere Bild.
            //Nötig um bei Teilen die abstehen (z.B. Bild Z....66_Depth.bmp, der linke Teil, der übersteht) sinnvolle Werte zu erhalten
            int mean = CalculateMeanHeight(emptyBytes);

            for (int i = 0; i < fullBytes.Length; i++)
            {
                int currentFullByte = fullBytes[i];
                int currentEmptyByte = emptyBytes[i];

                //Falls für die aktuelle Stelle in dem leeren Bild keine Daten vorhanden sind, durchschnittliche Höhe verwenden
                if (currentEmptyByte == 0)
                    currentEmptyByte = mean;

                int currentDiff = currentFullByte - currentEmptyByte;

                //Falls das gefüllte Bild an dieser Stelle Werte besitzt und nicht im Schatten/Kante(?), bzw einen Bereich ohne Werte ist:
                //Den aktuellen Wert des leeren Bildes abziehen um den Wert für diesen "Pixel" zu ehalten
                //Einmal über das geasamte Bild gemacht, ergibt das das Volumen des Körpers
                if (currentFullByte > 0 && currentDiff < 0)
                {
                    diff += currentDiff;
                }
            }

            return diff;
        }

        private int CalculateMeanHeight(byte[] bytes)
        {
            int mean = 0;
            int count = 0;

            foreach (byte b in bytes)
            {
                if (b != 0)
                {
                    mean += b;
                    count++;
                }
            }

            mean = mean / count;

            return mean;
        }
    }
}
