using System;
namespace cellebriteTask
{
    public class Contact
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public DateTime Time { get; set; }
        public int FirstNameLength { get; set; }

        public Contact()
        {
        }


        public override string ToString()
        {

            return "first name: " + FirstName + "\n" +
                    "last name: " + LastName + "\n" +
                    "phone: " + Phone + "\n" +
                    "time: " + Time.ToString();
        }
    }
}
