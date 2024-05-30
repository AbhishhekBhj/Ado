using Webapiwithado.Models;
using System.Data.SqlClient;
using System.Reflection.Metadata;
using Microsoft.Extensions.Configuration;
using System.Data;
using Newtonsoft.Json;

namespace Webapiwithado.DataAccess
{
    public class WidgetDataAcess
    {
        private readonly string _connectionString;


        

        public WidgetDataAcess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

       
        public async Task<ResponseModel> InsertWidgetAsync(Widget widget)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    // Attempt to open the database connection
                    await connection.OpenAsync();

                    using (SqlCommand sqlCommand = new SqlCommand("insertnewwidget", connection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        sqlCommand.Parameters.AddWithValue("@WidgetName", widget.WidgetName);
                        sqlCommand.Parameters.AddWithValue("@WidgetDescription", widget.WidgetDescription);
                        sqlCommand.Parameters.AddWithValue("@WidgetSampleCode", widget.WidgetSampleCode);

                        // Execute the command
                   int rowsAffected =      await sqlCommand.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                                ResponseModel responseModel = new ResponseModel
                                {
                                    Message = "Success",
                                    Status = 200,
                                    Data = JsonConvert.SerializeObject(widget)
                                };
                                return responseModel;
                            }
                            else
                        {
                                ResponseModel responseModel = new ResponseModel
                                {
                                    Message = "Failed",
                                    Status = 500,
                                    Data = JsonConvert.SerializeObject("Failed to insert widget")
                                };
                                return responseModel;   
                        }
                         
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Log the SQL exception for further investigation
                Console.WriteLine($"SQL error occurred while inserting widget: {sqlEx.Message}");
                ResponseModel responseModel = new ResponseModel
                {
                    Message = "Failed",
                    Status = 500,
                    Data = JsonConvert.SerializeObject(sqlEx.Message)

                };
                
                throw;
            }
            catch (InvalidOperationException invalidOpEx)
            {
                // Log the InvalidOperationException for further investigation
                Console.WriteLine($"Invalid operation error occurred while inserting widget: {invalidOpEx.Message}");
                ResponseModel responseModel = new ResponseModel
                {
                    Message = "Failed",
                    Status = 500,
                    Data = JsonConvert.SerializeObject(invalidOpEx.Message)

                };
                throw;
            }
            catch (Exception ex)
            {
                // Log any other exceptions that may occur
                Console.WriteLine($"An error occurred while inserting widget: {ex.Message}");
                ResponseModel responseModel = new ResponseModel
                {
                    Message = "Failed",
                    Status = 500,
                    Data = JsonConvert.SerializeObject(ex.Message)

                };
                throw;
            }
        }


        public async Task<ResponseModel> GetWidgets()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    // Opening the database connection
                    await connection.OpenAsync();

                    string query = "SELECT * FROM hamroquizapp.dbo.Widget";

                    using (SqlCommand sqlCommand = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync())
                        {
                            List<Widget> allWidgets = new List<Widget>();

                            // Until the data reader is reading 
                            while (await sqlDataReader.ReadAsync())
                            {
                                
                                // Add read data to the list
                                allWidgets.Add(new Widget
                                {
                                    WidgetId = sqlDataReader.GetInt32(0),
                                    WidgetName = sqlDataReader.GetString(1) ?? "",
                                    WidgetDescription = sqlDataReader.GetString(2) ?? "",
                                    WidgetSampleCode = sqlDataReader.GetString(3) ?? ""
                                });
                            }
                            ResponseModel responseModel = new ResponseModel { 
                            Message = "Success",
                            Status = 200,
                            Data = allWidgets
                            
                            };

                            return responseModel;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                Console.WriteLine($"An error occurred while retrieving widgets: {ex.Message}");
                ResponseModel responseModel = new ResponseModel
                {
                    Message = "Failed",
                    Status = 500,
                    Data = JsonConvert.SerializeObject(ex.Message)

                };
                
                throw; // Re-throw the exception to propagate it upwards
            }
        }

    }
}
