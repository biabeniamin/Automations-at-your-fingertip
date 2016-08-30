using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopServerLogical
{
    public class DatabaseOperations
    {
        private OleDbConnection _connection;
        private OleDbCommand _command;
        private OleDbDataReader _reader;
        private OleDbDataAdapter _adapter;
        private DataSet _set;
        public DatabaseOperations()
        {
            _connection = new OleDbConnection("Provider=Microsoft.Ace.Oledb.12.0;Data Source=db.accdb");
            _connection.Open();
        }
        public OleDbDataReader GetReader(string query)
        {
            _command = new OleDbCommand(query, _connection);
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
            _command = new OleDbCommand(query, _connection);
            _command.ExecuteNonQuery();
        }
        public OleDbDataAdapter GetAdapter(string query)
        {
            _command = new OleDbCommand(query, _connection);
            _adapter = new OleDbDataAdapter(_command);
            return _adapter;
        }
    }
}
