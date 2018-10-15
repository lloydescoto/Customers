using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        string[] products = { "Siomai", "Hotdog", "Bread", "Burger", "Water" };
        List<Customer> customers = new List<Customer>();
        List<string> customersList = new List<string>();
        List<Customer> customerRecord = new List<Customer>();
        Dictionary<string, double> productQuantity = new Dictionary<string, double>();
        Dictionary<string, double> productPercentage = new Dictionary<string, double>();
        Dictionary<string, double> productRanking = new Dictionary<string, double>();
        Random rand = new Random();
        public Form1()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            panel1.MouseDown += panel1_MouseDown;
            panel1.MouseUp += panel1_MouseUp;
            panel1.MouseMove += panel1_MouseMove;
            listBox1.DrawMode = DrawMode.OwnerDrawFixed;
            listBox1.DrawItem += new DrawItemEventHandler(listBox1_DrawItem);
        }
        private bool _dragging = false;
        private Point _start_point = new Point(0, 0);
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging = true;
            _start_point = new Point(e.X, e.Y);
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Point p = PointToScreen(e.Location);
                Location = new Point(p.X - this._start_point.X, p.Y - this._start_point.Y);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
            }
            else
            {
                productQuantity.Clear();
                customerRecord.Clear();
                dataGridView3.Rows.Clear();
                customerRecord = getCustomerRecord(listBox1.SelectedItem.ToString(), customers);
                productQuantity = getProductsQuantity(products, customerRecord);
                productPercentage = getProductsPercentage(productQuantity);
                productRanking = getProductRanking(productPercentage);
                int ranking = 1;
                foreach (var prod in productRanking)
                {
                    dataGridView3.Rows.Add(ranking,prod.Key, prod.Value);
                    ranking++;
                }
            }
        }
        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                e = new DrawItemEventArgs(e.Graphics,e.Font,e.Bounds,e.Index,e.State ^ DrawItemState.Selected,e.ForeColor,Color.FromArgb(21,237,163));
            e.DrawBackground();
            e.Graphics.DrawString(listBox1.Items[e.Index].ToString(), e.Font, Brushes.White, e.Bounds, StringFormat.GenericDefault);
            e.DrawFocusRectangle();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int randProd = rand.Next(0, 5);
            int randQuan = rand.Next(1, 6);
            dataGridView2.Rows.Add(textBox1.Text, products[randProd], randQuan);
            customers.Add(new Customer(textBox1.Text, randQuan, products[randProd]));
            if(customersList.Contains(textBox1.Text))
            {
            }
            else
            {
                customersList.Add(textBox1.Text);
                listBox1.DataSource = null;
                listBox1.DataSource = customersList;
            }

        }

        public Dictionary<string,double> getProductRanking(Dictionary<string, double> ProductsPercentage)
        {
            Dictionary<string, double> productRanking = new Dictionary<string, double>();
            double[] Percent = new double[ProductsPercentage.Count];
            string[] Product = new string[ProductsPercentage.Count];
            int index = 0;
            foreach (var prod in ProductsPercentage)
            {
                Percent[index] = prod.Value;
                Product[index] = prod.Key;
                index++;
            }
            for (int x = 0; x < ProductsPercentage.Count; x++)
            {
                int minimum = x;
                for (int y = x; y < ProductsPercentage.Count; y++)
                {
                    if (Percent[minimum] < Percent[y])
                    {
                        minimum = y;
                    }
                }
                double temp = Percent[x];
                Percent[x] = Percent[minimum];
                Percent[minimum] = temp;
                string tempName = Product[x];
                Product[x] = Product[minimum];
                Product[minimum] = tempName;
            }
            for (int x = 0; x < ProductsPercentage.Count; x++)
            {
                productRanking.Add(Product[x], Percent[x]);
            }
            return productRanking;
        }

        public Dictionary<string, double> getProductsPercentage(Dictionary<string, double> ProductsQuantity)
        {
            Dictionary<string, double> productsPercentage = new Dictionary<string, double>();
            double totalQuantity = 0;
            foreach(var Prod in ProductsQuantity)
            {
                totalQuantity += Prod.Value;
            }
            foreach (var Prod in ProductsQuantity)
            {
                double Percentage = 0;
                Percentage = (Prod.Value / totalQuantity) * 100;
                productsPercentage.Add(Prod.Key, Percentage);
            }
            return productsPercentage;
        }

        public Dictionary<string,double> getProductsQuantity(string[] Products, List<Customer> CustomerRecord)
        {
            Dictionary<string, double> productsQuantity = new Dictionary<string, double>();
            for(int x = 0;x < Products.Length;x++)
            {
                double total = 0;
                for(int y = 0;y < CustomerRecord.Count;y++)
                {
                    if (Products[x] == CustomerRecord[y].Product)
                        total += CustomerRecord[y].Quantity;
                }
                productsQuantity.Add(Products[x], total);
            }

            return productsQuantity;
        }

        public List<Customer> getCustomerRecord(string Key, List<Customer> CustomersList)
        {
            List<Customer> customerRecord = new List<Customer>();
            for(int x = 0;x < CustomersList.Count; x++)
            {
                if (Key == CustomersList[x].Name)
                    customerRecord.Add(new Customer(CustomersList[x].Name, CustomersList[x].Quantity, CustomersList[x].Product));
            }
            return customerRecord;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for(int x = 0;x < products.Length; x++)
            {
                dataGridView1.Rows.Add(products[x]);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }

    public class Customer
    {
        public string Name;
        public double Quantity;
        public string Product;

        public Customer(string Name, double Quantity, string Product)
        {
            this.Name = Name;
            this.Quantity = Quantity;
            this.Product = Product;
        }
    }
}
