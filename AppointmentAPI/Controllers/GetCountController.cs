using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Web.Http;

namespace AppointmentAPI.Controllers
{
    public class GetCountController : ApiController
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["testDb"].ToString());
        SqlCommand cmd = new SqlCommand();
        SqlDataAdapter adp = null;

       [HttpGet]
       public IHttpActionResult GetCount(string DocCode, int month, int year)
        {
            //int month = 11; //int year = 2019;
            DataTable dt = new DataTable();
            string query = @";With cs(c1,csCount,day,month,year)"
+ " AS("
+ " select 'CS' as CS, count(*), Day([AHistStartTime]), MONTH([AHistStartTime]), YEAR([AHistStartTime])"
+ " FROM[dbo].[CS_AppointmentHistory]"
+ " Left Join CPIPersonal on[AHistCPerCPI] = [CPerCPI] and[AHistInstn] = [CPerInstn]"
+ " Left Join CS_DepartmentProcedures on[AHistDProcID] = [DProcID]"
+ " Left Join Departments on[DProcDeptCode] = [DeptCode]"
+ " Left Join Doctors on[AHistAppResDoctorInfo] = [DocCode]"
+ " WHERE AHistAppResDoctorInfo = '"+DocCode+"' and MONTH([AHistStartTime])= '"+month+"' and YEAR([AHistStartTime])= '"+year+"' and AHistStsID != 4"
+ " Group by Day([AHistStartTime]), MONTH([AHistStartTime]), YEAR([AHistStartTime]) ),"
+ " opd(c1, opdCount, day, month, year)"
+ " As("
 + " select 'OPD' as OPD, count(*), Day(ORegDate), Month(ORegDate), Year(ORegDate) FROM[dbo].[OutpatientRegister]"
 + " Where ORegAttDoc = '"+DocCode+"' and"
 + " Month(ORegDate) = "+month+" and Year(ORegDate) = "+year+" and ORegInstn = 'AMM0000' and ORegCanDate is Null"
 + " Group by Day(OregDate), Month(ORegDate), Year(ORegDate)),"
 
+ " ipd(c1, ipdCount, day, month, year)"
 + " As("
 + " select 'IPD' as IPD, count(*), Day(ARegDate), Month(ARegDate), Year(ARegDate) FROM[dbo].AdmitRegister"
 + " Where ([ARegAttDoc] = '"+DocCode+ "' or  ARegRefDoc = '" + DocCode + "' or ARegFamDoc = '" + DocCode + "') and"
 + " Month(aRegDate) = "+month+" and Year(aRegDate) = "+year+" and ARegInstn = 'AMM0000' and ARegCancelDate is Null"
 + " Group by Day(AregDate), Month(ARegDate), Year(ARegDate)),"
+ "  orr(c1, otCount, day, month, year)"
+ " As("
+ " select 'OR' as opr, count(*), day(ORegSchStart), month(ORegSchStart), year(ORegSchStart)"

+ " from OperatingRegister where ORegOrdBy = '"+DocCode+"' and ORegInstn = 'AMM0000' and month(ORegSchStart) = "+month+" and  year(ORegSchStart) = "+year+""
+ " Group by day(ORegSchStart), month(ORegSchStart), year(ORegSchStart) )"

+ " select cs.csCount,opd.opdCount,ipd.ipdCount,orr.otCount,(COALESCE(opd.day, cs.day, ipd.day, orr.day))as day,(COALESCE(opd.month, cs.month, ipd.month, orr.month))as month,(COALESCE(opd.year, cs.year, ipd.year, orr.year))as year"
 + " from opd full join ipd on opd.day = ipd.day and opd.month = ipd.month and opd.year = ipd.year"

+ "    full join cs on  opd.day = cs.day and opd.month = cs.month and opd.year = cs.year"

 + "   full join orr on opd.day = orr.day and opd.month = orr.month and opd.year = orr.year";

            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = query;
                cmd.Connection = con;
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                adp = new SqlDataAdapter(cmd);
                dt.TableName = "GetAllCount";
                adp.Fill(dt);
                con.Close();
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

          return  Ok( dt );
        }
    }
}
