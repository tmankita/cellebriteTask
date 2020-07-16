using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Import information from source path- must be Exist
/// Export Contacts to file in Destination path.
/// </summary>

namespace cellebriteTask
{
    class Program
    {


        static void Main(string[] args)
        {
            string source = @"/Users/tmankita/Projects/cellebriteTask/ex_v8";
            string destination = @"/Users/tmankita/Projects/cellebriteTask/ex_v8_output";
            Extractor extractor = new Extractor();
            List<Contact> contacts = extractor.extract(source);
            try
            {
                if (File.Exists(destination))
                {
                    File.Delete(destination);
                }

                using (StreamWriter sw = new StreamWriter(destination))
                {
                    foreach (Contact contact in contacts)
                    {
                        sw.WriteLine(contact);
                        sw.WriteLine("--------------------");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }




        }
    }
}
