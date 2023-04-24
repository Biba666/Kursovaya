using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;
using Newtonsoft.Json;
using System.Net.Http;

namespace Kursovaya
{
    public class DbHistoryRow
    {
        public int GameId { get; set; }
        public string Side { get; set; }
        public float Amount { get; set; }
        public string Status { get; set; }

    }

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

        public bool AddMoney(float money)
        {
            conn.Open();

            if (conn.State == System.Data.ConnectionState.Broken)
            {
                return false;
            }

            _ = new NpgsqlCommand("UPDATE public.user SET balance = balance + round(@money::numeric, 2) WHERE login = @login;", conn)
            {
                Parameters =
                {
                    new("money", NpgsqlTypes.NpgsqlDbType.Real)
                    {
                        Value = money
                    },
                    new("login", NpgsqlTypes.NpgsqlDbType.Varchar)
                    {
                        Value = "buba"
                    }
                }
            }.ExecuteNonQuery();

            conn.Close();

            return true;
        }

        public bool UpdateBalance(float money)
        {
            conn.Open();

            if (conn.State == System.Data.ConnectionState.Broken)
            {
                return false;
            }

            _ = new NpgsqlCommand("UPDATE public.user u SET balance = round(@money::numeric, 2) WHERE u.login = @login;", conn)
            {
                Parameters =
                {
                    new("money", NpgsqlTypes.NpgsqlDbType.Real)
                    {
                        Value = money
                    },
                    new("login", NpgsqlTypes.NpgsqlDbType.Varchar)
                    {
                        Value = "buba"
                    }
                }
            }.ExecuteNonQuery();

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

        public float GetBalance(string login)
        {
            conn.Open();

            if (conn.State == System.Data.ConnectionState.Broken)
            {
                return -1;
            }

            NpgsqlDataReader ndr = new NpgsqlCommand(
                "SELECT u.balance " +
                "FROM public.user u " +
                "WHERE u.login = '" + login + "';",
                conn
            ).ExecuteReader();

            if (!ndr.Read())
            {
                return -1;
            }

            float data = (float)ndr.GetDouble(0);

            conn.Close();

            return data;
        }

        public List<Match> GetMatches(string login, string historyType)
        {
            conn.Open();

            if (conn.State == System.Data.ConnectionState.Broken)
            {
                return null;
            }

            string command;
            
            switch (historyType)
            {
                case "Все":
                    command = "SELECT b.game_id, b.side, b.amount, 'on-going' AS status " +
                        "FROM public.user u " +
                        "JOIN public.bet b ON b.user_id = u.id " +
                        "WHERE u.login = @login " +
                        "UNION ALL " +
                        "SELECT b.game_id, b.side, b.amount, 'finished' AS status " +
                        "FROM public.user u " +
                        "JOIN history.bet b ON b.user_id = u.id " +
                        "WHERE u.login = @login;";
                    break;
                case "Прошедшие матчи":
                    command = "SELECT b.game_id, b.side, b.amount, 'finished' AS status " +
                        "FROM public.user u " +
                        "JOIN history.bet b ON b.user_id = u.id " +
                        "WHERE u.login = @login;";
                    break;
                case "Грядущие матчи":
                    command = "SELECT b.game_id, b.side, b.amount, 'on-going' AS status " +
                        "FROM public.user u " +
                        "JOIN public.bet b ON b.user_id = u.id " +
                        "WHERE u.login = @login;";
                    break;
                default:
                    return null;
            }

            NpgsqlDataReader reader = new NpgsqlCommand(command, conn)
            {
                Parameters =
                        {
                            new("login", NpgsqlTypes.NpgsqlDbType.Varchar)
                            {
                                Value = login
                            }
                        }
            }.ExecuteReader();

            //Get data from db
            List<DbHistoryRow> table = new();
            while (reader.Read())
            {
                table.Add(new DbHistoryRow
                {
                    GameId = reader.GetInt32(0),
                    Side = reader.GetString(1),
                    Amount = (float)reader.GetDouble(2),
                    Status = reader.GetString(3)
                });
            }

            //Get data from api
            dynamic allMatches = new ApiConnection().GetMatches("?ids=" + string.Join(",", GetAllGameIds(table)));

            List<Match> result = new();

            foreach (dynamic match in allMatches.matches)
            {
                float? amount = GetMoneyFromDbTableByGameId(table, (int)match.id);
                string side = GetSideFromDbTableByGameId(table, (int)match.id);
                string status = GetStatusFromDbTableByGameId(table, (int)match.id);

                if (amount == null || side == null || status == null)
                {
                    continue;
                }

                result.Add(new Match
                {

                    GameName = (string)match.homeTeam.name + " - " + (string)match.awayTeam.name,
                    Date = (DateTime)match.utcDate,
                    Money = (float)amount,
                    Team = (side == "HomeTeam") ? (string)match.homeTeam.name : (side == "AwayTeam") ? (string)match.awayTeam.name : "Draw",
                    Status = ((match.score.winner == "HOME_TEAM" && side == "HomeTeam") ||
                        (match.score.winner == "AWAY_TEAM" && side == "AwayTeam") ||
                        (match.score.winner == "DRAW" && side == "Draw")) ? "Выигрыш" :
                        (match.score.winner == null) ? "Ожидание" :
                        "Проигрыш"
                });
            }

            return result;
        }
        private string GetStatusFromDbTableByGameId(List<DbHistoryRow> table, int gameId)
        {
            foreach (DbHistoryRow row in table)
            {
                if (row.GameId == gameId)
                {
                    return row.Status;
                }
            }

            return null;
        }

        private string GetSideFromDbTableByGameId(List<DbHistoryRow> table, int gameId)
        {
            foreach (DbHistoryRow row in table)
            {
                if (row.GameId == gameId)
                {
                    return row.Side;
                }
            }

            return null;
        }

        private float? GetMoneyFromDbTableByGameId(List<DbHistoryRow> table, int gameId)
        {
            foreach (DbHistoryRow row in table)
            {
                if (row.GameId == gameId)
                {
                    return row.Amount;
                }
            }

            return null;
        }

        private List<int> GetAllGameIds(List<DbHistoryRow> table)
        {
            List<int> result = new();

            foreach (DbHistoryRow row in table)
            {
                result.Add(row.GameId);
            }

            return result;
        }
    }
}
