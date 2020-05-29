using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Web.Http;

namespace AppointmentAPI.Controllers
{
    public class GetDetailController : ApiController
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["testDb"].ToString());
        SqlCommand cmd = new SqlCommand();
        SqlDataAdapter adp = null;

        [HttpGet]
        public IHttpActionResult GetCount(string DocCode, string Date)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT 'AP' as CS, CPerSurname, AHistCPerCPI,CPerAge,CPerSex, CONVERT(VARCHAR(5), AHistStartTime,108) AS AppTime, DeptDesc, "
+ " null as Regs, null as news, null as fl, null as rm, null as otpro, null as ott "
+ " FROM [dbo].[CS_AppointmentHistory] "
+ " Left Join CPIPersonal on [AHistCPerCPI] = [CPerCPI] and [AHistInstn] = [CPerInstn] "
+ " Left Join CS_DepartmentProcedures on [AHistDProcID] = [DProcID] "
+ " Left Join Departments on [DProcDeptCode] = [DeptCode] "
+ " Left Join Doctors on [AHistAppResDoctorInfo] = [DocCode] "
+ " WHERE AHistAppResDoctorInfo = '"+ DocCode+"' and  datediff(day, [AHistStartTime], '"+Date+"') = 0 and AHistStsID != 4 "
+ " Union "
+ " SELECT 'O' as OPD, CPerSurname, ORegCPI, CPerAge,CPerSex, null as at, null as ad, ORegRegNum, "
+ " EWS = (select top 1 EAssEWS from WB_EmergReassessments where EAssPAID = ORegPAID and EAssDeletedDate is null and EAssType in (3,4) order by EAssDate desc), "
+ " null as fl, ORegRoom, null as otpro, null as ott "
+ " FROM [dbo].[OutpatientRegister] Left Join CPIPersonal on [ORegCPI] = [CPerCPI] and ORegInstn = [CPerInstn] "
+ " Where ORegAttDoc = '"+DocCode+"' and datediff(day, ORegDate, '"+Date+"') = 0 and ORegInstn= 'AMM0000' and ORegCanDate is Null  "
+ " Union "
+ " SELECT 'A' as IPD, CPerSurname, ARegCPI,CPerAge,CPerSex,null as at, null as ad, ARegRegNum, "
+ " EWS = (select top 1 EAssEWS from WB_EmergReassessments where EAssPAID = ARegPAID and EAssDeletedDate is null and EAssType in (3,4) order by EAssDate desc), "
+ " RoomWard, RHistRoom,null as otpro, null as ott "
+ " FROM [dbo].AdmitRegister Left Join CPIPersonal on ARegCPI = [CPerCPI] and ARegInstn = [CPerInstn] Left Join RoomHistory on ARegRegNum = RHistRegNum and ARegInstn = RHistInstn "
+ " Join Rooms on RHistRoom = RoomNum "
+ " Where  ([ARegAttDoc] ='"+DocCode+"' or  ARegRefDoc ='"+DocCode+"' or ARegFamDoc = '"+DocCode+"') and datediff(day, ARegDate, '"+Date+"') = 0 and ARegInstn= 'AMM0000' and ARegCancelDate is Null "
+ " Union "
+ " SELECT 'OR' as opr, CPerSurname, ORegCPI, CPerAge,CPerSex, null as apt, null as ad,ORegRegNum, "
+ " EWS = (select top 1 EAssEWS from WB_EmergReassessments where EAssPAID = ORegPAID and EAssDeletedDate is null and EAssType in (3,4) order by EAssDate desc), "
+ " null as fl, ThrDesc, OPrcDesc, CONVERT(VARCHAR(5), ORegSchStart,108) AS OTTime "
+ " FROM OperatingRegister Left Join CPIPersonal on ORegCPI = [CPerCPI] and ORegInstn = [CPerInstn] Left Join ORTheatre on ORegTheatre = ThrCode "
+ " left Join OperationWork on ORegRegNum = OWrkRegNum and ORegInstn = OWrkInstn "
+ " left Join ORProcedures on OPrcCode = OWrkProc "
+ " Left Join OperationDocs on ORegRegNum = ODocRegNum and ORegInstn = ODocInstn"
+ " where ODocDoc = '"+DocCode+"' and ORegInstn = 'AMM0000' and datediff(day, ORegDate, '"+Date+"') = 0 and ORegCanDate is null";

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
                dt.TableName = "GetDetail";
                adp.Fill(dt);
                con.Close();
            }
            catch
            {
                return Content(HttpStatusCode.BadRequest, "Pararmeter Not found!");
            }
            
            return Ok(dt);
        }
    }
}
