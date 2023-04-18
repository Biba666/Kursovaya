﻿using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;
using Newtonsoft.Json;

namespace Kursovaya
{
    class DatabaseConnection
    {
        private readonly NpgsqlConnection conn = new NpgsqlConnection("Host=localhost; Port=5432; Database=postgres; Username=postgres; Password=1");

        public bool RunCommand(string command)
        {
            conn.Open();

            if (conn.State == System.Data.ConnectionState.Broken) {
                return false;
            }

            _ = new NpgsqlCommand(command, conn).ExecuteNonQuery();

            conn.Close();

            return true;
        }

        public string GetUsername(string login)
        {
            conn.Open();

            if (conn.State == System.Data.ConnectionState.Broken)
            {
                return null;
            }

            NpgsqlDataReader ndr = new NpgsqlCommand(
                "SELECT u.surname || ' ' || u.name || ' ' || coalesce(u.patronymic, '') " +
                "FROM public.user u " +
                "WHERE u.login = '" + login + "';",
                conn
            ).ExecuteReader();

            if (!ndr.Read())
            {
                return null;
            }

            string data = ndr.GetString(0);

            conn.Close();

            return data;

        }
    }
}