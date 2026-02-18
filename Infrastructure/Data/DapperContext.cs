using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            // Lấy chuỗi kết nối từ appsettings.json của project API hoặc WorkerService
            _connectionString = _configuration.GetConnectionString("OracleConnection");
        }

        public IDbConnection CreateConnection()
            => new OracleConnection(_connectionString);
    }
}
