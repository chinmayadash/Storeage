using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Storeage.Models;
using Storeage.Services.Abstract;
using Storeage.Services.Concrete;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Linq;

namespace Storeage.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IImageService _imageService;
        public HomeController(ILogger<HomeController> logger, IImageService imageService)
        {
            _logger = logger;
            _imageService = imageService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            string connetionString = null;
            List<EmployeeModel> employeeModel = new List<EmployeeModel>();
            SqlConnection connection;
            SqlCommand command;
            string sql = null;
            SqlDataReader dataReader;
            connetionString = "Data Source=tcp:evoke12.database.windows.net;Initial Catalog=clopay;User ID=evoke12;Password=XYZ@123abcde";
            sql = "select * from SupportTeam";
            connection = new SqlConnection(connetionString);
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();

                employeeModel = DataReaderMapToList<EmployeeModel>(dataReader);

                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
            }
            return View(employeeModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SavePicture(ImageModel imageModel)
        {
            string fileExtension = Path.GetExtension(imageModel.File.FileName);
            if (imageModel.File == null || imageModel.File.FileName == null)
            {
                return View("Index");
            }

            AddEmp(imageModel);

            _imageService.UploadImageToAzure(imageModel.File, imageModel);
            return RedirectToAction("Privacy", "Home", new { area = "" });
        }

        public ActionResult Delete(string blobName)
        {
            _imageService.DeleteDocumentAsync(blobName);
            DeleteEmployee(blobName);
            return RedirectToAction("Privacy", "Home", new { area = "" });
        }

        public static List<T> DataReaderMapToList<T>(IDataReader dr)
        {
            List<T> list = new List<T>();
            T obj = default(T);
            while (dr.Read())
            {
                obj = Activator.CreateInstance<T>();
                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (!object.Equals(dr[prop.Name], DBNull.Value))
                    {
                        prop.SetValue(obj, dr[prop.Name], null);
                    }
                }
                list.Add(obj);
            }
            return list;
        }

        public void AddEmp(ImageModel imageModel)
        {
            string connetionString = null;
            SqlConnection connection;
            SqlCommand command;
            string path = "https://evstorage12.blob.core.windows.net/clopay/";
            SqlDataReader dataReader;
            connetionString = "Data Source=tcp:evoke12.database.windows.net;Initial Catalog=clopay;User ID=evoke12;Password=XYZ@123abcde";
            connection = new SqlConnection(connetionString);
            connection.Open();
            command = new SqlCommand("insert into SupportTeam values(" + imageModel.EmpID + "," + imageModel.EmpID + ",'" + imageModel.Name + "','" + imageModel.Project + "','" + path + imageModel.EmpID + Path.GetExtension(imageModel.File.FileName) + "')", connection);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                connection.Close();
            }
            finally
            {
                connection.Close();
            }
        }

        public void DeleteEmployee(string imagePath)
        {
            string connetionString = null;
            SqlConnection connection;
            SqlCommand command;
            connetionString = "Data Source=tcp:evoke12.database.windows.net;Initial Catalog=clopay;User ID=evoke12;Password=XYZ@123abcde";
            connection = new SqlConnection(connetionString);
            connection.Open();
            command = new SqlCommand("delete from SupportTeam where ImagePath = '" + imagePath + "' ", connection);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception x)
            {
                connection.Close();
            }
            finally
            {
                connection.Close();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}