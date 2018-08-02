using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Runtime.InteropServices;
using IronPython.Hosting;
using System.Diagnostics;
using System.IO;
using Microsoft.Scripting.Hosting;
using System.Text;
namespace Project.Controllers
{
     public class HomeController : Controller
    {
        SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-8U502A3\SQLSERVER;Initial Catalog=Project;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework");
        SqlDataAdapter da;
        int nousr=0, nopro=0;
        int p = 0;
        int[] productids = new int[100];
        [DllImport("libtest.so", EntryPoint = "print")]
        static extern void print(string message);
        int budget = 0, capacity = 0;
        List<ProductTable> li = new List<ProductTable>();

        DataSet GetData()
        {
            da = new SqlDataAdapter();
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            da.SelectCommand = new SqlCommand("SELECT * FROM LoginRegTable", conn);
            da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            da.Fill(ds, "LoginRegTable");
            return ds;
        }
        DataSet GetProduct(String catname)
        {
            da = new SqlDataAdapter();
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            da.SelectCommand = new SqlCommand("SELECT * FROM "+catname, conn);
            da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            da.Fill(ds, catname);
            return ds;
        }
        double[,] a = new double[100, 100];
        DataSet FindUser(string a)
        {
            da = new SqlDataAdapter();
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            da.SelectCommand = new SqlCommand("SELECT * FROM UserRegistration where ProductLocation='"+a+"'", conn);
            da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            da.Fill(ds, "UserRegistration");
            return ds;
        }
        void GetProductAsLoc(string cagtname,string loc)
        {
            da = new SqlDataAdapter();
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            da.SelectCommand = new SqlCommand("SELECT [ProductId] FROM[dbo].["+cagtname+"] where[ProductLocation] = '"+loc+"'", conn);
            da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            da.Fill(ds, cagtname);
            string line;
            int lis = 0;
            System.IO.StreamReader file = new System.IO.StreamReader(@"F:\Zensar\Project\Project\OutputPy.txt");
            while ((line = file.ReadLine()) != null)
            {
                productids[lis] = Convert.ToInt32(line);
                lis++;
            }
            DataSet dis = GetProduct(cagtname);
            int ind = 0;
            while(ind<lis)
            { 
                foreach (DataRow dir in dis.Tables[cagtname].Rows)
                {
                    if (productids[ind] == Convert.ToInt32(dir[0]))
                    {
                        if ((budget == 0 || budget >= Convert.ToInt32(dir[2])) && (capacity == 0 || capacity <= Convert.ToInt32(dir[3])))
                        { 
                            li.Add(new ProductTable { Productid = Convert.ToInt16(dir[0]), ProductName = dir[1].ToString(), ProductPriceRange = Convert.ToInt32(dir[2]), ProductCapacityRange = Convert.ToInt32(dir[3]), ProductLocation = Convert.ToString(dir[4]), ProductAdress = Convert.ToString(dir[5]), ProductImagePath = dir[6].ToString(), ProductImagePath1 = dir[7].ToString(), ProductImagePath2 = dir[8].ToString(), ProductImagePath3 = dir[9].ToString(), ProductImagePath4 = dir[10].ToString(), ProductDetails = dir[11].ToString() });
                                break;
                        }
                    }
                }
                ind++;
            }
            
            ViewBag.prodlist = li;
        }

        void pyth()
        {
            var engine = Python.CreateEngine(); // Extract Python language engine from their grasp
            var scope = engine.CreateScope(); // Introduce Python namespace (scope)

            ScriptSource source = engine.CreateScriptSourceFromFile(@"F:\Zensar\Project\Project\algorithm.py"); // Load the script
            object result = source.ExecuteProgram();
            
        }

        public ActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LogIn(LoginRegTable l)
        {
            DataSet ds = GetData();
            foreach (DataRow dr in ds.Tables["LoginRegTable"].Rows)
            {
                if (Convert.ToString(dr["usrEmail"]) == l.usrEmail)
                {
                    if (Convert.ToString(dr["usrPasswd"]) == l.usrPasswd)
                    {
                        ViewBag.User = dr["usrFirstName"];
                        Session["usrid"] =Convert.ToInt32(dr["usrid"]);
                        return View("Category");
                    }
                    else
                    {
                        ViewBag.logError = "Incorrect Password";
                        return View();
                    }
                }
            }
            ViewBag.logError = "Email is not registered.";
            return View();
        }
        public ActionResult Category()
        {
            ViewBag.User = Session["User"];
            ViewBag.User = Session["User"];
            return View();
        }
        public ActionResult Recommendation()
        {
             Session["p"] = 0;
            return View();
        }
        public ActionResult SignUp(LoginRegTable l)
        {
            ViewBag.passerror = "";
            if ((l.usrFirstName != null) && (l.usrEmail != null) && (l.usrLastName != null) && (l.usrPasswd != null) && (l.usrCity != null))
            {
                if (l.usrPasswd.Equals(l.usrConfPasswd))
                {
                    DataSet ds = GetData();
                    try
                    {
                        DataRow dr = ds.Tables["LoginRegTable"].NewRow();
                        dr["usrFirstName"] = l.usrFirstName;
                        dr["usrLastName"] = l.usrLastName;
                        dr["usrEmail"] = l.usrEmail;
                        dr["usrPasswd"] = l.usrPasswd;
                        dr["usrDOB"] = l.usrDOB;
                        dr["usrCity"] = l.usrCity;
                        ds.Tables["LoginRegTable"].Rows.Add(dr);
                        SqlCommandBuilder cmb = new SqlCommandBuilder(da);
                        int a = da.Update(ds.Tables["LoginRegTable"]);
                        if (a > 0)
                        {
                            return View("LogIn");
                        }
                        else
                        {
                            ViewBag.logError = "Something is not right" + l.usrPasswd + " " + l.usrConfPasswd;
                            ViewBag.dis = "display";
                            return View("LogIn");
                        }
                    }
                    catch (FormatException f)
                    {
                        ViewBag.logError = "Something is not right" + l.usrPasswd + " " + l.usrConfPasswd;
                        ViewBag.dis = "display";
                        return View("LogIn");
                    }
                }

                else
                {
                    ViewBag.logError = "Password do not match"+l.usrPasswd+" "+l.usrConfPasswd;
                    ViewBag.dis = "display";
                    return View("LogIn");
                }
            }
            else
            {
                ViewBag.logError = "All fields must be filled";
                ViewBag.dis = "display";
                return View("LogIn");
            }
        }

        List<string> l1 = new List<string>();
        void CreateMatrix()
        {
            int Found = 0;
            int K = 2;
            a[0, 0] = 0;
            DataSet usr = FindUser("Pune");
            a[0, 1] =Convert.ToDouble(Session["usrid"]);
            foreach (DataRow i in usr.Tables["UserRegistration"].Rows)
            {
                for (int j = 1; j < 100; j++)
                {
                    if ((int)i["usrid"] == a[0, j])
                    {
                        Found = 1;
                        break;
                    }
                }
                if (Found == 0)
                {
                    a[0, K] = (int)i["usrid"];
                    K++;
                }
                Found = 0;
                nousr += 1;
            }
            nousr = K-1;
            K = 1;
            foreach (DataRow i in usr.Tables["UserRegistration"].Rows)
            {
                for (int j = 1; j < 100; j++)
                {
                    if ((int)i["Productid"] == a[j, 0])
                    {
                        Found = 1;
                        break;
                    }
                }
                if (Found == 0)
                {
                    a[K, 0] = (int)i["Productid"];
                    K++;
                }
                Found = 0;
                
            }
            nopro = K-1;
            int p=0, q=0;
            foreach (DataRow i in usr.Tables["UserRegistration"].Rows)
            {
                for (int j = 1; j < 100; j++)
                {
                    if ((int)i["usrid"] == a[0, j])
                    {
                        p = j;
                        break;
                    }
                }
                for (int j = 1; j < 100; j++)
                {
                    if ((int)i["Productid"] == a[j, 0])
                    {
                        q = j;
                        break;
                    }

                }
                a[q,p] = (double)i["ratings"];
                Found = 0;
            }
        }
        void writeData()
        {
            CreateMatrix();
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"F:\Zensar\Project\Project\s.txt");
            for(int i=0;i<=nopro;i++)
            {
                string str = "";
                for (int j=0;j<=nousr;j++)
                {
                    str +=Convert.ToString(a[i,j]) + ",";
                }
                file.WriteLine(str);
            }
            file.Close();
        }
        String otp()
        {
            System.IO.StreamReader file = new System.IO.StreamReader(@"c:\test.txt");
            String line = file.ReadLine();
            file.Close();
            return line;
        }
        public ActionResult HallsAction()
        {
            Session["Category"] = "Halls_data";
            ViewBag.Category = "Halls";
            writeData();
            pyth();
            GetProductAsLoc("Halls_data","Pune");
            ViewBag.User = Session["usrid"];
            ViewBag.name1 = a;
            return View("Recommendation");
        }
        
        public ActionResult CateringAction()
        {
            Session["Category"] = "Caters_data";
            ViewBag.Category = "Caters";
            writeData();
            pyth();
            GetProductAsLoc("Caters_data", "Pune");
            ViewBag.User = Session["usrid"];
            ViewBag.name1 = a;
            return View("Recommendation");
        }
        public ActionResult DecoratorAction()
        {
            Session["Category"] = "Design_data";
            ViewBag.Category = "Decorator";
            writeData();
            pyth();
            GetProductAsLoc("Design_data", "Pune");
            ViewBag.User = Session["usrid"];
            ViewBag.name1 = a;
            return View("Recommendation");
        }

        public ActionResult CombosAction()
        {
            ViewBag.User = Session["usrid"];
            List<string> l1 = new List<string>();
            l1.Add("Select Event Type");
            l1.Add("Anniverary");
            l1.Add("Birthday");
            l1.Add("Marriage");
            ViewData["l1"] = new SelectList(l1);
            ViewBag.Category = "Combo";
            return View("Recommendation");
        }
       
        public ActionResult AboutCategory(int id,string category)
        {
            DataSet ds;
            ViewBag.Category = category;
            ViewBag.id = id;
            ViewBag.User = Session["User"];
            ds = GetProduct(Session["Category"].ToString());
            if (Session["Category"].ToString() == "Caters_data")
            {
                ViewBag.butt = "Show";
            }
            foreach (DataRow dr in ds.Tables[Session["Category"].ToString()].Rows)
            {
                if(Convert.ToInt32(dr[0])==id)
                {
                    ViewBag.item1 = dr[6];
                    ViewBag.item2 = dr[7];
                    ViewBag.item3 = dr[8];
                    ViewBag.item4 = dr[9];
                    ViewBag.item5 = dr[10];
                    ViewBag.name1 = dr[1];
                    ViewBag.price1 = dr[2];
                    ViewBag.capacity = dr[3];
                    ViewBag.details = dr[11];
                    ViewBag.addr = dr[5];
                    ViewBag.Location = dr[4];
                    Session["Productid"] = dr[0];
                    Session["Location"] = dr[4];
                    Session["Prize"] = dr[2];
                    ViewBag.SwPrize = Convert.ToInt32(dr[2]) *  2 / 3600;
                    ViewBag.StPrize = Convert.ToInt32(dr[2]) * 2 / 3600;
                    ViewBag.MCPrize = Convert.ToInt32(dr[2]) *2/ 1200;
                    ViewBag.DPrize = Convert.ToInt32(dr[2]) *2/ 3600;

                }
            }
            return View();
        }
        public ActionResult Filter( ProductTable pd)
        {
            capacity = pd.ProductCapacityRange;
            budget = pd.ProductPriceRange;
            GetProductAsLoc(Session["Category"].ToString(),"Pune");
            return View("Recommendation");
        }
        public ActionResult incre()
        {
            Session["p"] = Convert.ToInt32(Session["p"]) + 1;
            int i=1;
            ViewBag.Category = Session["Category"];
            GetProductAsLoc(Session["Category"].ToString(),"Pune");
            List<ProductTable> it = new List<ProductTable>();
            foreach(ProductTable q in li)
            {
                if(Convert.ToInt32(Session["p"])*5<i && i< Convert.ToInt32(Session["p"]) * i+5)
                {
                    it.Add(q);
                }
                i++;
            }
            ViewBag.prodlist = it;
            return View("Recommendation");
        }       
         public ActionResult About(FormCollection fc)
        {
            int sum=0;
            if(fc["Dessert"].ToString().Contains("true"))
            { 
            sum+=Convert.ToInt32(Session["Prize"]) * 2 / 3600;
            }
            if (fc["Sweet"].ToString().Contains("true"))
            {
                sum += Convert.ToInt32(Session["Prize"]) * 2 / 3600;
            }
            if (fc["Starter"].ToString().Contains("true"))
            {
                sum += Convert.ToInt32(Session["Prize"]) * 2 / 3600;
            }
            if (fc["MainCoarse"].ToString().Contains("true"))
            {
                sum += Convert.ToInt32(Session["Prize"]) * 2 / 1200;
            }
            
            if (Session["Category"].ToString() == "Caters_data")
            {
                sum *= 300;
                ViewBag.a = sum;
            }
            return View("AboutCategory");
        }

        
        public ActionResult SubmitRating()
        {
            List<string> l1 = new List<string>();
            l1.Add("Select Ratings for Product");
            l1.Add("1");
            l1.Add("2");
            l1.Add("3");
            l1.Add("4");
            l1.Add("5");
            ViewData["l1"] = new SelectList(l1);

            return View();
        }
        public ActionResult Payment()
        {
            return View();
        }
        public ActionResult Abts()
        {
            return View();
        }
        public ActionResult Serv()
        {
            return View();
        }
        public ActionResult cont()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SubmitRating(FormCollection fc)
        {
            
            da = new SqlDataAdapter();
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            da.SelectCommand = new SqlCommand("SELECT * FROM UserRegistration", conn);
            da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            da.Fill(ds, "UserRegistration");

            try
            {
                DataRow dr = ds.Tables["UserRegistration"].NewRow();
                dr["usrid"] = Session["usrid"];
                dr["Productid"] = Session["Productid"];
                dr["ProductLocation"] = Session["Location"];
                dr["ratings"] = Convert.ToInt32(fc["l1"]);

                ds.Tables["UserRegistration"].Rows.Add(dr);
                SqlCommandBuilder cmb = new SqlCommandBuilder(da);
                int a = da.Update(ds.Tables["UserRegistration"]);
                if (a > 0)
                {
                    return View("Category");
                }
                else
                {
                    return View();
                }
            }
            catch (FormatException f)
            {
                return View();
            }
        }

    }
}