using System.Web.Http;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;

namespace AppointmentAPI.Controllers
{
    public class LoginController : ApiController
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["testDb"].ToString());
        SqlCommand cmd = new SqlCommand();
        SqlDataAdapter adp = null;
        DataTable dt = new DataTable();

        /**
        [HttpGet]
        [ActionName("getUsers")]
        public DataTable Get()
        {
            
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select HHUUserID, HHUPasswd, DocCode, DocSurname, DocGiven from HHUsers Join Doctors on HHUUserID = DocUserID";
                cmd.Connection = con;
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                adp = new SqlDataAdapter(cmd);
                dt.TableName = "DocotorLogin";
                adp.Fill(dt);
                con.Close();
            }
            catch
            {

            }
            return dt;    
        }
       **/
       
    [HttpPost]
    public IHttpActionResult Lgin(string UsrID, string password)
    {
         //   DataTable dt = new DataTable();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select DocCode, DocSurname, DocGiven from HHUsers Join Doctors on HHUUserID = DocUserID WHERE HHUUserID ='" + UsrID + "' and HHUPasswd='" + password +"'";
                cmd.Connection = con;
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
    
                adp = new SqlDataAdapter(cmd);
            //  dt.TableName = "EmployeeLogin1";
                adp.Fill(dt);
                con.Close();
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            if (dt == null ) {
               return Content(HttpStatusCode.NotFound, "Data not match!");
            }
            return Ok(dt);
     }


    }
}