using CRUD_App_With_MYSQL_VCoder.CommonLayer.Model;
using Microsoft.Extensions.Configuration;
using CRUD_App_With_MYSQL_VCoder.Common_Utility;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using MySqlConnector;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static CRUD_App_With_MYSQL_VCoder.CommonLayer.Model.UpdateInformation;
using Microsoft.Data.SqlClient;
using System.Data;
using MySql.Data.MySqlClient;



namespace CRUD_App_With_MYSQL_VCoder.RepositoryLayer
{
    public class CrudApplicationRL : ICrudApplicationRL
    {

        public readonly IConfiguration _configuration;
        public readonly ILogger<CrudApplicationRL> _logger;
        //public readonly MySqlConnection _mySqlConnection;
        public readonly SqlConnection _sqlConnection;

        public CrudApplicationRL(IConfiguration configuration,ILogger<CrudApplicationRL> logger)
        {
            _configuration = configuration;
            _logger = logger;
            //_mySqlConnection = new MySqlConnection(_configuration["ConnectionStrings:MySqlDBString"]);
            _sqlConnection = new SqlConnection(_configuration["ConnectionStrings:MySqlDBString"]);

        }

        

       

        public async Task<AddInformationResponse> AddInformation(AddInformationRequest request)
        {
            _logger.LogInformation("AddInformatio method called in repository layer");
            AddInformationResponse response = new AddInformationResponse();
            response.IsSuccess = true;
            response.Message = "Successful";
            try
            {
                if (_sqlConnection.State != ConnectionState.Open)
                {
                    await _sqlConnection.OpenAsync();
                }

                using (SqlCommand sqlCommand = new SqlCommand(SqlQueries.AddInformation, _sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    sqlCommand.CommandText = "INSERT INTO crudApplicationTable(UserId,UserName, EmailID, MobileNumber, Salary, Gender) VALUES (@UserId, @UserName, @EmailID, @MobileNumber, @Salary, @Gender);";
                    sqlCommand.CommandTimeout = 180;
                    sqlCommand.Parameters.AddWithValue("@UserId", request.UserId);
                    sqlCommand.Parameters.AddWithValue("@UserName", request.UserName);
                    sqlCommand.Parameters.AddWithValue("@EmailID", request.EmailID);
                    sqlCommand.Parameters.AddWithValue("@MobileNumber", request.MobileNumber);
                    sqlCommand.Parameters.AddWithValue("@Gender", request.Gender);
                    sqlCommand.Parameters.AddWithValue("@Salary", request.Salary);

                    int Status = await sqlCommand.ExecuteNonQueryAsync();
                    if (Status <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "AddInformation Query Not Executed";
                        _logger.LogError("Error occured Query not executed");
                        return response;
                    }
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                _logger.LogError($"Error at AddInformation Repository Layer{ex.Message}");
            }

            finally
            {
                if (_sqlConnection.State == ConnectionState.Open)
                {
                    await _sqlConnection.CloseAsync();
                }

            }
            return response;
        }





        public async Task<DeleteInformationByIDResponse> DeleteInformationById(DeleteInformationByIDRequest request)
        {
            _logger.LogInformation($"DeleteInformationByID Repository Layer Calling");
            DeleteInformationByIDResponse response = new DeleteInformationByIDResponse();
            response.IsSuccess = true;
            response.Message = "Successful";
            try
            {
                if (_sqlConnection.State != ConnectionState.Open)
                {
                    await _sqlConnection.OpenAsync();
                }

                using (SqlCommand sqlCommand = new SqlCommand(SqlQueries.DeleteInformationByID, _sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    sqlCommand.CommandText = "UPDATE CrudApplicationTable SET IsActive=0 WHERE UserId=@Id";
                    sqlCommand.CommandTimeout = 180;
                    sqlCommand.Parameters.AddWithValue("@Id", request.Id);
                    int Status = await sqlCommand.ExecuteNonQueryAsync();
                    if (Status <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "DeleteInformationByID Query Not Executed";
                        _logger.LogError("DeleteInformationByID Query Not Executed");
                    }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                _logger.LogError($"DeleteInformationByID Repository Layer Error : {ex.Message}");
            }
            finally
            {
                if (_sqlConnection.State == ConnectionState.Open)
                {
                    await _sqlConnection.CloseAsync();
                }
            }
            return response;
        }

        public async Task<ReadInformationResponse> ReadAllInformation()
           {
                _logger.LogInformation("ReadAllInformation Repository Layer Calling");
                ReadInformationResponse response = new ReadInformationResponse();
                response.IsSuccess = true;
                response.Message = "Successful";
                try
                {
                    

                    if (_sqlConnection.State != ConnectionState.Open)
                    {
                        await _sqlConnection.OpenAsync();
                    }

                    using (SqlCommand sqlCommand = new SqlCommand(SqlQueries.ReadInformation, _sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.CommandText = "SELECT * FROM CrudApplicationTable WHERE IsActive=1";
                        sqlCommand.CommandTimeout = 180;

                        using (SqlDataReader dataReader = await sqlCommand.ExecuteReaderAsync())
                        {
                            if (dataReader.HasRows)
                            {
                                response.readInformation = new List<ReadInformation>();

                                while (await dataReader.ReadAsync())
                                {
                                    var getData = new ReadInformation
                                    {
                                        UserId = dataReader["UserId"] != DBNull.Value ? Convert.ToInt32(dataReader["UserId"]) : 0,
                                        UserName = dataReader["UserName"] != DBNull.Value ? Convert.ToString(dataReader["UserName"]) : null,
                                        EmailId = dataReader["EmailId"] != DBNull.Value ? Convert.ToString(dataReader["EmailId"]) : null,
                                        Gender = dataReader["Gender"] != DBNull.Value ? Convert.ToString(dataReader["Gender"]) : null,
                                        MobileNumber = dataReader["MobileNumber"] != DBNull.Value ? Convert.ToString(dataReader["MobileNumber"]) : null,
                                        Salary = dataReader["Salary"] != DBNull.Value ? Convert.ToInt32(dataReader["Salary"]) : 0,
                                        IsActive = dataReader["IsActive"] != DBNull.Value ? Convert.ToBoolean(dataReader["IsActive"]) : false
                                    };
                                    response.readInformation.Add(getData);
                                }
                            }
                            else
                            {
                                response.Message = "No Record In Database";
                                _logger.LogWarning("No Record In Database");
                            }
                        }
                    }

                
                
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.Message = ex.Message;
                    _logger.LogError($"ReadAllInformation Repository Layer Error : {ex.Message}");
                }
                finally
                {
                    //await _mySqlConnection.CloseAsync();
                    //await _mySqlConnection.DisposeAsync();
                    if (_sqlConnection.State == ConnectionState.Open)
                    {
                        await _sqlConnection.CloseAsync();
                    }
                }
                return response;
            
             }

       

        public async Task<UpdateAllInformationByIdResponse> UpdateAllInformationById(UpdateAllInformationByIdRequest request)
        {
            _logger.LogInformation("UpdateAllInformationById Repository Layer Calling");
            UpdateAllInformationByIdResponse response = new UpdateAllInformationByIdResponse();
            response.IsSuccess = true;
            response.Message = "Successful";
            try
            {
                if (_sqlConnection.State != ConnectionState.Open)
                {
                    await _sqlConnection.OpenAsync();
                }

                using (SqlCommand sqlCommand = new SqlCommand(SqlQueries.ReadInformation, _sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    sqlCommand.CommandText = "UPDATE CrudApplicationTable SET UserName=@UserName, EmailID=@EmailID, MobileNumber=@MobileNumber, Salary=@Salary, Gender=@Gender WHERE UserId=@UserId";
                    sqlCommand.CommandTimeout = 180;
                    sqlCommand.Parameters.AddWithValue("@UserId", request.UserId);
                    sqlCommand.Parameters.AddWithValue("@UserName", request.UserName);
                    sqlCommand.Parameters.AddWithValue("@EmailId", request.EmailId);
                    sqlCommand.Parameters.AddWithValue("@MobileNumber", request.MobileNumber);
                    sqlCommand.Parameters.AddWithValue("@Gender", request.Gender);
                    sqlCommand.Parameters.AddWithValue("@Salary", request.Salary);
                    int Status = await sqlCommand.ExecuteNonQueryAsync();
                    if (Status <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "UnSuccessful Please check UserID";
                        _logger.LogError("UnSuccessful Please check UserID");
                    }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                _logger.LogError($"UpdateAllInformationById Repository Layer Error : {ex.Message}");
            }
            finally
            {
                if (_sqlConnection.State == ConnectionState.Open)
                {
                    await _sqlConnection.CloseAsync();
                }
            }
            return response;
        }




    }
}
