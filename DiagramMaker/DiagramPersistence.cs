using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramClient
{
    public class DiagramPersistence
    {
        public DiagramPersistence()
        {

        }

        public void SaveSvgToFile(string svgContent, string filePath)
        {
            string newFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);

            try
            {
                File.WriteAllText(filePath, svgContent, Encoding.UTF8);
                Console.WriteLine($"SVG diagram successfully saved to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while saving the SVG: {ex.Message}");
            }
        }
    }
}
