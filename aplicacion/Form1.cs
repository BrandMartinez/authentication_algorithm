using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.IO;
using System.Collections;

//@author: Brandán Martínez

namespace aplicacion
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Run();
            generator = RandomNumberGenerator.Create();
        }

        private RandomNumberGenerator generator;
        private Dictionary<String, String[]> table = new Dictionary<string, string[]>();
        private int iterations = 10000;


        private void button1_Click(object sender, EventArgs e)  //Register Button
        {
            string username = textBox1.Text;
            string password = textBox2.Text;

            if (table.ContainsKey(username))
            {
                label3.Text = "This name is already registered";
            }
            else
            {
                int salt = GetNext();
                int tempsalt = salt;
                var tempiterations = sha256(password + salt.ToString());
                for (int i = 0; i < iterations; i++)
                {
                    tempsalt = tempsalt + i;
                    tempiterations = sha256(tempiterations + tempsalt.ToString());
                }

                string[] words = new string[5];
                words[0] = iterations.ToString();
                words[1] = salt.ToString();
                words[2] = tempiterations;

                table.Add(username, words);
                Write();
                label3.Text = "You have been registered";
            }
        }

        private void button2_Click(object sender, EventArgs e) //Log In button
        {
            string username = textBox1.Text;
            string password = textBox2.Text;
            if (table.ContainsKey(username))
            {
                string[] words = new string[5];
                words = table[username];
                int tempsalt;
                int tempo;
                Int32.TryParse(words[1], out tempsalt);
                var tempiterations = sha256(password + tempsalt);
                Int32.TryParse(words[0], out tempo);
                for (int i = 0; i < tempo; i++)
                {
                    tempsalt = tempsalt + i;
                    tempiterations = sha256(tempiterations + tempsalt.ToString());
                }


                if (words[2] == tempiterations)
                {
                    label3.Text = "You logged in as " + username;
                }
                else
                {
                    label3.Text = "The password is wrong";
                }
            }
            else
            {
                label3.Text = "That name is not registered";
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {}

        private void label2_Click(object sender, EventArgs e)
        {}

        

        private int GetNext()
        {
            byte[] rndArray = new byte[4];
            generator.GetBytes(rndArray);
            return BitConverter.ToInt32(rndArray, 0);
        }

        private void Run()
        {
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader("passwords.txt"))
                {
                    string line = "";
                    string[] words = new string[5];
                    string usr = "";
                    int j = -1;


                    while ((line = sr.ReadLine()) != null)
                    {
                        if (j==-1)
                        {
                            usr = line;
                        }
                        else
                        {
                            words[j] = line;
                        }
                        j++;
                        if (j == 3)
                        {
                            table.Add(usr, words);
                            words = new string[5];
                            j = -1;
                        }
                    }
                    
                    //string[] temp = table[usr];
                   // label3.Text = temp[2];

                }
            }
            catch (Exception e)
            {
                label3.Text = "The file could not be read:";
                Console.WriteLine(e.Message);
            }


        }

        private void Write()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter("passwords.txt"))
            {
                foreach (KeyValuePair<string, string[]> entry in table)
                {
                    // If the line doesn't contain the word 'Second', write the line to the file.
                    string[] temp = new string[5];
                    temp = entry.Value;

                    file.WriteLine(entry.Key);
                    file.WriteLine(temp[0]);
                    file.WriteLine(temp[1]);
                    file.WriteLine(temp[2]);


                }
            }
        }


        private static string sha256(string randomString)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

        
    }


}
