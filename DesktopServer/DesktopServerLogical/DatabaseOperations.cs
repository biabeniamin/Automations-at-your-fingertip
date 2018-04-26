using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopServerLogical
{
    public class DatabaseOperations
    {
        private SqlConnection _connection;
        private SqlCommand _command;
        private SqlDataReader _reader;
        private SqlDataAdapter _adapter;
        private DataSet _set;
        public ConnectionState State => _connection.State;
        public DatabaseOperations()
        {
            String path;

            path = Helpers.GetDirectoryParent(System.IO.Directory.GetCurrentDirectory(), 3);
            _connection = new SqlConnection($@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={path}\DesktopServerLogical\configurations.mdf;Integrated Security=True;Connect Timeout=30");
            try
            {
                _connection.Open();
            }
            catch (Exception ee)
            {
                System.Windows.Forms.MessageBox.Show($"The database cooudn't be opened. The app will run without preconfigurations {ee.Message}");
            }
            
        }
        public SqlDataReader GetReader(string query)
        {
            if(ConnectionState.Open != _connection.State)
            {
                return null;
            }

            _command = new SqlCommand(query, _connection);
            _reader = _command.ExecuteReader();
            return _reader;
        }
        public type ReadOneValue<type>(string query)
        {
            type value = default(type);
            GetReader(query);
            if (_reader.Read())
            {
                value = (type)_reader[0];
            }
            _reader.Dispose();
            return value;
        }
        public void ExecuteQuery(string query)
        {
            if (ConnectionState.Open != _connection.State)
            {
                return;
            }
            _command = new SqlCommand(query, _connection);
            _command.ExecuteNonQuery();
        }
        public SqlDataAdapter GetAdapter(string query)
        {
            
            _command = new SqlCommand(query, _connection);
            _adapter = new SqlDataAdapter(_command);
            return _adapter;
        }
    }
}
